using System.Collections.Generic;
using System.Linq;
using tradelr.Common.Library.Imaging;
using tradelr.Common.Models.currency;
using tradelr.Common.Models.photos;
using tradelr.DBML;
using tradelr.Library;
using tradelr.Library.Constants;
using tradelr.Models.products;

namespace tradelr.Models.facebook
{
    public class Product
    {
        public string currencySymbol { get; set; }
        public string sellingPrice { get; set; }
        public string title { get; set; }
        public string url { get; set; }
        public long id { get; set; }
        public string thumbnailUrl { get; set; }
    }

    public static class ProductHelper
    {
        public static IEnumerable<Product> ToFacebookModel(this IEnumerable<product> values, string hostname)
        {
            foreach (var p in values)
            {
                var currency = p.MASTERsubdomain.currency.ToCurrency();
                yield return new Product()
                                 {
                                     currencySymbol = currency.symbol,
                                     title = p.title,
                                     id = p.id,
                                     url = hostname.ToDomainUrl(p.ToProductUrl()),
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
                                             ? hostname.ToDomainUrl(p.product_image.ToModel(Imgsize.COMPACT).url)
                                             : hostname.ToDomainUrl(GeneralConstants.PHOTO_NO_THUMBNAIL_MEDIUM)
                                 };
            }
        }
    }
}