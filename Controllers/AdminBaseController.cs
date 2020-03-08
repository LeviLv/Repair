using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Repair.Entitys;
using Repair.Models;

namespace Repair.Controllers
{
    public class AdminBaseController : Controller
    {
        protected LoginDto loginDto = new LoginDto();

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var currId = context.HttpContext.Session.GetString("currId");
            var currRole = context.HttpContext.Session.GetString("currRole");


            var ac = context.RouteData.Values["action"].ToString();
#if !DEBUG
            if (ac != "AdminLogin" && ac != "Login" && currId == null)
            {
                context.HttpContext.Response.Redirect("/Admin/Login");
                return;
            }

            if (currId != null)
            {
                var s = currId.ToString();
                loginDto.CurrentId = int.Parse(s);
                loginDto.CurrentRole = currRole.ToString();
            }
#endif
        }

        public int IsSuper()
        {
#if DEBUG
            return 1;
#endif
#if !DEBUG
            return loginDto.CurrentId == 9 ? 1 : 0;
#endif
        }

        protected List<int> GetRole()
        {
            if (!loginDto.CurrentId.HasValue)
            {
                HttpContext.Response.Redirect("/Admin/Login");
            }

            return loginDto.CurrentRole.Split(',').Select(int.Parse).ToList();
        }

        protected JsonResult Success()
        {
            return Json(Result.Successed);
        }

        protected JsonResult Success(object data)
        {
            return Json(new {code = 0, data = data});
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

        public class data<T>
        {
            public data()
            {
            }

            public T List { get; set; }
            public long Total { get; set; }
        }

        public class JsonResponse<T>
        {
            public JsonResponse()
            {
            }

            public JsonResponse(ResponseStatus status)
            {
                this.ResponseStatus = (int) status;
            }

            public JsonResponse(int status)
            {
                this.ResponseStatus = status;
            }

            /// <summary>
            /// 0,成功; -1到-5,程序异常; -6开始逻辑不允许
            /// </summary>
            public int ResponseStatus { get; private set; }

            public string ResponseMsg { get; set; }
            public T ResponseData { get; set; }
        }

        public class JsonApiResponse<T>
        {
            public JsonApiResponse()
            {
            }

            public JsonApiResponse(ResponseStatus status)
            {
                this.errorCode = (int) status;
                this.status = this.errorCode == 0 ? "SUCCESS" : "ERROR";
            }

            public JsonApiResponse(int status)
            {
                this.errorCode = status;
                this.status = this.errorCode == 0 ? "SUCCESS" : "ERROR";
            }

            /// <summary>
            /// 0,成功; -1到-5,程序异常; -6开始逻辑不允许
            /// </summary>
            public int errorCode { get; private set; }

            /// <summary>
            /// 该属性只是针对status的说明
            /// </summary>
            public string errorMessage { get; set; }

            /// <summary>
            /// 当status为SUCCESS时，才可能有值
            /// </summary>
            public T data { get; set; }

            /// <summary>
            /// SUCCESS/ERROR，只有当返回的结果不影响流程才为SUCCESS
            /// </summary>
            public string status { get; set; }
        }

        #region 按照这个返回

        public JsonResult Jsons(OperationResultType responseStatus = 0, string responseMsg = null)
        {
            return Jsons(null, responseStatus, responseMsg);
        }

        public JsonResult Jsons(dynamic dyn, OperationResultType responseStatus = 0, string responseMsg = null)
        {
            JsonResponse<dynamic> resp = new JsonResponse<dynamic>((int) responseStatus);
            data<dynamic> data = new data<dynamic>();
            data.List = dyn.List;
            data.Total = dyn.Total;
            resp.ResponseData = data;
            resp.ResponseMsg = responseMsg;
            resp.ResponseMsg = "成功";
            //var z = Json(resp).Data;
            return Json(resp);
        }

        #endregion
    }

    /// <summary>
    /// 返回状态枚举
    /// </summary>
    public enum ResponseStatus
    {
        /// <summary>
        /// 成功
        /// </summary>
        Success = 0,

        /// <summary>
        /// 服务器出现异常
        /// </summary>
        SystemException = -1,

        /// <summary>
        /// 操作失败,提示用户消息
        /// </summary>
        ErrorAlertMsg = -2,

        /// <summary>
        /// 业务逻辑错误
        /// </summary>
        LogicError = -6,

        /// <summary>
        /// 用户未登录
        /// </summary>
        Unlogin = -7,

        /// <summary>
        /// 输入参数异常
        /// </summary>
        ArgumentInputException = -9,

        /// <summary>
        /// 未注册
        /// </summary>
        UnRegist = -11,

        /// <summary>
        /// 参数验证未通过
        /// </summary>
        ArgumentValidateFailed = -13,

        /// <summary>
        /// 无效的参数（找不到对应数据）
        /// </summary>
        InvalidArgument = -15,

        /// <summary>
        /// 当前时间不允许预订
        /// </summary>
        CurrentTimeCanNotBook = -17,

        /// <summary>
        /// 当前时间已订满
        /// </summary>
        BookDeskNoMore = -19,

        /// <summary>
        /// 已经预定过
        /// </summary>
        HadBookedDesk = -21,

        /// <summary>
        /// 尚未通过审核
        /// </summary>
        HaveNotPassedApprove = -23,


        /// <summary>
        /// 重复的操作
        /// </summary>
        RepeatOperation = -25,

        /// <summary>
        /// 已到店
        /// </summary>
        ArrivedIn = -27,

        /// <summary>
        /// 禁止登录
        /// </summary>
        DisableLogin = -29
    }


    /// <summary>
    ///     表示业务操作结果的枚举
    /// </summary>
    [Description("业务操作结果的枚举")]
    public enum OperationResultType
    {
        /// <summary>
        ///     操作成功
        /// </summary>
        [Description("操作成功.")] Success,

        /// <summary>
        ///     操作取消或操作没引发任何变化
        /// </summary>
        [Description("操作没有引发任何变化，提交取消.")] NoChanged,

        /// <summary>
        ///     参数错误
        /// </summary>
        [Description("参数错误.")] ParamError,

        /// <summary>
        ///     指定参数的数据不存在
        /// </summary>
        [Description("指定参数的数据不存在.")] QueryNull,

        /// <summary>
        ///     权限不足
        /// </summary>
        [Description("当前用户权限不足，不能继续操作.")] PurviewLack,

        /// <summary>
        ///     非法操作
        /// </summary>
        [Description("非法操作.")] IllegalOperation,

        /// <summary>
        ///     警告
        /// </summary>
        [Description("警告")] Warning,

        /// <summary>
        ///     操作引发错误
        /// </summary>
        [Description("操作引发错误.")] Error,
    }
}