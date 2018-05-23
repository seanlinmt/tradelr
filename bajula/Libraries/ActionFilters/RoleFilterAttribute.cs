using System.Net;
using System.Web;
using System.Web.Mvc;
using tradelr.Common;
using tradelr.Common.Constants;
using tradelr.Crypto.token;
using tradelr.Library;
using tradelr.Library.Constants;
using tradelr.Library.JSON;
using clearpixels.Logging;
using tradelr.Models.users;

namespace tradelr.Libraries.ActionFilters
{
    public class RoleFilterAttribute : ActionFilterAttribute
    {
        public UserRole role { get; set; }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            // ignore if role is NONE
            if (role == UserRole.NONE)
            {
                return;
            }
            var token = (TradelrSecurityToken)filterContext.HttpContext.Items["token"];

            // session expired
            if (token == null)
            {
                filterContext.RequestContext.HttpContext.Response.StatusCode = HttpStatusCode.Unauthorized.ToInt();
                if (filterContext.HttpContext.Request.IsAjaxRequest())
                {
                    filterContext.Result = new EmptyResult();
                }
                else
                {
                    filterContext.Result = new RedirectResult("/login?redirect=" + HttpUtility.UrlEncode(filterContext.HttpContext.Request.Url.PathAndQuery));
                }
                return;
            }

            var userRole = token.UserRole.ToEnum<UserRole>();

            if (userRole.HasFlag(role))
            {
                return; // ok
            }

            // not authorised
            Syslog.Write("No Auth Req: ", filterContext.HttpContext.Request.RawUrl, " ", token.UserID);

            if (filterContext.IsChildAction)
            {
                // don't set status here as it seems to cause IIS to return error page
                filterContext.Result = new EmptyResult();
            }
            else if (filterContext.HttpContext.Request.IsAjaxRequest())
            {
                filterContext.RequestContext.HttpContext.Response.StatusCode = HttpStatusCode.Forbidden.ToInt();
                filterContext.Result = new EmptyResult();

            }
            else
            {
                filterContext.RequestContext.HttpContext.Response.StatusCode = HttpStatusCode.Forbidden.ToInt();
                filterContext.Result = new ViewResult() { ViewName = "~/Views/Error/NoPermission.aspx" };

            }
        }

    }
}
