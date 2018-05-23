using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using tradelr.Common;
using tradelr.Crypto.token;
using tradelr.Library;
using tradelr.Library.JSON;
using tradelr.Models.users;

namespace tradelr.Libraries.ActionFilters
{
    public class PermissionFilterAttribute : ActionFilterAttribute
    {
        public UserPermission permission { get; set; }
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var token = (TradelrSecurityToken)filterContext.HttpContext.Items["token"];
            if (token != null)
            {
                var userperm = token.Permission.ToEnum<UserPermission>();
                if (!userperm.HasFlag(permission))
                {
                    if (filterContext.HttpContext.Request.IsAjaxRequest())
                    {
                        var jsondata = JavascriptReturnCodes.NOPERMISSION.ToJsonFailData();
                        filterContext.Result = new JsonResult() { Data = jsondata , JsonRequestBehavior = JsonRequestBehavior.AllowGet};
                    }
                    else
                    {
                        filterContext.Result = new ViewResult()
                                                   {
                                                       ViewName = "~/Views/error/NoPermission.aspx"
                                                   };
                    }
                    return;
                }
            }
            base.OnActionExecuting(filterContext);
        }
    }
}