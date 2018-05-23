using System.Collections.Generic;
using System.Data.Linq;
using System.Text;
using tradelr.Common.Constants;
using tradelr.Common.Models.currency;
using tradelr.DBML;
using tradelr.Library.Constants;
using tradelr.Models.products;

namespace tradelr.Models.transactions
{
    public class OrderItem
    {
        public long? id { get; set; } // productid: used when deserializing order and checkout items
        public int quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public string SKU { get; set; }
        public string description { get; set; }
        public string variantid { get; set; }
        public decimal UnitPriceWithTax { get; set; }
        public string tax { get; set; }


        // for viewing invoice
        public string subTotalString { get; set; }
        public decimal subTotal
        {
            get { return UnitPrice*quantity; } 
            set {}
        }   // does not include tax

        // for shipping cost calculation
        public Dimension dimension { get; set; }
    }

    public static class OrderItemHelper
    {
        public static string ToDescriptionString(this EntitySet<orderItem> rows)
        {
            // just return the first non empty description
            var sb = new StringBuilder();
            foreach (var row in rows)
            {
                if (!string.IsNullOrEmpty(row.description))
                {
                    sb.Append(row.description);
                    break;
                }
                
            }
            if (sb.Length > GeneralConstants.ORDER_DESCRIPTION_STRING_LENGTH)
            {
                return sb.ToString(0, GeneralConstants.ORDER_DESCRIPTION_STRING_LENGTH) + "...";
            }
            return sb.ToString();
        }

        public static List<OrderItem> ToModel(this IEnumerable<orderItem> rows, Currency currency)
        {
            var items = new List<OrderItem>();
            foreach (var row in rows)
            {
                var item = new OrderItem
                               {
                                   id = row.variantid,
                                   SKU = row.product_variant.sku,
                                   description = row.description,
                                   quantity = row.quantity
                               };

                // for viewing invoice
                if (row.unitPrice.HasValue)
                {
                    var subTotal = row.unitPrice.Value*row.quantity;
                    if (row.tax.HasValue)
                    {
                        subTotal = (1 + row.tax.Value/100)*subTotal;
                        item.tax = row.tax.Value + "%";
                    }
                    item.UnitPrice = row.unitPrice.Value;
                    item.subTotal = subTotal;
                    item.subTotalString = subTotal.ToString("n" + currency.decimalCount);
                }

                items.Add(item);
            }
            return items;
        }
    }
}
