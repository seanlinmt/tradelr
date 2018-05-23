using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using tradelr.Common;
using tradelr.Common.Constants;
using tradelr.Common.Library.Imaging;
using tradelr.Common.Models.currency;
using tradelr.Common.Models.photos;
using tradelr.DBML;
using tradelr.Library;
using tradelr.Library.Constants;

namespace tradelr.Facebook.Models.facebook
{
    public class Product
    {
        public string currencySymbol { get; set; }
        public string sellingPrice { get; set; }
        public string title { get; set; }
        public string url { get; set; }
        public string thumbnailUrl { get; set; }
    }

    public static class ProductHelper
    {
        public static IEnumerable<Product> ToModel(this IQueryable<product> values, string subdomain)
        {
            foreach (var p in values)
            {
                var currency = p.MASTERsubdomain.currency.ToCurrency();
                yield return new Product()
                                 {
                                     currencySymbol = currency.symbol,
                                     title = p.title,
                                     url =
                                         string.Concat("http://", subdomain, ".tradelr.com",
                                                       GeneralConstants.URL_SINGLE_PRODUCT_SHOW, p.id, "/",
                                                       p.title.ToSafeUrl()),
                                     sellingPrice =
                                         p.specialPrice.HasValue
                                             ? (p.tax.HasValue
                                                    ? (p.specialPrice.Value*(p.tax.Value/100 + 1)).ToString("n" +
                                                                                                            currency.
                                                                                                                decimalCount)
                                                    : p.specialPrice.Value.ToString("n" + currency.decimalCount))
                                             : p.sellingPrice.HasValue
                                                   ? (p.tax.HasValue
                                                          ? (p.sellingPrice.Value*(p.tax.Value/100 + 1)).ToString("n" +
                                                                                                                  currency
                                                                                                                      .
                                                                                                                      decimalCount)
                                                          : p.sellingPrice.Value.ToString("n" + currency.decimalCount))
                                                   : "",
                                     thumbnailUrl =
                                         p.thumb.HasValue
                                             ? string.Concat("http://", subdomain, ".tradelr.com",
                                                             p.product_image.ToModel(Imgsize.COMPACT).url)
                                             : string.Concat("http://", subdomain, ".tradelr.com",
                                                             GeneralConstants.PHOTO_NO_THUMBNAIL_MEDIUM)
                                 };
            }
        }
    }
}