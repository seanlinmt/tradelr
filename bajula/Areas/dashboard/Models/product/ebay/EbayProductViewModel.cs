using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Ebay;
using Ebay.Enums;
using Ebay.Resources;
using eBay.Service.Core.Soap;
using tradelr.Common.Models.currency;
using tradelr.DBML;
using tradelr.Libraries.Helpers;
using tradelr.Library;
using tradelr.Library.Constants;
using clearpixels.Logging;

namespace tradelr.Areas.dashboard.Models.product.ebay
{
    public class EbayProductViewModel
    {
        // http://developer.ebay.com/devzone/xml/docs/Reference/eBay/types/ListingDurationCodeType.html

        public static readonly Dictionary<string, string> DurationNames = new Dictionary<string, string>()
                                                                               {
                                                                                   {"Days_1", "1 Day"},
                                                                                   {"Days_3", "3 Days"},
                                                                                   {"Days_5", "5 Days"},
                                                                                   {"Days_7", "7 Days"},
                                                                                   {"Days_10", "10 Days"},
                                                                                   {"Days_14", "14 Days"},
                                                                                   {"Days_21", "21 Days"},
                                                                                   {"Days_30", "30 Days"},
                                                                                   {"Days_60", "60 Days"},
                                                                                   {"Days_90", "90 Days"},
                                                                                   {"Days_120", "120 Days"},
                                                                                   {"GTC", "Auto Relist every 30 days"}
                                                                               };

        public bool includeAddress { get; set; }
        public bool autorelist { get; set; }
        
        // has already been posted. Used to detect if we're updating and posting a new entry
        public bool isPosted { get; set; } 
        public bool isActive { get; set; }

        public List<IEnumerable<SelectListItem>> categories { get; set; }
        public IEnumerable<SelectListItem> shippingProfiles { get; set; }
        public IEnumerable<SelectListItem> conditions { get; set; }
        public IEnumerable<SelectListItem> durations { get; set; }
        public IEnumerable<SelectListItem> dispatchTimes { get; set; }
        public IEnumerable<SelectListItem> TypeList { get; set; } 

        public int quantity { get; set; }

        private readonly ebay_product ebayproduct;
        private readonly SiteCodeType siteid;
        private IQueryable<ebay_category> sitecategories;
        private Currency currency;

        public string ListingID { get; set; }

        public string StartPrice { get; set; }
        public string BuynowPrice { get; set; }
        public string ReservePrice { get; set; }

        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string ViewLocation { get; set; }
        public string ListingFees { get; set; }

        public EbayProductViewModel(ebay_product _ebayproduct, MASTERsubdomain sd, tradelrDataContext db)
        {
            categories = new List<IEnumerable<SelectListItem>>();
            sitecategories = db.ebay_categories.Where(x => x.siteid == siteid.ToString());
            currency = sd.currency.ToCurrency();

            if (_ebayproduct == null)
            {
                // new product
                this.ebayproduct = new ebay_product();
                siteid = SiteCodeType.US;
                includeAddress = true;
                quantity = 1;
                conditions = Enumerable.Empty<SelectListItem>();
                durations = Enumerable.Empty<SelectListItem>();
                TypeList = typeof(ListingType).ToSelectList(true, null, null, ListingType.FixedPriceItem.ToString());
            }
            else
            {
                // existing product
                ebayproduct = _ebayproduct;
                siteid = ebayproduct.siteid.ToEnum<SiteCodeType>();
                includeAddress = ebayproduct.includeAddress;
                isPosted = true;
                isActive = ebayproduct.isActive;
                quantity = ebayproduct.quantity;

                var leafcategory = sitecategories.Single(x => x.categoryid == ebayproduct.categoryid);

                durations = leafcategory.ebay_listingdurations.Where(x => x.listingtypeid == ebayproduct.listingType)
                    .Select(x => new SelectListItem()
                                     {
                                         Text = DurationNames.ContainsKey(x.duration)
                                                 ? DurationNames[x.duration]
                                                 : x.duration,
                                         Value = x.duration,
                                         Selected = x.duration == ebayproduct.duration
                                     });
                conditions = leafcategory.ebay_conditions.Select(x => new SelectListItem()
                                                                          {
                                                                              Text = x.name,
                                                                              Value = x.value.ToString(),
                                                                              Selected =
                                                                                  x.value == ebayproduct.condition
                                                                         });

                ListingID = ebayproduct.ebayid;

                // intervals
                if (ebayproduct.startTime.HasValue)
                {
                    StartDate = ebayproduct.startTime.Value.ToString(GeneralConstants.DATEFORMAT_INVOICE);
                }

                if (ebayproduct.endTime.HasValue)
                {
                    EndDate = ebayproduct.endTime.Value.ToString(GeneralConstants.DATEFORMAT_INVOICE);
                }

                // prices
                if (ebayproduct.startPrice.HasValue)
                {
                    StartPrice = ebayproduct.startPrice.Value.ToString("n" + currency.decimalCount);
                }
                if (ebayproduct.buynowPrice.HasValue)
                {
                    BuynowPrice = ebayproduct.buynowPrice.Value.ToString("n" + currency.decimalCount);
                }
                if (ebayproduct.reservePrice.HasValue)
                {
                    ReservePrice = ebayproduct.reservePrice.Value.ToString("n" + currency.decimalCount);
                }

                ViewLocation = ebayproduct.ToExternalLink();
                ListingFees = ebayproduct.listingFees;

                TypeList = typeof (ListingType)
                                .ToSelectList(true, null, null,
                                        Listing.GetTradelrSupportedType(ebayproduct.listingType.ToEnum<ListingTypeCodeType>()).ToString());
            }

            dispatchTimes = db.ebay_dispatchtimes
                .Where(x => x.siteid == siteid.ToString())
                .OrderBy(x => x.dispatchTime)
                .Select(x => new SelectListItem()
                                 {
                                     Text = x.name,
                                     Value = x.dispatchTime.ToString(),
                                     Selected = x.dispatchTime == ebayproduct.dispatchTime
                                 });

            shippingProfiles = sd.ebay_shippingprofiles
                .Where(x => x.siteid == siteid.ToString())
                .Select(x => new SelectListItem()
                                 {
                                     Text = x.title,
                                     Value = x.id.ToString(),
                                     Selected = x.id == ebayproduct.profileid
                                 });
        }

        

