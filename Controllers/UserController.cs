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

        public UserController(UserService userService, CommunityService communityService,
            RepairListService repairListService, RepairListInfoService repairListInfoService)
        {
            _userService = userService;
            _communityService = communityService;
            _repairListService = repairListService;
            _repairListInfoService = repairListInfoService;
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
            return View();
        }

        public async Task<IActionResult> RepairList(int? status)
        {
            var model = await _repairListService.GetRepairListByStatus(GetUID(), status);
            return View(model);
        }

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

        public async Task LoginOut()
        {
            await HttpContext.SignOutAsync();
        }
    }
}