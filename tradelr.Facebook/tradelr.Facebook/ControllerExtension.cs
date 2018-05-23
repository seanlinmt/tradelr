using System.Web.Mvc;
using System.Web.Configuration;
using FacebookToolkit.Rest;
using FacebookToolkit.Session;

namespace tradelr.Facebook
{
    public static class ControllerExtension
    {
        public static Api GetApi(this Controller controller, string apiKey, string secret)
        {
            FBMLCanvasSession session = new FBMLCanvasSession(apiKey ?? WebConfigurationManager.AppSettings["ApiKey"], secret ?? WebConfigurationManager.AppSettings["Secret"]);
            return new Api(session);
        }
        public static Api GetApi(this Controller controller)
        {
            FBMLCanvasSession session = new FBMLCanvasSession(WebConfigurationManager.AppSettings["ApiKey"], WebConfigurationManager.AppSettings["Secret"]);
            return new Api(session);
        }

        public static Api GetApiIFrame(this Controller controller, string apiKey, string secret)
        {
            IFrameCanvasSession session = new IFrameCanvasSession(apiKey ?? WebConfigurationManager.AppSettings["ApiKey"], secret ?? WebConfigurationManager.AppSettings["Secret"], controller.ControllerContext.HttpContext.Session.Contents);

            return new Api(session);
        }

        public static Api GetApiIFrame(this Controller controller)
        {

            IFrameCanvasSession session = new IFrameCanvasSession(WebConfigurationManager.AppSettings["ApiKey"], WebConfigurationManager.AppSettings["Secret"], controller.ControllerContext.HttpContext.Session.Contents);
            return new Api(session);
        }
    }
}
