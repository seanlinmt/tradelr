using System.Web.Mvc;

namespace tradelr.Areas.help
{
    public class helpAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "help";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "help_default",
                "help/{controller}/{action}/{id}",
                new { action = "Index", controller = "Default", id = UrlParameter.Optional },
                new[] { "tradelr.Areas.help.Controllers" }
            );
        }
    }
}
