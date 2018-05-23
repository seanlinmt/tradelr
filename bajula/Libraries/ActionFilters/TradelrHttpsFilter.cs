using System.IO.Compression;
using System.Web;
using System.Web.Mvc;
using tradelr.Common.Constants;
using tradelr.Library.Constants;

namespace tradelr.Libraries.ActionFilters
{
    public class TradelrHttps : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            // If the request has arrived via HTTP
#if SUPPORT_HTTPS
            if (!filterContext.HttpContext.Request.IsSecureConnection)
            {
                // redirect to tradelr https if on custom domain
                var hostname = filterContext.HttpContext.Request.Url.Host;
                if (hostname.EndsWith(GeneralConstants.SUBDOMAIN_HOST))
                {
                    filterContext.Result = new RedirectResult(filterContext.HttpContext.Request.Url.ToString().Replace("http:", "https:"));
                }
            }
#endif
            base.OnActionExecuting(filterContext);
        }
    }

}