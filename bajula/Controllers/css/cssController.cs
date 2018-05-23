using System;
using System.Collections.Generic;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using tradelr.Areas.dashboard.Models.theme;
using tradelr.Libraries.ActionFilters;
using tradelr.Libraries.Loader;
using tradelr.Library.Caching;
using tradelr.Library.Caching.SimpleCache;
using tradelr.Library.Constants;
using tradelr.Models.liquid;

namespace tradelr.Controllers.css
{
    //[ElmahHandleError]
    public class cssController : Controller
    {
        private const string CONTENT_TYPE_CSS = "text/css";
        public ActionResult checkout()
        {
            var csscontent = CssLoader.Instance.LoadFeatures("checkout.xml");
            Response.AddFileDependencies(csscontent.filenames.ToArray());
            Response.Cache.SetLastModifiedFromFileDependencies();
            Response.Cache.SetETagFromFileDependencies();
            Response.Cache.SetCacheability(HttpCacheability.Public);
            Response.Cache.SetExpires(DateTime.Now.AddDays(1));
            Response.ContentType = CONTENT_TYPE_CSS;
            return Content(csscontent.content);
        }

        public ActionResult inside()
        {
            var csscontent = CssLoader.Instance.LoadFeatures("inside.xml");
            Response.AddFileDependencies(csscontent.filenames.ToArray());
            Response.Cache.SetLastModifiedFromFileDependencies();
            Response.Cache.SetETagFromFileDependencies();
            Response.Cache.SetCacheability(HttpCacheability.Public);
            Response.Cache.SetExpires(DateTime.Now.AddDays(1));
            Response.ContentType = CONTENT_TYPE_CSS;
            return Content(csscontent.content);
        }

        public ActionResult mediapicker()
        {
            var csscontent = CssLoader.Instance.LoadFeatures("mediapicker.xml");
            Response.AddFileDependencies(csscontent.filenames.ToArray());
            Response.Cache.SetLastModifiedFromFileDependencies();
            Response.Cache.SetETagFromFileDependencies();
            Response.Cache.SetCacheability(HttpCacheability.Public);
            Response.Cache.SetExpires(DateTime.Now.AddDays(1));
            Response.ContentType = CONTENT_TYPE_CSS;
            return Content(csscontent.content);
        }

        public ActionResult outside()
        {
            var csscontent = CssLoader.Instance.LoadFeatures("outside.xml");
            Response.AddFileDependencies(csscontent.filenames.ToArray());
            Response.Cache.SetLastModifiedFromFileDependencies();
            Response.Cache.SetETagFromFileDependencies();
            Response.Cache.SetCacheability(HttpCacheability.Public);
            Response.Cache.SetExpires(DateTime.Now.AddDays(1));
            Response.ContentType = CONTENT_TYPE_CSS;
            return Content(csscontent.content);
        }
    }
}
