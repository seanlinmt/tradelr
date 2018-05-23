using System.Web.Mvc;
using tradelr.Areas.checkout.Models;
using tradelr.Libraries.ActionFilters;

namespace tradelr.Areas.checkout.Controllers
{
    [TradelrHttps]
    public class cartController : baseCartController
    {
        public ActionResult Index()
        {
            var viewmodel = cart.ToViewModel(baseviewmodel, sessionid);

            return View(viewmodel);
        }

#if DEBUG
        // unbinds user from cart
        public ActionResult logout()
        {
            cart.user = null;
            repository.Save();
            return RedirectToAction("Index");
        }
#endif
    }
}
