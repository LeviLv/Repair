using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Repair.EntityFramework.Domain;
using Repair.Entitys;
using Repair.Models;
using Repair.Services;
using System.Threading.Tasks;

namespace Repair.Controllers
{
    public class AdminController : AdminBaseController
    {
        private readonly UserService _userService;

        private readonly CommunityService _communityService;

        private readonly RepairListService _repairListService;

        private readonly RepairListInfoService _repairListInfoService;

        private readonly AdminRoleService _adminRoleService;

        public AdminController(UserService userService
            , CommunityService communityService
            , RepairListService repairListService
            , RepairListInfoService repairListInfoService
            , AdminRoleService adminRoleService)
        {
            _userService = userService;
            _communityService = communityService;
            _repairListService = repairListService;
            _repairListInfoService = repairListInfoService;
            _adminRoleService = adminRoleService;
        }

        public IActionResult Index()
        {
            ViewBag.IsSuper = IsSuper();
            return View();
        }


        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpGet("AdminLogin")]
        public async Task<JsonResult> LoginAsync(string mobile, string pwd)
        {
            var b = await _userService.AdminLogin(mobile, pwd);
            if (b)
            {
                var user = await _userService.GetUserByMobile(mobile);
                var role = await _adminRoleService.GetAdminRoleString(user.Id);
                loginDto.CurrentId = user.Id;
                loginDto.CurrentRole = role;
                HttpContext.Session.SetString("currId", user.Id.ToString());
                HttpContext.Session.SetString("currRole", role);
            }

            return Success(b);
        }

        #region 用户列表


        /// <summary>
        /// 用户列表
        /// </summary>
        /// <returns></returns>
        public IActionResult UserList()
        {
            ViewBag.SearchModel = new PageBase();
            return View();
        }

        public async Task<JsonResult> GetUserList(QueryUserModel pageBase)
        {
            var list = await _userService.GetUserList(pageBase);
            return Jsons(list);
        }
        
        public async Task<JsonResult> GetUserList2(QueryUserModel pageBase)
        {
            var list = await _userService.GetUserList2(pageBase);
            return Jsons(list);
        }

        public async Task<JsonResult> MakeRepairMan(int userId, string communityIds)
        {
            await _userService.MakeRepairMan(userId, communityIds);
            return Success();
        }

        public async Task<JsonResult> CancleRepairMan(int userId)
        {
            await _userService.CancleRepairMan(userId);
            return Success();
        }


        #endregion

        #region 修理工列表


        /// <summary>
        /// 修理工列表
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> RepairManList()
        {
            var comm = await _communityService.FirstOrDefult(p => p.AdminId == loginDto.CurrentId.Value);
            var model = new QueryRepairManModel();
            if (IsSuper() != 1)
            {
                model.communnityId = comm?.Id;
            }
            ViewBag.SearchModel = model;
            return View();
        }

        public async Task<JsonResult> GetRepairManList(QueryRepairManModel pageBase)
        {
            var list = await _userService.GetRepairManList(pageBase);
            return Jsons(list);
        }

        #endregion

        #region 社区列表


        /// <summary>
        /// 社区列表
        /// </summary>
        /// <returns></returns>
        public IActionResult CommunityList()
        {
            ViewBag.SearchModel = new QueryCommunityModel();
            ViewBag.IsSuper = IsSuper();
            return View();
        }


        public IActionResult InsertCommunity()
        {
            return View();
        }

        public IActionResult CommunityMan(int commuityId,int type = 0)
        {
            var SearchModel = new QueryUserModel();
            if (type == 1 || type == 4)
            {
                SearchModel.IsRepair = true;
            }

            ViewBag.SearchModel = SearchModel;
            return View();
        }

        public async Task<JsonResult> GetCommunityList(QueryCommunityModel pageBase)
        {
            if (loginDto.CurrentId == 9)
            {
                pageBase.AdminId = null;
            }

            var list = await _communityService.GetList(pageBase);
            return Jsons(list);
        }

        public async Task<JsonResult> SetCommuityRepairMan(int commuityId, int userId)
        {
            await _communityService.SetCommuityRepairMan(commuityId, userId);
            return Success();
        }

        public async Task<JsonResult> SetCommuityAdmin(int commuityId, int userId)
        {
            var name = await _communityService.GetCommunityAdminMan(commuityId);
            if (!string.IsNullOrWhiteSpace(name))
            {
                return Fail($"用户【{name}】 已经是该小区的负责人！");
            }
            await _communityService.SetCommuityAdmin(commuityId, userId);
            return Success();
        }

        public async Task<JsonResult> InsertOrUpdateCommunity(Community community)
        {
            if (community.Id.HasValue && community.Id == 0)
            {
                await _communityService.Insert(community);
            }
            else
            {
                await _communityService.Update(community);
            }

            return Success();
        }

        public async Task<JsonResult> AddCommunityAsync(string name)
        {
            await _communityService.Insert(new Community()
            {
                Name = name
            });
            return Success();
        }


        #endregion

        public IActionResult RepairList()
        {
            ViewBag.SearchModel = new QueryRepairListModel();
            return View();
        }

        public async Task<JsonResult> GetRepairList(QueryRepairListModel pageBase)
        {
            pageBase.AdminId = loginDto.CurrentId;
            var list = await _repairListService.PageRepairList(pageBase);
            return Jsons(list);
        }

        /// <summary>
        /// 指派修理工接单
        /// </summary>
        /// <param name="repairId"></param>
        /// <param name="repairManId"></param>
        /// <returns></returns>
        public async Task<JsonResult> AppointRepairMan(int repairId, int repairManId)
        {
            await _repairListService.AppointRepairMan(repairId, repairManId);
            return Success();
        }
    }
}