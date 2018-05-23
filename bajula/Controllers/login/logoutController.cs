using System.Web.Mvc;
using tradelr.Common.Constants;
using tradelr.Libraries.ActionFilters;
using tradelr.Libraries.Facebook;
using tradelr.Library.Constants;

namespace tradelr.Controllers.login
{
    //[ElmahHandleError]
    public class logoutController : baseController
    {
        public ActionResult Index()
        {
            Session.Clear();
            ClearAuthCookie();
            ClearOldCookie();
            UtilFacebook.ClearFacebookCookies(Response, GeneralConstants.FACEBOOK_API_KEY);
            return Redirect("/");
        }

    }
}