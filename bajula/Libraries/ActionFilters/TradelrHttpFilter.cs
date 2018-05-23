using System.IO.Compression;
using System.Web;
using System.Web.Mvc;

namespace tradelr.Libraries.ActionFilters
{
    public class TradelrHttp : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            // If the request has arrived via HTTPS...
            if (filterContext.HttpContext.Request.IsSecureConnection)
            {
                filterContext.Result = new RedirectResult(filterContext.HttpContext.Request.Url.ToString().Replace("https:", "http:"));
            }
            base.OnActionExecuting(filterContext);
        }
    }

}