using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;
using tradelr.DBML;

namespace tradelr.Models.products
{
    public class Variant
    {
        public long? id { get; set; }
        public string sku { get; set; }
        public string color { get; set; }
        public string size { get; set; }
        public int? instock { get; set; }
        public int? onorder { get; set; }

    }

    public static class VariantHelper
    {
        
        public static List<Variant> ToModel(this IEnumerable<product_variant> values)
        {
            var list = new List<Variant>();
            foreach (var value in values)
            {
                var v = new Variant()
                            {
                                id = value.id,
                                sku = value.sku,
                                color = value.color,
                                size = value.size
                            };
                list.Add(v);
            }
            return list;
        }

        public static string ToJqgridModel(this IEnumerable<product_variant> values, long? locationid)
        {
            var sb = new StringBuilder();
            foreach (var value in values)
            {
                sb.Append("<tr>");

                var items = value.inventoryLocationItems.AsQueryable();
                if (locationid.HasValue)
                {
                    items = items.Where(x => x.locationid == locationid.Value);
                }

                var instock = items.All(x => x.available == null)
                                  ? null
                                  : items.Sum(x => x.available);
                var onOrder = items.All(x => x.onOrder == null)
                                  ? null
                                  : items.Sum(x => x.onOrder);
                var selling = items.All(x => x.reserved == null)
                                  ? null
                                  : items.Sum(x => x.reserved);
                var sold = items.All(x => x.sold == null)
                                  ? null
                                  : items.Sum(x => x.sold);
                sb.AppendFormat(
                    "<td><div class='bold'>{0}</div>{1} {2}</td><td class='w50px' title='available'>{3}</td><td class='w50px' title='on order'>{4}</td><td class='w50px' title='selling'>{5}</td><td class='w50px' title='sold'>{6}</td>",
                    string.IsNullOrEmpty(value.sku)?"-":value.sku, 
                    value.color, 
                    value.size,
                    instock.ToInventoryLevelString(), 
                    onOrder.HasValue?onOrder.Value.ToString():"&nbsp;", 
                    selling.HasValue?selling.Value.ToString():"&nbsp;",
                    sold.HasValue?sold.Value.ToString():"&nbsp;");
                sb.Append("</tr>");
            }
            return sb.ToString();
        }

        public static string ToProductFullTitle(this product_variant v)
        {
            var sb = new StringBuilder();
            sb.Append(v.product.title);

            if (!string.IsNullOrEmpty(v.color))
            {
                sb.AppendFormat(" / {0}", v.color);
            }

            if (!string.IsNullOrEmpty(v.size))
            {
                sb.AppendFormat(" / {0}", v.size);
            }

            return sb.ToString();
        }

        public static string ToHtmlLink(this product_variant v, bool openInNewPage = true)
        {
            return string.Format("<a href='{0}' target='{1}'>{2}</a>", v.product.ToProductUrl(), openInNewPage ? "_blank" : "_self", v.ToProductFullTitle());
        }
    }
}