using System.Web.Mvc;

namespace tradelr.Areas.checkout
{
    public class checkoutAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "checkout";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {

            context.MapRoute(
                "checkout_createorder_json",
                "checkout/order/{cartid}/create.json",
                new { controller = "order", action = "create", isJson = true },
                new[] { "tradelr.Areas.checkout.Controllers" }
            );

            context.MapRoute(
                "checkout_cart",
                "checkout/{controller}/{cartid}/{action}",
                new { Action = "Index"},
                new[] { "tradelr.Areas.checkout.Controllers" }
            );

        }
    }
}