        public IEnumerable<SelectListItem> GetSites()
        {
            return EbayService.SupportedSites.Select(
                x => new SelectListItem()
                         {
                             Text = x.ToString(), 
                             Value = x.ToString(), 
                             Selected = x == siteid
                         });
        }

        public IEnumerable<SelectListItem> GetRefundPolicy()
        {
            switch (siteid)
            {
                case SiteCodeType.Australia:
                case SiteCodeType.UK:
                case SiteCodeType.Singapore:
                case SiteCodeType.US:
                case SiteCodeType.Malaysia:
                    return new[]
                               {
                                   new SelectListItem()
                                       {
                                           Value = "Exchange",
                                           Text = "Exchange",
                                           Selected = "Exchange" == ebayproduct.refundPolicy
                                       },
                                   new SelectListItem()
                                       {
                                           Value = "MerchandiseCredit",
                                           Text = "Merchandise Credit",
                                           Selected = "MerchandiseCredit" == ebayproduct.refundPolicy
                                       },
                                   new SelectListItem()
                                       {
                                           Value = "MoneyBack",
                                           Text = "Money Back",
                                           Selected = "MoneyBack" == ebayproduct.refundPolicy
                                       }
                               };
                default:
                    throw new ArgumentOutOfRangeException("site");
            }
        }


        public IEnumerable<SelectListItem> GetReturnPolicy()
        {
            switch (siteid)
            {
                case SiteCodeType.Australia:
                case SiteCodeType.UK:
                case SiteCodeType.Singapore:
                case SiteCodeType.US:
                case SiteCodeType.Malaysia:
                    return typeof (ReturnPolicy).ToSelectList(true, null, null, ebayproduct.returnPolicy);
                default:
                    throw new ArgumentOutOfRangeException("site");
            }
        }

        public IEnumerable<SelectListItem> GetReturnWithin()
        {
            switch (siteid)
            {
                case SiteCodeType.Australia:
                case SiteCodeType.UK:
                case SiteCodeType.Singapore:
                case SiteCodeType.US:
                case SiteCodeType.Malaysia:
                    return new[]
                               {
                                   new SelectListItem()
                                       {
                                           Value = "Days_60",
                                           Text = "60 Days",
                                           Selected = "Days_60" == ebayproduct.returnWithin
                                       },
                                   new SelectListItem()
                                       {
                                           Value = "Days_30",
                                           Text = "30 Days",
                                           Selected = "Days_30" == ebayproduct.returnWithin
                                       },
                                   new SelectListItem()
                                       {
                                           Value = "Days_14",
                                           Text = "14 Days",
                                           Selected = "Days_14" == ebayproduct.returnWithin
                                       },
                                   new SelectListItem()
                                       {
                                           Value = "Days_7",
                                           Text = "7 Days",
                                           Selected = "Days_7" == ebayproduct.returnWithin
                                       },
                                   new SelectListItem()
                                       {
                                           Value = "Days_3",
                                           Text = "3 Days",
                                           Selected = "Days_3" == ebayproduct.returnWithin
                                       }
                               };
                default:
                    throw new ArgumentOutOfRangeException("site");
            }
        }

        public void PopulateCategories(int? categoryid = null)
        {
            if (categoryid.HasValue)
            {
                // work backwards
                bool completed = false;
                var nextleaf = sitecategories.Single(x => x.categoryid == categoryid.Value);
                while (!completed)
                {
                    var leaf = nextleaf;

                    if (leaf.categoryid == leaf.parentid)
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
                                                     Value = x.categoryid.ToString(),
                                                     Selected = x.categoryid == leaf.categoryid
                                                 })
                        );

                    nextleaf = sitecategories.SingleOrDefault(x => x.categoryid == leaf.parentid);
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
                            .Where(x => x.level == 1)
                            .OrderBy(x => x.name)
                            .Select(x => new SelectListItem() {Text = x.name, Value = x.categoryid.ToString()})
                    );
                categories.Add(root);
            }
        }
    }
}