using System.Web.Mvc;

namespace tradelr.Areas.dashboard
{
    public class dashboardAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "dashboard";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "dashboard no action",
                "dashboard",
                new { Controller = "dashboard", action = "Index" },
                new[] { "tradelr.Areas.dashboard.Controllers" }
                );

            context.MapRoute(
                "Contacts",                                              // Route name
                "dashboard/contacts/{id}/{domainid}",                           // URL with parameters
                new { controller = "Contacts", action = "Show", domainid = UrlParameter.Optional },
                new { id = @"\d+" },
                new[] { "tradelr.Areas.dashboard.Controllers" }
            );

            context.MapRoute(
                "Sales Invoice",                                              // Route name
                "dashboard/invoices/{id}",                           // URL with parameters
                new { controller = "Invoices", action = "View" },
                new { id = @"\d+" },
                new[] { "tradelr.Areas.dashboard.Controllers" }
            );

            context.MapRoute(
                "Pages",                                              // Route name
                "dashboard/pages/{id}",                           // URL with parameters
                new { controller = "Pages", action = "View" },
                new { id = @"\d+" },
                new[] { "tradelr.Areas.dashboard.Controllers" }
            );

            context.MapRoute(
                "Purchase Order",                                              // Route name
                "dashboard/orders/{id}",                           // URL with parameters
                new { controller = "Orders", action = "View" },
                new { id = @"\d+" },
                new[] { "tradelr.Areas.dashboard.Controllers" }
            );

            context.MapRoute(
                "Products Dashboard",                                              // Route name
                "dashboard/product/{action}/{id}",                           // URL with parameters
                new { controller = "Product"},
                new { id = @"\d+" },
                new[] { "tradelr.Areas.dashboard.Controllers" }
            );

            context.MapRoute(
                "REST Actions",                                              // Route name
                "dashboard/{controller}/{id}/{action}",                           // URL with parameters
                new { action = "List" },
                new { id = @"\d+" },
                new[] { "tradelr.Areas.dashboard.Controllers" }
            );

            context.MapRoute(
                "dashboard_default",
                "dashboard/{controller}/{action}/{id}",
                new { controller = "dashboard", action = "Index", id = UrlParameter.Optional },
                new[] { "tradelr.Areas.dashboard.Controllers" }
                );
        }
    }
}
