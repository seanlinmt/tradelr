using System.Net;
using System.Web.Mvc;
using tradelr.Libraries.ActionFilters;

namespace tradelr.Controllers.liquid
{
    [TradelrHttp]
    public class liquidErrorController : baseController
    {
        public ActionResult NotFound()
        {
            var template = CreateLiquidTemplate("404", "Page not found");
            template.InitContentTemplate("templates/404.liquid");

            Response.StatusCode = (int)HttpStatusCode.NotFound;

            return Content(template.Render());
        }

    }
}
