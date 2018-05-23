using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using tradelr.Common.Library.Imaging;
using tradelr.Common.Models.currency;
using tradelr.DBML;
using tradelr.Libraries.Imaging;
using tradelr.Library.Constants;

namespace tradelr.Models.products
{
    public class ProductBase
    {
        public long? id { get; set; }
        public string sellingPrice { get; set; }
        public string specialPrice { get; set; }
        public string tax { get; set; }
        public string thumbnailUrl { get; set; }
        public string title { get; set; }
        public List<Variant> variants { get; set; }

        public ProductBase()
        {
            variants = new List<Variant>();
        }
    }

    public static class ProductBaseHelper
    {
        /// <summary>
        /// this creates object for serialization to invoice items for client-side
        /// </summary>
        /// <param name="variants"></param>
        /// <returns></returns>
        public static IEnumerable<ProductBase> ToBaseModel(this IEnumerable<product> values)
        {
            var products = new List<ProductBase>();
            Currency currency = null;
            foreach (var value in values)
            {
                if (currency == null)
                {
                    currency = value.MASTERsubdomain.currency.ToCurrency();
                }
                var p = new ProductBase();
                p.id = value.id;
                p.sellingPrice = value.sellingPrice.HasValue
                                     ? currency.symbol + (value.specialPrice ?? value.sellingPrice).Value.ToString("n" +
                                                                                                 currency.decimalCount)
                                     : "";
                p.title = value.title;
                p.tax = value.tax.HasValue ? value.tax.Value + "%" : "";
                p.thumbnailUrl = value.thumb.HasValue
                                     ? Img.by_size(value.product_image.url, Imgsize.THUMB).ToHtmlImage()
                                     : GeneralConstants.PHOTO_NO_THUMBNAIL.ToHtmlImage();
                foreach (var variant in value.product_variants)
                {
                    var v = new Variant
                                {
                                    id = variant.id,
                                    color = variant.color,
                                    size = variant.size,
                                    sku = variant.sku,
                                    instock = variant.inventoryLocationItems.All(x => x.available == null)
                                                  ? null
                                                  : variant.inventoryLocationItems.Sum(x => x.available)
                                };
                    p.variants.Add(v);
                }

                products.Add(p);
            }

            return products;
        }

    }
}
