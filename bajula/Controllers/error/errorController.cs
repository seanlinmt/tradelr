using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.UI.WebControls;
using tradelr.Controllers.liquid;
using tradelr.Library;
using tradelr.Library.Constants;
using clearpixels.Logging;
using tradelr.Models.error;

namespace tradelr.Controllers.error
{
    public sealed class errorController : Controller
    {
        public ActionResult TrialExpired()
        {
            return View();
        }

        public ActionResult Index(string msg, string redirect)
        {
            msg = HttpUtility.UrlDecode(msg);
            var viewModel = new ErrorViewModel();
            if (!string.IsNullOrEmpty(redirect))
            {
                // try to get subdomain from session
                viewModel.redirectUrl = HttpUtility.UrlDecode(redirect);
            }

            if (string.IsNullOrEmpty(msg))
            {
                viewModel.message = "We are currently looking into this. Please try again later or <a href=\"mailto:support@tradelr.com\">contact us</a> if the error does not go away.";
            }
            else
            {
                viewModel.message = msg;
            }

            // manifest file will fail if not commented out
            Response.StatusCode = (int) HttpStatusCode.InternalServerError;

            if (Request.IsAjaxRequest())
            {
                return Content("Internal Error");
            }
            
            return View(viewModel);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Log(string message)
        {
            Syslog.Write("JAVASCRIPT: " + HttpUtility.UrlDecode(message));
            return new EmptyResult();
        }

        public ActionResult NoAuth()
        {
            // this was causing the exception cannot modify header after header has been sent error
            Response.StatusCode = (int) HttpStatusCode.Unauthorized;

            if (Request.IsAjaxRequest())
            {
                return Content("Not authorised");
            }
            
            
            return View();
        }

        public ActionResult NotFound()
        {
            // manifest file will fail if not commented out
            Response.StatusCode = (int)HttpStatusCode.NotFound;

            if (Request.IsAjaxRequest())
            {
                return Content("Not found");
            }
            

            return View();
        }

        public ActionResult NoPermission()
        {
            if (Request.IsAjaxRequest())
            {
                return Content("NoPermission");
            }
            

            return View();
        }

        public ActionResult Offline()
        {
            if (Request.IsAjaxRequest())
            {
                return Content("Offline");
            }
            
            return View();
        }

        public ActionResult StoreDisabled()
        {
            if (Request.IsAjaxRequest())
            {
                return Content("Private store");
            }
            
            return View();
        }
    }
}