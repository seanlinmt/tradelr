using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using tradelr.DBML;

namespace tradelr.Models.inventory
{
    public class History
    {
        public string created { get; set; }
        public string description { get; set; }
        public string instock { get; set; }
        public string reserved { get; set; }
        public string onOrder { get; set; }
        public string sold { get; set; }
    }

    public static class HistoryHelper
    {
        private static string ToDisplayString(this int? value)
        {
            if (!value.HasValue)
            {
                return "";
            }

            if (value.Value <= 0)
            {
                return value.Value.ToString();
            }

            return "+" + value.Value;
        }

        public static IEnumerable<History> ToModel(this IEnumerable<inventoryHistory> values)
        {
            foreach (var value in values)
            {
                yield return new History()
                                 {
                                     created = value.created.ToString("dd MMM yy"),
                                     description = value.description,
                                     instock = value.available.ToDisplayString(),
                                     reserved = value.reserved.ToDisplayString(),
                                     sold = value.sold.ToDisplayString(),
                                     onOrder = value.onOrder.ToDisplayString()
                                 };
            }
        }
    }
}