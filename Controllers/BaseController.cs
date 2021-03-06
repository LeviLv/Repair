﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using Repair.Entitys;

namespace Repair.Controllers
{
    public class BaseController : Controller
    {
        private readonly IMemoryCache _cache;

        public BaseController(IMemoryCache cache) => _cache = cache;

        public BaseController()
        {

        }

        protected JsonResult Success()
        {
            return Json(Result.Successed);
        }

        protected JsonResult Success(object data)
        {
            return Json(new { code = 0, data = data });
        }

        protected JsonResult Fail(int code, string msg)
        {
            return Json(Result.Fail(code, msg));
        }

        protected JsonResult Fail(string msg)
        {
            return Json(Result.Fail(msg));
        }

        protected JsonResult Fail()
        {
            return Json(Result.Fail("失败"));
        }
        
        protected int GetUID()
        {
            var uid = HttpContext.User.Identity.Name;
            return int.Parse(uid);
        }

        protected bool IsLogin()
        {
            var uid = HttpContext.User.Identity.Name;
            return !string.IsNullOrWhiteSpace(uid);
        }
    }
}