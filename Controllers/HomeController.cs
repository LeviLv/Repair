using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Repair.AutoMapper;
using Repair.EntityFramework.Domain;
using Repair.Entitys;
using Repair.Models;
using Repair.Services;

namespace Repair.Controllers
{

    [Authorize]
    public class HomeController : BaseController
    {
        private readonly ILogger<HomeController> _logger;
        private readonly UserService _userService;
        private readonly CommunityService _communityService;
        private readonly RepairListService _repairListService;

        public HomeController(ILogger<HomeController> logger
            , UserService userService
            , CommunityService communityService
            , RepairListService repairListService)
        {
            _logger = logger;
            _userService = userService;
            _communityService = communityService;
            _repairListService = repairListService;
        }

        public async Task<IActionResult> Index()
        {
            ViewBag.User = JsonConvert.SerializeObject(await _userService.GetUserAsync(GetUID()));
            ViewBag.Status1 = (await _repairListService.GetRepairListByStatus(GetUID(), null)).Count;
            ViewBag.Status2 = (await _repairListService.GetRepairListByStatus(GetUID(), 2)).Count;
            ViewBag.Status3 = (await _repairListService.GetRepairListByStatus(GetUID(), 4)).Count;
            ViewBag.Status4 = (await _repairListService.GetRepairListByStatus(GetUID(), 5)).Count;
            return View();
        }

        public IActionResult RepairInfo()
        {
            return View();
        }

        public IActionResult News()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
