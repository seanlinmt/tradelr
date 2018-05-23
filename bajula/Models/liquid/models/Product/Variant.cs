using System.Collections.Generic;
using System.Linq;
using DotLiquid;
using tradelr.DBML;
using tradelr.Models.products;

namespace tradelr.Models.liquid.models.Product
{
    public class Variant : Drop
    {
        public long id { get; set; }
        public string title { get; set; }
        public bool available { get; set; }
        public int inventory_quantity { get; set; }
        public string sku { get; set; }
        public bool requires_shipping { get; set; } // todo
        public bool taxable { get; set; } // todo
        public decimal? price { get; set; }

        public decimal? weight { get; set; }

        // proprietary
        public string option1 { get; set; } // color
        public string option2 { get; set; } // size
        public List<string> options { get; set; }

        public Variant()
        {
            options = new List<string>();
        }
    }

    public static class VariantHelper
    {
        public static Variant ToLiquidModel(this product_variant row, decimal? price)
        {
            var v = new Variant()
                       {
                           id = row.id,
                           title = row.ToVariantTitle(),
                           inventory_quantity = row.inventoryLocationItems.Sum(y => y.available) ?? 0,
                           sku = row.sku,
                           price = price,
                           option1 = row.color,
                           option2 = row.size
                       };
            // handle options
            if (!string.IsNullOrEmpty(v.option1))
            {
                v.options.Add(v.option1);
            }
            if (!string.IsNullOrEmpty(v.option2))
            {
                v.options.Add(v.option2);
            }

            v.available = row.HasStock();

            return v;
        }

        public static IEnumerable<Variant> ToLiquidVariantModel(this IEnumerable<product_variant> rows, decimal? price)
        {
            foreach (var row in rows)
            {
                yield return row.ToLiquidModel(price);
            }
        }

        public static string ToVariantTitle(this product_variant v)
        {
            var options = new List<string>();

            if (!string.IsNullOrEmpty(v.color))
            {
                options.Add(v.color);
            }

            if (!string.IsNullOrEmpty(v.size))
            {
                options.Add(v.size);
            }

            return string.Join(" / ", options);
        }
    }
}