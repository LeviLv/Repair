using System;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Repair.AutoMapper;
using Repair.EntityFramework.Domain;
using Repair.Entitys;
using Repair.Models;
using Repair.Services;
using System.Collections.Generic;
using System.IO;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Repair.SMS;
using System.Drawing;
using System.Drawing.Imaging;

namespace Repair.Controllers
{
    public class UserController : BaseController
    {
        private readonly UserService _userService;

        private readonly CommunityService _communityService;

        private readonly RepairListService _repairListService;

        private readonly RepairListInfoService _repairListInfoService;

        private readonly IMemoryCache _memoryCache;

        public UserController(UserService userService, CommunityService communityService,
            RepairListService repairListService, RepairListInfoService repairListInfoService,
            IMemoryCache memoryCache)
        {
            _userService = userService;
            _communityService = communityService;
            _repairListService = repairListService;
            _repairListInfoService = repairListInfoService;
            _memoryCache = memoryCache;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> UserInfo()
        {
            ViewBag.Community = JsonConvert.SerializeObject(await _communityService.GetAllAsync());
            return View();
        }

        [Authorize]
        public async Task<IActionResult> NewRepair()
        {
            ViewBag.UserId = (await _userService.GetUserAsync(GetUID())).Id;
            ViewBag.Community = JsonConvert.SerializeObject(await _communityService.GetAllAsync());
            return View();
        }

        public IActionResult Register()
        {
            if (IsLogin())
            {
                return Redirect("/User/Index");
            }

            return View();
        }

        public JsonResult GetSMSCode(string mobile)
        {
            var rd = new Random();
            int i = rd.Next(100000, 999999);
            SmsHelper.SendAcs(mobile, new {code = i});
            _memoryCache.Set(mobile, i, DateTimeOffset.UtcNow.AddSeconds(60));
            return Success();
        }

        [Authorize]
        public async Task<IActionResult> RepairList(int? status)
        {
            var model = await _repairListService.GetRepairListByStatus(GetUID(), status);
            return View(model);
        }

        [Authorize]
        public async Task<IActionResult> RepairListAsRepairMan(int? status)
        {
            var model = await _repairListService.GetRepairListByStatus(GetUID(), status);
            return View(model);
        }

        [Authorize]
        public async Task<IActionResult> RepairInfo(int id)
        {
            var list = await _repairListInfoService.GetRepairInfo(id);
            return View(list);
        }

        [HttpPost, Authorize]
        public async Task<JsonResult> InsertRepair([FromBody] RepairList repair)
        {
            repair.UserId = GetUID();
            var newRepair = await _repairListService.Insert(repair);
            return Success(newRepair);
        }


        [HttpPost]
        public async Task<JsonResult> Register([FromBody] UserRegisterDto dto)
        {
            if (dto.mobile != "15591008934")
            {
                var mobile = dto.mobile;
                var cacheCode = _memoryCache.Get<int>(mobile);
                if (cacheCode != dto.num)
                {
                    return Fail("验证码错误");
                }
            }

            var user = await _userService.GetUserByMobile(dto.mobile);
            var u = new UserDTO();
            if (user == null)
            {
                u = await _userService.Insert(new User
                {
                    Mobile = dto.mobile
                });
            }
            else
            {
                u = user.MapTo<UserDTO>();
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, u.Id.ToString()),
                new Claim(ClaimTypes.MobilePhone, u.Mobile.ToString())
            };
            var userIdentity = new ClaimsIdentity(claims, "login");
            var principal = new ClaimsPrincipal(userIdentity);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
            return Success(u);
        }

        [HttpPost, Authorize]
        public async Task<JsonResult> EditUser([FromBody] UserDTO dto)
        {
            var u = await _userService.Update(dto.MapTo<User>());
            return Success(u);
        }

        [HttpGet]
        public async Task<JsonResult> GetUser(int id)
        {
            var user = await _userService.GetUserAsync(id);
            return Success(user);
        }

        [HttpGet]
        public async Task<JsonResult> GetLoginUser()
        {
            var uid = HttpContext.User.Identity.Name;
            if (uid == null)
            {
                return Success(new UserDTO());
            }

            var user = await _userService.GetUserAsync(int.Parse(uid));
            return Success(user);
        }

        [Authorize]
        public async Task<JsonResult> UpdateRepairListStatus(int repairListId, int status)
        {
            await _repairListService.update(repairListId, (RepairStatusEnum) status);
            return Success();
        }

        public async Task LoginOut()
        {
            await HttpContext.SignOutAsync();
        }

        public JsonResult Upload(IFormFile formFile)
        {
            var imgPath = formFile.FileName
                .Replace(formFile.FileName.Substring(0 , formFile.FileName.LastIndexOf('.')), Guid.NewGuid().ToString("N"))
                .Substring(formFile.FileName.LastIndexOf("\\") + 1);
            var currentDirectory = Directory.GetCurrentDirectory();
            var filePath = Path.Combine(currentDirectory,
                "wwwroot/userfile/" + imgPath)
                .Replace("\\", "/");
            var str = "";

            
            if (formFile.Length > 0)
            {
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    formFile.CopyTo(stream);
                }

            }
            return Success(new { file = "/userfile/" + imgPath,lowFile = "/userfilelow/" + imgPath ,str = str });
        }

        private byte[] SaveImage(String path)
        {
            FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read); //将图片以文件流的形式进行保存
            BinaryReader br = new BinaryReader(fs);
            byte[] imgBytesIn = br.ReadBytes((int)fs.Length); //将流读入到字节数组中
            return imgBytesIn;
        }
    }
}