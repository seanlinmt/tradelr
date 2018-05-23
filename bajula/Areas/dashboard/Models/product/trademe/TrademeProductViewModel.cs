using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using api.trademe.co.nz.v1;
using tradelr.Common.Models.currency;
using tradelr.DBML;
using tradelr.Library;
using tradelr.Library.Constants;

namespace tradelr.Areas.dashboard.Models.product.trademe
{
    public class TrademeProductViewModel
    {
        // what needs to be displayed?
        public int ListingID;
        public int quantity;
        public string StartPrice;
        public string BuynowPrice;
        public string ReservePrice;
        public string ViewLocation;
        public string ListingFees;

        public string StartDate;
        public string EndDate;

        public List<IEnumerable<SelectListItem>> categories;
        public IEnumerable<SelectListItem> durations;

        // shipping
        public IEnumerable<TrademeShippingCost> shippingCosts; 

        // listing status
        public bool isPosted;
        public bool isActive;

        // options
        public bool isBrandNew;
        public bool onlyAuthenticated;
        public bool isBold;
        public bool isFeatured;
        public bool isHomepageFeatured;
        public bool hasGallery;
        public bool autorelist;
        public bool safetrader;
        public bool freeShipping;

        private Currency currency;
        private IQueryable<trademe_category> sitecategories;
        private trademe_product trademeproduct;

        public TrademeProductViewModel(trademe_product _trademeproduct, MASTERsubdomain sd, tradelrDataContext db)
        {
            categories = new List<IEnumerable<SelectListItem>>();
            sitecategories = db.trademe_categories;
            currency = sd.currency.ToCurrency();

            if (_trademeproduct == null)
            {
                // new product
                trademeproduct = new trademe_product();
                quantity = 1;
                durations = Enumerable.Empty<SelectListItem>();
                shippingCosts = Enumerable.Empty<TrademeShippingCost>();
                freeShipping = true;
            }
            else
            {
                // existing product
                trademeproduct = _trademeproduct;
                isPosted = true;
                isActive = trademeproduct.isActive;
                isBrandNew = trademeproduct.isnew;
                onlyAuthenticated = trademeproduct.authenticatedOnly;
                isBold = trademeproduct.isBold;
                isFeatured = trademeproduct.isFeatured;
                isHomepageFeatured = trademeproduct.isHomepageFeatured;
                hasGallery = trademeproduct.hasGallery;

                // shipping costs
                shippingCosts = trademeproduct.trademe_shippingcosts.ToModel();
                freeShipping = !shippingCosts.Any();

                quantity = trademeproduct.quantity;

                var leafcategory = sitecategories.Single(x => x.id == trademeproduct.categoryid);

                durations = leafcategory.trademe_listingdurations.Where(x => x.categoryid == trademeproduct.categoryid)
                    .Select(x => new SelectListItem()
                                     {
                                         Text = x.duration == 1? (x.duration + " day"):( x.duration + " days"),
                                         Value = x.duration.ToString(),
                                         Selected = x.duration == trademeproduct.duration
                                     });

                ListingID = trademeproduct.listingid;

                // intervals
                if (trademeproduct.startTime.HasValue)
                {
                    StartDate = trademeproduct.startTime.Value.ToString(GeneralConstants.DATEFORMAT_INVOICE);
                }

                if (trademeproduct.endTime.HasValue)
                {
                    EndDate = trademeproduct.endTime.Value.ToString(GeneralConstants.DATEFORMAT_INVOICE);
                }

                // prices
                StartPrice = trademeproduct.startPrice.ToString("n" + currency.decimalCount);
                
                if (trademeproduct.buynowPrice.HasValue)
                {
                    BuynowPrice = trademeproduct.buynowPrice.Value.ToString("n" + currency.decimalCount);
                }

                ReservePrice = trademeproduct.reservePrice.ToString("n" + currency.decimalCount);

                ViewLocation = trademeproduct.ToExternalLink();
                ListingFees = trademeproduct.listingfees.HasValue? trademeproduct.listingfees.Value.ToString(): "";
            }
        }

        public void PopulateCategories(string categoryid)
        {
            if (!string.IsNullOrEmpty(categoryid))
            {
                // work backwards
                bool completed = false;
                var nextleaf = sitecategories.Single(x => x.id == categoryid);
                while (!completed)
                {
                    var leaf = nextleaf;

                    if (leaf.id == leaf.parentid)
                    {
                        completed = true;
                    }

                    var cats = new[] { new SelectListItem() { Text = "select ...", Value = "" } }
                        .Union(
                            sitecategories
                                .Where(x => x.parentid == leaf.parentid)
                                .OrderBy(x => x.name)
                                .Select(x => new SelectListItem()
                                                 {
                                                     Text = x.name, 
                                                     Value = x.id.ToString(),
                                                     Selected = x.id == leaf.id
                                                 })
                        );

                    nextleaf = sitecategories.SingleOrDefault(x => x.id == leaf.parentid);
                    categories.Add(cats);

                    if (nextleaf == null)
                    {
                        completed = true;
                    }
                }
            }
            else
            {
                var root = new[] {new SelectListItem() {Text = "select ...", Value = ""}}
                    .Union(
                        sitecategories
                            .Where(x => x.id == x.parentid)
                            .OrderBy(x => x.name)
                            .Select(x => new SelectListItem() {Text = x.name, Value = x.id.ToString()})
                    );
                categories.Add(root);
            }
        }

        public IEnumerable<SelectListItem> GetPickups()
        {
            return new[]
                       {
                           new SelectListItem()
                               {
                                   Value = PickupOption.Allow.ToInt().ToString(),
                                   Text = "Buyer can pick-up",
                                   Selected = PickupOption.Allow.ToInt() == trademeproduct.pickup
                               },
                           new SelectListItem()
                               {
                                   Value = PickupOption.Demand.ToInt().ToString(),
                                   Text = "Buyer must pick-up",
                                   Selected = PickupOption.Demand.ToInt() == trademeproduct.pickup
                               },
                           new SelectListItem()
                               {
                                   Value = PickupOption.Forbid.ToInt().ToString(),
                                   Text = "No pick-ups",
                                   Selected = PickupOption.Forbid.ToInt() == trademeproduct.pickup
                               },

                       };
        }
    }
}