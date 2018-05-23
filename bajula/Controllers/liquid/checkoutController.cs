using System.Web.Mvc;
using tradelr.Libraries.ActionFilters;
using tradelr.Library.Constants;

namespace tradelr.Controllers.liquid
{
    public class checkoutController : baseController
    {
        public ActionResult Index()
        {
            // checkout
            var cartid = GetCartIdFromCookie();
            if (string.IsNullOrEmpty(cartid))
            {
                return RedirectToAction("Index", "Error");
            }
            return Redirect(string.Format("{0}/checkout/cart/{1}", GeneralConstants.HTTP_SECURE, cartid));
        }

    }
}
