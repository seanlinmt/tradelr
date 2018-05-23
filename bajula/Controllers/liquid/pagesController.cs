using System;
using System.Linq;
using System.Web.Mvc;
using tradelr.Areas.dashboard.Models.store.page;
using tradelr.Libraries.ActionFilters;
using tradelr.Library;
using tradelr.Models.facebook;
using Page = tradelr.Models.liquid.models.Page;

namespace tradelr.Controllers.liquid
{
    public class pagesController : baseController
    {
        public ActionResult Index(string id)
        {
            var page = MASTERdomain.pages.SingleOrDefault(x => string.Equals(x.permalink, id, StringComparison.InvariantCultureIgnoreCase));
            if (page == null ||
                !((PageSettings)page.settings).HasFlag(PageSettings.VISIBLE))
            {
                return Content(CreatePageMissingTemplate().Render());
            }
            var templatename = "templates/page.liquid";
            if (!string.IsNullOrEmpty(page.templatename))
            {
                templatename = string.Format("templates/{0}", page.templatename);
            }

            var template = CreateLiquidTemplate("page", page.name);
            // opengraph
            var opengraph = MASTERdomain.organisation.ToOpenGraph(null,null);
            template.AddHeaderContent(this.RenderViewToString("~/Views/store/liquid/defaultHeader.ascx", opengraph));

            template.InitContentTemplate(templatename);
            template.AddParameters("page", new Page(page));

            
            return Content(template.Render());
        }

    }
}
