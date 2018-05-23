using System.Collections.Generic;
using System.Web.Script.Serialization;
using DotLiquid;
using tradelr.DBML;
using tradelr.Models.liquid.models.Product;
using tradelr.Models.products;
using Variant = tradelr.Models.liquid.models.Product.Variant;

namespace tradelr.Models.liquid.models.Cart
{
    public class CartItem : Drop
    {
        public long id { get; set; }
        public string title { get; set; }
        [ScriptIgnore]
        public Product.Product product { get; set; }
        [ScriptIgnore]
        public Variant variant { get; set; }
        public long variant_id
        {
            get
            {
                return variant.id;
            }
        }
        public int quantity { get; set; }
        public decimal price { get; set; }
        public decimal total_price { get; set; }
        [ScriptIgnore]
        public decimal line_price
        {
            get
            {
                return total_price;
            }
        }
    }

    public static class CartItemHelper
    {
        public static IEnumerable<CartItem> ToLiquidModel(this IEnumerable<cartitem> rows, long? viewerid)
        {
            foreach (var row in rows)
            {
                yield return row.ToLiquidModel(viewerid);
            }
        }

        public static CartItem ToLiquidModel(this cartitem row, long? viewerid)
        {
            var item = new CartItem
            {
                id = row.id,
                quantity = row.quantity,
                price = row.product_variant.product.ToUserPrice(viewerid).Value,
                title = row.product_variant.ToProductFullTitle(),
                product = row.product_variant.product.ToLiquidModel(viewerid, "")
            };
            item.variant = row.product_variant.ToLiquidModel(item.price);
            item.total_price = item.price * item.quantity;
            return item;
        }
    }

}