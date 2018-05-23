using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;
using tradelr.Common.Constants;
using tradelr.Common.Library.Imaging;
using tradelr.Common.Models.currency;
using tradelr.Common.Models.photos;
using tradelr.DBML;
using tradelr.Libraries.Imaging;
using tradelr.Library.Constants;
using tradelr.Models.liquid.models;
using tradelr.Models.liquid.models.Product;
using tradelr.Models.products;
using tradelr.Models.transactions;

namespace tradelr.Models.store
{
    public class CheckoutItem : OrderItem
    {
        public string productUrl { get; set; }
        public string thumbnailUrl { get; set; }
    }

    public static class CheckoutItemHelper
    {
        public static CheckoutItem ToCheckoutItem(this product_variant p, int quantity, long? viewerid)
        {
            var unitprice = p.product.ToUserPrice(viewerid);
            var item = new CheckoutItem()
                       {
                           id = p.id,
                           productUrl = p.product.ToProductUrl(),
                           quantity = quantity,
                           UnitPrice = unitprice.Value,
                           UnitPriceWithTax = p.product.tax.HasValue
                                              ? (unitprice.Value * (p.product.tax.Value / 100 + 1))
                                              : unitprice.Value,
                           SKU = p.sku,
                           thumbnailUrl =
                               p.product.thumb.HasValue
                                   ? p.product.product_image.ToModel(Imgsize.THUMB).url
                                   : GeneralConstants.PHOTO_NO_THUMBNAIL,
                           description = p.ToProductFullTitle()
                       };

            if (!string.IsNullOrEmpty(p.product.dimensions))
            {
                var serializer = new JavaScriptSerializer();
                item.dimension = serializer.Deserialize<Dimension>(p.product.dimensions);
            }
            return item;
        }
    }
}