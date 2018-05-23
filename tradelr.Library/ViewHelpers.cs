using System;
using System.IO;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Web.UI;
using tradelr.Library.Constants;
using tradelr.Models.users;

namespace tradelr.Library
{
    public static class ViewHelpers
    {
        public static string CssInclude(this HtmlHelper helper, string url, string mediaTypes)
        {
            var sb = new StringBuilder();
            sb.AppendLine(string.Format("<link href=\"{0}?v={1}\" rel=\"stylesheet\" type=\"text/css\" media=\"{2}\" />",
                url, 
                GeneralConstants.TIMESTAMP,
                mediaTypes));
            return sb.ToString();
        }

        public static string JavascriptInclude(this HtmlHelper helper, string url)
        {
            var sb = new StringBuilder();
#if !DEBUG
            if(url.EndsWith(".js"))
            {
                url = url.Replace(".js", ".min.js");
            }
#endif
            sb.AppendLine(string.Format("<script src=\"{0}?v={1}\" type=\"text/javascript\"></script>", url, GeneralConstants.TIMESTAMP));
            return sb.ToString();
        }

        public static string RenderViewToString(string viewName, ViewDataDictionary viewData)
        {
            var vp = new ViewPage { ViewData = viewData };
            var control = vp.LoadControl(viewName);

            vp.Controls.Add(control);

            var sb = new StringBuilder();
            using (var sw = new StringWriter(sb))
            {
                using (var tw = new HtmlTextWriter(sw))
                {
                    vp.RenderControl(tw);
                }
            }

            return sb.ToString();
        }

        public static string RenderViewToString(this Controller controller, string viewName, object viewData)
        {
            var sb = new StringBuilder();
            var memWriter = new StringWriter(sb);

            //Create fake http context to render the view
            var fakeResponse = new HttpResponse(memWriter);
            var fakeContext = new HttpContext(HttpContext.Current.Request, fakeResponse);
            var fakeControllerContext = new ControllerContext(
                new HttpContextWrapper(fakeContext),
                controller.ControllerContext.RouteData,
                controller.ControllerContext.Controller);

            var oldContext = HttpContext.Current;
            HttpContext.Current = fakeContext;

            //Use HtmlHelper to render partial view to fake context
            var html = new HtmlHelper(new ViewContext(fakeControllerContext,
                new FakeView(), new ViewDataDictionary(), new TempDataDictionary(), memWriter),
                new ViewPage());
            html.RenderPartial(viewName, viewData);

            //Restore context
            HttpContext.Current = oldContext;

            //Flush memory and return output
            memWriter.Flush();
            return sb.ToString();

        }
    }

    public class FakeView : IView
    {
        #region IView Members

        public void Render(ViewContext viewContext, TextWriter writer)
        {
            throw new NotImplementedException();
        }

        #endregion
    }

}
