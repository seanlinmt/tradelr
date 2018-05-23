using System.Collections.Generic;
using System.Linq;
using tradelr.DBML;
using tradelr.Library;
using tradelr.Models.coupons;
using tradelr.Models.shipping;
using tradelr.Models.store;
using tradelr.Common.Models.currency;
using tradelr.Models.subdomain;

namespace tradelr.Areas.checkout.Models
{
    public class CheckoutViewModel : BaseViewModel
    {
        public CheckoutViewModel(BaseViewModel baseviewmodel) : base(baseviewmodel)
        {
            CartItems = new List<CheckoutItem>();
        }

        public CheckoutViewModel()
        {
            CartItems = new List<CheckoutItem>(); 
        }

        public bool isDigitalOrder { get; set; }
        public bool isLoggedIn { get; set; }
        public long subdomainid { get; set; }

        public string store_url { get; set; }

        public Currency currency { get; set; }
        public List<CheckoutItem> CartItems { get; set; }
        public decimal sub_total { get; set; }
        public string coupon { get; set; }
        public decimal discount_amount { get; set; }
        public decimal total { get; set; }
    }

    public static class CheckoutViewModelHelper
    {
        public static CheckoutViewModel ToViewModel(this cart c, BaseViewModel baseviewmodel, long? viewerid)
        {
            var model = new CheckoutViewModel(baseviewmodel);
            model.subdomainid = c.subdomainid;
            model.store_url = c.MASTERsubdomain.ToHostName().ToDomainUrl();

            foreach (var entry in c.cartitems) 
            {
                var citem = entry.product_variant.ToCheckoutItem(entry.quantity, viewerid);
                model.CartItems.Add(citem);
            }

            model.sub_total = model.CartItems.Sum(x => x.subTotal);
            model.coupon = c.coupon;
            model.discount_amount = c.ToDiscountAmount(model.sub_total);
            model.total = model.sub_total - model.discount_amount;
            model.currency = c.MASTERsubdomain.currency.ToCurrency();

            model.isLoggedIn = viewerid.HasValue;
            model.isDigitalOrder = c.isDigitalOrder();

            return model;
        }
    }

}