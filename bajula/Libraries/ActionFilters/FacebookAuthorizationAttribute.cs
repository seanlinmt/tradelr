using System;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.Configuration;
using FacebookToolkit.Schema;
using FacebookToolkit.Session;

namespace tradelr.Libraries.ActionFilters
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class FacebookAuthorizationAttribute : ActionFilterAttribute
    {
        public bool IsFbml { get; set; }

        /// <summary> 
        /// The APi key for this application given by facebook
        /// </summary>
        public string ApiKey { get; set; }

        /// <summary> 
        /// The APi Secret for this application given by facebook
        /// </summary>
        public string Secret { get; set; }
        /// <summary> 
        /// The comma separated list of extended permissions
        /// </summary>
        public string RequiredPermissions { get; set; }

        public override void OnActionExecuting(ActionExecutingContext c)
        {
            CanvasSession session = null;
            if (IsFbml)
            {
                if (!string.IsNullOrEmpty(RequiredPermissions))
                {
                    session = new FBMLCanvasSession(ApiKey ?? WebConfigurationManager.AppSettings["ApiKey"], Secret ?? WebConfigurationManager.AppSettings["Secret"], ParsePermissions(RequiredPermissions));
                }
                else
                {
                    session = new FBMLCanvasSession(ApiKey ?? WebConfigurationManager.AppSettings["ApiKey"], Secret ?? WebConfigurationManager.AppSettings["Secret"]);
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(RequiredPermissions))
                {
                    session = new IFrameCanvasSession(ApiKey ?? WebConfigurationManager.AppSettings["ApiKey"], Secret ?? WebConfigurationManager.AppSettings["Secret"], ParsePermissions(RequiredPermissions));
                }
                else
                {
                    session = new IFrameCanvasSession(ApiKey ?? WebConfigurationManager.AppSettings["ApiKey"], Secret ?? WebConfigurationManager.AppSettings["Secret"]);
                }
            }
            if (string.IsNullOrEmpty(session.SessionKey))
            {
                c.Result = new ContentResult { Content = session.GetRedirect() };
            }
            else
            {
                var permissionsString = session.CheckPermissions();
                if (!string.IsNullOrEmpty(permissionsString))
                {
                    c.Result = new ContentResult { Content = session.GetPermissionsRedirect(session.GetPermissionUrl(permissionsString, session.GetNextUrl())) };
                }
            }
        }

        public override void OnResultExecuted(ResultExecutedContext c)
        {
            base.OnResultExecuted(c);
            if (!IsFbml)
            {
                c.HttpContext.Response.AppendHeader("P3P", "CP=\"CAO PSA OUR\"");
            }

        }
        private List<Enums.ExtendedPermissions> ParsePermissions(string permissions)
        {
            if (string.IsNullOrEmpty(permissions))
                return null;
            string[] input = permissions.Split(',');
            List<Enums.ExtendedPermissions> output = new List<Enums.ExtendedPermissions>();
            foreach (var item in input)
            {
                output.Add((Enums.ExtendedPermissions)Enum.Parse(typeof(Enums.ExtendedPermissions), item.Trim(), true));
            }
            return output;

        }

    }
}
