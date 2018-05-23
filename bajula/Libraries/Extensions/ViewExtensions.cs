using System;
using System.ComponentModel;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using tradelr.Common.Library;
using tradelr.Libraries.Helpers;
using tradelr.Libraries.Loader;
using tradelr.Library;

namespace tradelr.Libraries.Extensions
{
    public static class ViewExtensions
    {
        public static void RenderControl(this HtmlHelper helper,
                                         TradelrControls control)
        {
            RenderControl(helper, control, null);
        }

        public static void RenderControl(this HtmlHelper helper,
                                         TradelrControls control, object data)
        {
            //data = data ?? new object();
            string controlName = control.ToDescriptionString();
            helper.RenderPartial(controlName, data, helper.ViewContext.ViewData);
        }

        /// <summary>
        /// A helper for registering script tags on an MVC View page.
        /// </summary>
        public static string RegisterViewJS(this HtmlHelper helper)
        {
            //get the directory where the scripts are
            var segments = helper.ViewDataContainer.ToString().Split(new[] {'_','.'});
            // skip first one
            string path = "";
            for (int i = 1; i < segments.Length-1; i++)
            {
                path = string.Concat(path, "/", segments[i]);
            }
            StringBuilder sb = new StringBuilder();
            sb.Append("<script type='text/javascript'>");
            sb.Append(JsLoader.Instance.LoadViewJavascript(path));
            sb.Append("</script>");
            return sb.ToString();
        }
    }
}