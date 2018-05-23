using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using tradelr.Controllers;
using tradelr.DBML;
using tradelr.DBML.Lucene;
using tradelr.Libraries.ActionFilters;
using tradelr.Library.JSON;
using tradelr.Models.contacts.viewmodel;
using tradelr.Models.group;
using tradelr.Models.products;
using tradelr.Models.products.viewmodel;
using tradelr.Models.users;

namespace tradelr.Areas.dashboard.Controllers
{
    [PermissionFilter(permission = UserPermission.INVENTORY_VIEW)]
    [RoleFilter(role = UserRole.USER)]
    [TradelrHttps]
    public class groupController : baseController
    {
        public ActionResult Pricing()
        {
            var viewmodel = new ContactListViewModel(baseviewmodel);
            viewmodel.PopulateContactGroups(repository, subdomainid.Value);

            return View(viewmodel);
        }

        [HttpPost]
        public ActionResult PricingDelete(long id, string productids)
        {
            var group = repository.GetContactGroup(id, subdomainid.Value);
            if (group == null)
            {
                return SendJsonErrorResponse("Invalid contact group specified");
            }
            var products = productids.Split(new[] {','});
            var entriesToDelete = group.contactGroupPricings.Where(x => products.Contains(x.id.ToString()));
            try
            {
                repository.DeleteGroupPricings(entriesToDelete);
            }
            catch (Exception ex)
            {
                return SendJsonErrorResponse(ex);
            }
            return Json("Entries removed successfully".ToJsonOKMessage());
        }

        [HttpPost]
        [JsonFilter(Param = "groupPricing", RootType = typeof(GroupPricing))]
        public ActionResult PricingAdd(GroupPricing groupPricing)
        {
            var group = repository.GetContactGroup(groupPricing.groupid, subdomainid.Value);
            if (group == null)
            {
                return SendJsonErrorResponse("Invalid contact group specified");
            }
            
            foreach (var entry in groupPricing.prices)
            {
                var price = entry;
                // get contact group
                var gprice = new contactGroupPricing();
                gprice.productid = price.id;
                gprice.price = price.price;
                var exist = group.contactGroupPricings.Where(x => x.productid == price.id).SingleOrDefault();
                if (exist != null)
                {
                    exist.price = price.price;
                }
                else
                {
                    group.contactGroupPricings.Add(gprice);    
                }
            }

            repository.Save();

            return Json("Group prices added successfully".ToJsonOKMessage());
        }

        [HttpPost]
        public ActionResult PricingList(long? cat, int rows, int page)
        {
            var results = repository.GetGroupPricings(cat, subdomainid.Value);

            var records = results.Count();
            var total = (records / rows);
            if (records % rows != 0)
            {
                total++;
            }

            // return in the format required for jqgrid
            results = results.Skip(rows * (page - 1)).Take(rows);
            var pricingJqGrid = results.ToGroupPricingJqGrid();
            pricingJqGrid.page = page;
            pricingJqGrid.records = records;
            pricingJqGrid.total = total;

            return Json(pricingJqGrid);
        }

        [HttpGet]
        public ActionResult productPricing(string term, long? sinceid, long id)
        {
            var options = new VariantsContentOptions();
            options.sinceid = sinceid;
            options.term = term;
            options.groupid = id;
            return View(options);
        }

        public ActionResult productPricingContent(string term, long? sinceid)
        {
            IEnumerable<product> products = repository.GetProducts(subdomainid.Value);
            if (!string.IsNullOrEmpty(term))
            {
                var search = new LuceneSearch();
                var ids = search.ProductSearch(term, accountSubdomainName);
                products = products.Where(x => ids.Select(y => y.id).Contains(x.id.ToString())).AsEnumerable();
                products = products.Join(ids, x => x.id.ToString(), y => y.id, (x, y) => new { x, y.score })
                        .OrderByDescending(x => x.score).Select(x => x.x);
            }

            if (sinceid.HasValue)
            {
                products = products.Where(x => x.id > sinceid);
            }
            products = products.OrderBy(x => x.id).Take(20);
            var viewmodel = products.ToBaseModel();
            return View(viewmodel);
        }
    }
}
