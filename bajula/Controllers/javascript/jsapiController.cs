using System;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Caching;
using System.Web.Mvc;
using tradelr.Common;
using tradelr.Libraries.ActionFilters;
using tradelr.Libraries.Loader;
using tradelr.Library.Caching;
using tradelr.Library.Caching.SimpleCache;
using tradelr.Models.contacts;
using tradelr.Models.liquid;
using tradelr.Models.users;

namespace tradelr.Controllers.javascript
{
    //[ElmahHandleError]
    public class jsapiController : Controller
    {
        private const string CONTENT_TYPE_JAVASCRIPT = "application/x-javascript";

        public ActionResult colorpicker()
        {
            // try cache first
            LoadedContent jscontent;
            if (HttpContext.Cache[Request.Path] == null)
            {
                jscontent = JsLoader.Instance.LoadFeatures("colorpicker", "files.xml");
                HttpContext.Cache.Insert(Request.Path, jscontent, new CacheDependency(jscontent.filenames.ToArray()));
            }
            else
            {
                jscontent = (LoadedContent)HttpContext.Cache[Request.Path];
            }

            // handle caching
            Response.AddFileDependencies(jscontent.filenames.ToArray());
            Response.Cache.SetLastModifiedFromFileDependencies();
            Response.Cache.SetETagFromFileDependencies();
            Response.Cache.SetCacheability(HttpCacheability.Public);
            Response.Cache.SetExpires(DateTime.UtcNow.AddDays(1));
            return Content(jscontent.content, CONTENT_TYPE_JAVASCRIPT);
        }

        public ActionResult core()
        {
            LoadedContent jscontent;
            if (HttpContext.Cache[Request.Path] == null)
            {
                jscontent = JsLoader.Instance.LoadFeatures("root", "files.xml");
                jscontent.content = string.Concat(jscontent.content, GenerateJavascript());
                HttpContext.Cache.Insert(Request.Path, jscontent, new CacheDependency(jscontent.filenames.ToArray()));
            }
            else
            {
                jscontent = (LoadedContent)HttpContext.Cache[Request.Path];
            }

            Response.AddFileDependencies(jscontent.filenames.ToArray());
            Response.Cache.SetLastModifiedFromFileDependencies();
            Response.Cache.SetETagFromFileDependencies();
            Response.Cache.SetCacheability(HttpCacheability.Public);
            Response.Cache.SetExpires(DateTime.UtcNow.AddDays(1));
            return Content(jscontent.content, CONTENT_TYPE_JAVASCRIPT);
        }

        public ActionResult extend()
        {
            LoadedContent jscontent = new LoadedContent();
            if (HttpContext.Cache[Request.Path] == null)
            {
                var extend = JsLoader.Instance.LoadFeatures("extend", "files.xml");
                jscontent.filenames.AddRange(extend.filenames);
                jscontent.content = string.Concat(jscontent.content, extend.content);

                var jqgrid = JsLoader.Instance.LoadFeatures("jqgrid", "files.xml");
                jscontent.filenames.AddRange(jqgrid.filenames);
                jscontent.content = string.Concat(jscontent.content, jqgrid.content);

                var main = JsLoader.Instance.LoadFeatures("main", "files.xml");
                jscontent.filenames.AddRange(main.filenames);
                jscontent.content = string.Concat(jscontent.content, main.content);
                
                var offline = JsLoader.Instance.LoadFeatures("offline", "files.xml");
                jscontent.filenames.AddRange(offline.filenames);
                jscontent.content = string.Concat(jscontent.content, offline.content);

                HttpContext.Cache.Insert(Request.Path, jscontent, new CacheDependency(jscontent.filenames.ToArray()));
            }
            else
            {
                jscontent = (LoadedContent)HttpContext.Cache[Request.Path];
            }
            
            // handle caching
            Response.AddFileDependencies(jscontent.filenames.ToArray());
            Response.Cache.SetLastModifiedFromFileDependencies();
            Response.Cache.SetETagFromFileDependencies();
            Response.Cache.SetCacheability(HttpCacheability.Public);
            Response.Cache.SetExpires(DateTime.UtcNow.AddDays(1));
            
            return Content(jscontent.content, CONTENT_TYPE_JAVASCRIPT);
        }

        public ActionResult uploader()
        {
            LoadedContent jscontent;
            if (HttpContext.Cache[Request.Path] == null)
            {
                jscontent = JsLoader.Instance.LoadFeatures("ajaxUpload", "files.xml");
                HttpContext.Cache.Insert(Request.Path, jscontent, new CacheDependency(jscontent.filenames.ToArray()));
            }
            else
            {
                jscontent = (LoadedContent)HttpContext.Cache[Request.Path];
            }

            Response.AddFileDependencies(jscontent.filenames.ToArray());
            Response.Cache.SetLastModifiedFromFileDependencies();
            Response.Cache.SetETagFromFileDependencies();
            Response.Cache.SetCacheability(HttpCacheability.Public);
            Response.Cache.SetExpires(DateTime.UtcNow.AddDays(1));
            return Content(jscontent.content, CONTENT_TYPE_JAVASCRIPT);
        }

        private static string GenerateJavascript()
        {
            var sb = new StringBuilder();
#if DEBUG
            sb.Append("var DEBUG = true;");
#else
            sb.Append("var DEBUG = false;");
#endif
            // error codes
            sb.Append("var tradelr = tradelr || {};");
            sb.Append("tradelr.returncode = {};");
            var names = Enum.GetNames(typeof(JavascriptReturnCodes));
            var values = Enum.GetValues(typeof (JavascriptReturnCodes));
            for (var i = 0; i < names.Length; i++)
            {
                sb.Append("tradelr.returncode.");
                sb.Append(names[i]);
                sb.Append(" = ");
                sb.Append((int)values.GetValue(i));
                sb.Append(";");
            }

            // user permissions
            sb.Append("tradelr.userpermissions = {};");
            names = Enum.GetNames(typeof(UserPermission));
            values = Enum.GetValues(typeof(UserPermission));
            for (var i = 0; i < names.Length; i++)
            {
                sb.Append("tradelr.userpermissions.");
                sb.Append(names[i]);
                sb.Append(" = ");
                sb.Append((int)values.GetValue(i));
                sb.Append(";");
            }

            // contact types
            sb.Append("tradelr.contacts = {};");
            names = Enum.GetNames(typeof(ContactType));
            values = Enum.GetValues(typeof(ContactType));
            for (var i = 0; i < names.Length; i++)
            {
                sb.Append("tradelr.contacts.");
                sb.Append(names[i]);
                sb.Append(" = ");
                sb.Append((int)values.GetValue(i));
                sb.Append(";");
            }

            return sb.ToString();
        }
    }
}
