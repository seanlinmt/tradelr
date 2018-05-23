using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using tradelr.Models.mobile;

namespace tradelr.Libraries.ActionFilters
{
    public class HandleMobileBrowser : ActionFilterAttribute
    {
        // only use mobile theme if brower width and height is less than 1000px. iphone 4 has resolution of 960px
        // for a list of common device sizes, http://resizemybrowser.com/
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var theme = filterContext.HttpContext.Request.QueryString["theme"];
            var useMobileThemes = false;

            if (filterContext.HttpContext.Session[BrowserCapability.IsMobileSession] == null)
            {
                if (filterContext.HttpContext.Request.Browser.ScreenPixelsWidth < 1000 &&
                    filterContext.HttpContext.Request.Browser.ScreenPixelsHeight < 1000 &&
                    filterContext.HttpContext.Request.Browser.IsMobileDevice)
                {
                    useMobileThemes = true;
                }
                filterContext.HttpContext.Session[BrowserCapability.IsMobileSession] = useMobileThemes;
            }

            switch (theme)
            {
                case "mobile":
                    filterContext.HttpContext.Session[BrowserCapability.IsMobileSession] = true;
                    break;
                case "main":
                    filterContext.HttpContext.Session[BrowserCapability.IsMobileSession] = false;
                    break;
                default:
                    break;
            }

            base.OnActionExecuting(filterContext);
        }
    }
}