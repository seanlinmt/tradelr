using System.Linq;
using System.Web.Mvc;
using tradelr.DBML;
using tradelr.Libraries.ActionFilters;
using tradelr.Library;
using tradelr.Models.facebook;
using tradelr.Models.liquid;
using tradelr.Models.liquid.models.Product;

namespace tradelr.Controllers.liquid
{
    public class productsController : baseController
    {
        public ActionResult Single(long? id, string title, bool isJson = false)
        {
            if (!id.HasValue)
            {
                return Content(CreatePageMissingTemplate().Render());
            }
            // if store disabled then hide the product
            if (!IsStoreEnabled && !sessionid.HasValue)
            {
                return RedirectToAction("Index", "login");
            }

            LiquidTemplate template;
            using (var repo = new TradelrRepository())
            {
                repo.SetIsolationToNoLock();
                var p = repo.GetProduct(id.Value, subdomainid.Value);

                if (p == null)
                {
                    return Content(CreatePageMissingTemplate().Render());
                }
                var liquidmodel = p.ToLiquidModel(sessionid, "");

                if (isJson)
                {
                    return Json(liquidmodel, JsonRequestBehavior.AllowGet);
                }

                if (p.hits.HasValue)
                {
                    p.hits += 1;
                }
                else
                {
                    p.hits = 1;
                }
                repo.Save();

                template = CreateLiquidTemplate("product", p.title);

                // opengraph
                var opengraph = MASTERdomain.organisation.ToOpenGraph(p, null);
                template.AddHeaderContent(this.RenderViewToString("~/Views/store/liquid/defaultHeader.ascx", opengraph));
                template.InitContentTemplate("templates/product.liquid");
                template.AddParameters("product", liquidmodel);
            }
            return Content(template.Render());
        }

    }
}
