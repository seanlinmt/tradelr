using System.Collections.Generic;
using System.Linq;
using tradelr.Common;
using tradelr.Common.Constants;
using tradelr.Common.Library.Imaging;
using tradelr.Common.Models.currency;
using tradelr.Common.Models.photos;
using tradelr.DBML;

namespace tradelr.OpenSocial.Models
{
    public class Product
    {
        public string currencySymbol { get; set; }
        public string sellingPrice { get; set; }
        public string title { get; set; }
        public string summary { get; set; }
        public string url { get; set; }
        public string thumbnailUrl { get; set; }
    }

    public static class ProductHelper
    {
        public static IEnumerable<Product> ToModel(this IQueryable<product> values, string subdomain)
        {
            foreach (var p in values)
            {
                yield return new Product()
                                 {
                                     currencySymbol = p.user.organisation1.currency.ToCurrencySymbol(),
                                     title = p.title,
                                     url =
                                         string.Concat("http://", subdomain, ".tradelr.com",
                                                       GeneralConstants.URL_SINGLE_PRODUCT_SHOW, p.id, "/", p.title.ToSafeUrl()),
                                     sellingPrice =
                                         p.sellingPrice.HasValue
                                             ? p.sellingPrice.Value.ToString("n")
                                             : "",
                                     summary = p.details.Substring(0, p.details.Length < 100 ? p.details.Length: 100),
                                     thumbnailUrl =
                                         p.thumbnail.HasValue
                                             ? string.Concat("http://", subdomain, ".tradelr.com", p.image.ToModel(Imgsize.GALLERY).url)
                                             : string.Concat("http://", subdomain, ".tradelr.com", GeneralConstants.PHOTO_NO_THUMBNAIL_MEDIUM)
                                 };
            }
        }
    }
}