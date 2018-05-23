using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using tradelr.Common.Library.Imaging;
using tradelr.Common.Models.currency;
using tradelr.DBML;
using tradelr.DBML.Helper;
using tradelr.Library;
using tradelr.Library.Constants;
using tradelr.Models.products;
using tradelr.Models.subdomain;

namespace tradelr.Models.facebook.app
{
    public class FacebookGalleryViewModel
    {
        public IEnumerable<SelectListItem> categories { get; set; }
        public IEnumerable<Product> products { get; set; }
        public string viewAllUrl { get; set; }
        public string canvasStoreUrl { get; set; }   // on facebook
        public string couponMessage { get; set; }
        public bool isOwner { get; set; }
        public string token { get; set; }
        public string hostname { get; set; }

        public FacebookGalleryViewModel()
        {
            products = Enumerable.Empty<Product>();
        }

        public void InitGalleryView(facebookPage fbpage, ITradelrRepository repository, bool liked)
        {
            hostname = string.Format("{0}.{1}", fbpage.MASTERsubdomain.name, GeneralConstants.SUBDOMAIN_HOST);
            viewAllUrl = hostname.ToDomainUrl();
#if DEBUG
            canvasStoreUrl = string.Format("http://apps.facebook.com/tradelrtest/store/{0}", fbpage.MASTERsubdomain.name);
#else
            canvasStoreUrl = string.Format("http://apps.facebook.com/tradelr/store/{0}", fbpage.MASTERsubdomain.name);
#endif
            categories = repository.GetProductCategories(null, fbpage.subdomainid)
                .Select(
                    x =>
                    new SelectListItem() {Text = x.MASTERproductCategory.name, Value = x.id.ToString()});
            if ((fbpage.MASTERsubdomain.flags & (int)SubdomainFlags.STORE_ENABLED) != 0)
            {
                products = repository.GetProducts(fbpage.subdomainid)
                            .IsActive()
                            .OrderByDescending(x => x.id)
                            .Take(21)
                            .ToFacebookModel(hostname);
            }

            // handle coupon message
            if (fbpage.MASTERsubdomain.facebookCoupon.HasValue)
            {
                if (liked)
                {
                    var coupon = fbpage.MASTERsubdomain.coupon;
                    var currency = fbpage.MASTERsubdomain.currency.ToCurrency();
                    string description;
                    string minimumPurchase = " on all purchases.";
                    if (coupon.minimumPurchase.HasValue)
                    {
                        minimumPurchase = string.Format(" with a minimum purchase of {0}{1}.", currency.code,
                                                        coupon.minimumPurchase.Value.ToString("n" +
                                                                                              currency.decimalCount));
                    }
                    if (coupon.couponPercentage.HasValue)
                    {
                        description = string.Format("{0}% discount", coupon.couponPercentage.Value.ToString("n2"));
                    }
                    else
                    {
                        description = string.Format("{0}{1} off", currency.code,
                                                    coupon.couponValue.Value.ToString("n" + currency.decimalCount));
                    }
                    couponMessage = string.Format(
                        "<p><strong>Discount code:</strong> {0}</p><p>{1}{2}</p>", coupon.code, description,
                        minimumPurchase);
                }
                else
                {
                    couponMessage = string.Format("<p class='strong'><img src='{0}/Content/img/arrow_up.png' /> Click on the like button above to view the latest discount code.</p>",
                        GeneralConstants.FACEBOOK_HOST);
                }
            }
        }
    }
}