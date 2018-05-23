using System.Linq;
using DotLiquid;
using tradelr.Common.Models.currency;
using tradelr.DBML;
using tradelr.DBML.Helper;
using tradelr.Library;
using tradelr.Models.products;
using tradelr.Models.subdomain;

namespace tradelr.Models.liquid.models
{
    public class Shop : Drop
    {
        public string name { get; set; }
        public string url { get; set; }
        public string domain { get; set; }
        public string permanent_domain { get; set; }
        public string email { get; set; }
        public int products_count { get; set; }
        public int collections_count { get; set; }

        public string currency { get; set; }
        public string currency_symbol { get; set; }
        public int currency_decimal_places { get; set; }

        // proprietary
        public string message { get; set; }
        public string payment_policy { get; set; }
        public string refund_policy { get; set; }
        public string fb_adminid { get; set; }
    }

    public static class ShopHelper
    {
        public static Shop ToLiquidModel(this MASTERsubdomain row)
        {
            var shop = new Shop
                           {
                               email = row.organisation.users.First().email,
                               name = row.storeName,
                               message = row.organisation.motd,
                               url = row.ToHostName().ToDomainUrl(),
                               permanent_domain = string.Format("{0}.tradelr.com", row.name),
                               products_count = row.products.AsQueryable().IsActive().Count(),
                               collections_count = row.product_collections.Where(x => (x.settings & (int)CollectionSettings.VISIBLE) != 0).Count(),
                               payment_policy = row.paymentTerms,
                               refund_policy = row.returnPolicy,
                               fb_adminid = row.organisation.users.First().FBID
                           };

            var currency = row.currency.ToCurrency();
            shop.currency = currency.code;
            shop.currency_symbol = currency.symbol;
            shop.currency_decimal_places = currency.decimalCount; 

            if (!string.IsNullOrEmpty(row.customDomain))
            {
                shop.domain = row.customDomain;
            }
            else
            {
                shop.domain = shop.permanent_domain;
            }
            
            return shop;
        }
    }

}