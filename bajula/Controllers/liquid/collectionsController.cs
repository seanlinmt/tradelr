using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using tradelr.DBML.Helper;
using tradelr.Libraries.ActionFilters;
using tradelr.Library;
using tradelr.Models.facebook;
using tradelr.Models.liquid.models;
using tradelr.Models.liquid.models.Product;

namespace tradelr.Controllers.liquid
{
    public class collectionsController : baseController
    {
        public ActionResult Index(int? page, string id, string tags)
        {
            if (string.IsNullOrEmpty(id))
            {
                id = "all";
            }

            var template = CreateLiquidTemplate("collection", "");
            // opengraph
            var opengraph = MASTERdomain.organisation.ToOpenGraph(null,null);
            template.AddHeaderContent(this.RenderViewToString("~/Views/store/liquid/defaultHeader.ascx", opengraph));
            
            template.InitContentTemplate("templates/collection.liquid");

            var collections = MASTERdomain.product_collections.AsQueryable();

            IEnumerable<string> taglist = null;
            if (!string.IsNullOrEmpty(tags))
            {
                taglist = tags.Split(new[] {'+'}, StringSplitOptions.RemoveEmptyEntries);
                template.AddParameters("current_tags", taglist);
            }

            if (id != "all")
            {
                var collection = collections.SingleOrDefault(x => string.Equals(x.permalink, id, StringComparison.InvariantCultureIgnoreCase));
                if (collection != null)
                {
                    template.AddParameters("collection",
                                       new Collection(collection, taglist, sessionid));
                    template.AddParameters("page_title", collection.name);
                    template.AddFilterValues("tag_current_handle", collection.permalink);
                }
            }
            else
            {
                template.AddParameters("collection", new Collection(collections, sessionid));
                template.AddParameters("page_title", "Collections");
                template.AddFilterValues("tag_current_handle", "all");
            }
                
            if (page.HasValue)
            {
                template.AddRegisters("current_page", page.Value);
            }

            return Content(template.Render());
        }

        public ActionResult Products(string collectionid, long productid)
        {
            var p = MASTERdomain.product_collections
                            .Where(x => string.Equals(x.permalink, collectionid, StringComparison.InvariantCultureIgnoreCase))
                            .SelectMany(x => x.productCollectionMembers)
                            .Where(y => y.productid == productid)
                            .Select(z => z.product)
                            .IsActive()
                            .SingleOrDefault();
            if (p == null)
            {
                return Content(CreatePageMissingTemplate().Render());
            }

            var collections = p.productCollectionMembers
                                .AsQueryable()
                                .Select(x => x.product_collection)
                                .Where(x => string.Equals(x.permalink, collectionid, StringComparison.InvariantCultureIgnoreCase));

            var collectionmodel = new Collection(collections,sessionid);
            
            var productmodel = collectionmodel.products.Single(x => x.id == p.id);

            var nextproducts = collectionmodel.products.AsQueryable().Where(x => x.id > p.id);
            var previousproducts = collectionmodel.products.AsQueryable().Where(x => x.id < p.id);

            if (collectionmodel.products.Count > 1)
            {
                collectionmodel.next_product =
                    (nextproducts.Count() != 0 ? nextproducts.First() : previousproducts.First())
                        .ToLiquidProductInCollectionUrl(collectionid);
                collectionmodel.previous_product =
                    (previousproducts.Count() != 0 ? previousproducts.Last() : nextproducts.Last())
                        .ToLiquidProductInCollectionUrl(collectionid);
            }

            var template = CreateLiquidTemplate("product", p.title);

            // opengraph
            var opengraph = MASTERdomain.organisation.ToOpenGraph(p, null);
            template.AddHeaderContent(this.RenderViewToString("~/Views/store/liquid/defaultHeader.ascx", opengraph));

            template.InitContentTemplate("templates/product.liquid");
            template.AddParameters("product", productmodel);
            template.AddParameters("collection", collectionmodel);
                        
            return Content(template.Render());
        }
    }
}
