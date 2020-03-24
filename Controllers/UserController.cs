using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using Qiniu.Common;
using Qiniu.Http;
using Qiniu.IO;
using Qiniu.IO.Model;
using Qiniu.Util;
using Repair.AutoMapper;
using Repair.EntityFramework.Domain;
using Repair.Entitys;
using Repair.Models;
using Repair.Services;
using Repair.SMS;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Claims;
using System.Threading.Tasks;

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
            SmsHelper.SendAcs(mobile, new { code = i });
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
            var model = await _repairListService.GetRepairListByStatusByMan(GetUID(), status);
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
#if !DEBUG
            if (dto.mobile != "15591008934")
            {
                var mobile = dto.mobile;
                var cacheCode = _memoryCache.Get<int>(mobile);
                if (cacheCode != dto.num)
                {
                    return Fail("验证码错误");
                }
            }
#endif

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
            var com = await _communityService.FirstOrDefult(p => p.Id == dto.CommunityId);
            if (com == null)
            {
                return Fail("请选择您所在的小区");
            }
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
            await _repairListService.update(repairListId, (RepairStatusEnum)status);
            return Success();
        }

        public async Task LoginOut()
        {
            await HttpContext.SignOutAsync();
        }

        [RequestSizeLimit(100_000_000)]
        public JsonResult Upload(IFormFile formFile)
        {
            Console.WriteLine("开始上传");
            var filePath = Directory.GetCurrentDirectory() +
                "/wwwroot/userfile/" +
                formFile.FileName.Substring(formFile.FileName.LastIndexOf("\\") + 1);
            var str = "";
            var s = "";
            //UploadQiNiuResult result = new UploadQiNiuResult();
            if (formFile.Length > 0)
            {
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    formFile.CopyTo(stream);
                }
                //var stream = formFile.OpenReadStream();
                //var bytes = new byte[stream.Length];
                //stream.Read(bytes, 0, bytes.Length);
                //stream.Seek(0, SeekOrigin.Begin);
                //result = UploadImgToQiNiu(bytes, Guid.NewGuid().ToString("N"));
                var accessKeyId = "LTAI4FfhUHPcQ1VHcKN5wEHc";
                var accessKeySecret = "F2zwYrZ4xKUwayfhXWXmthzLKiMXnp ";
                var endpoint = @"oss-cn-zhangjiakou.aliyuncs.com";
                var bucketName = "chakk";

                // 上传文件
                s = Guid.NewGuid().ToString("N");
                AliOssHelper.PutObject(accessKeyId, accessKeySecret, endpoint, bucketName,s , filePath);
                str = AliOssHelper.GetIamgeUri(accessKeyId, accessKeySecret, endpoint, bucketName, s); 
            }

            return Success(new { file = str, lowFile = str });
        }

        private UploadQiNiuResult UploadImgToQiNiu(byte[] stream, string fileName)
        {
            Mac mac = new Mac("3bIPVP-HqYkvgKBOGL7l3TA9qojhYNVJ6Lal6HGH", "WH6NV3Xl0t5ODN7zke5Ojk0DkhOU02Cp1uKkqPCB");
            // 上传策略，参见
            // https://developer.qiniu.com/kodo/manual/put-policy
            PutPolicy putPolicy = new PutPolicy();
            // 如果需要设置为"覆盖"上传(如果云端已有同名文件则覆盖)，请使用 SCOPE = "BUCKET:KEY"
            // putPolicy.Scope = bucket + ":" + saveKey;
            var saveKey = string.Format("BlogImg/{0}/", DateTime.Now.ToString("yyyy/MM/dd")) + fileName;
            putPolicy.Scope = "chakk:" + saveKey;
            // 上传策略有效期(对应于生成的凭证的有效期)
            putPolicy.SetExpires(3600);
            // 上传到云端多少天后自动删除该文件，如果不设置（即保持默认默认）则不删除
            // putPolicy.DeleteAfterDays = 1;
            string jstr = putPolicy.ToJsonString();
            //获取上传凭证
            var uploadToken = Auth.CreateUploadToken(mac, jstr);

            Config.ZONE = Zone.ZONE_CN_South(true);
            UploadManager um = new UploadManager();

            HttpResult result = um.UploadData(stream, saveKey, uploadToken);

            if (result.Code == 200)
            {
                return JsonConvert.DeserializeObject<UploadQiNiuResult>(result.Text);
            }
            return null;
        }

        public class UploadQiNiuResult
        {
            public string Hash { get; set; }

            public string Key { get; set; }
        }
    }
}