using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using tradelr.DataAccess;
using tradelr.DBML;

namespace tradelr.Models.products
{
    public class StockUnit
    {
        public long id { get; set; }
        public string title { get; set; }
    }

    public static class StockUnitHelper
    {
        public static string ToJQGridSelect(this IEnumerable<stockUnit> values)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var value in values)
            {
                var name = value.MASTERstockUnit.name;
                sb.Append(name);
                sb.Append(":");
                sb.Append(name);
                sb.Append(";");
            }
            var data = sb.ToString();
            if (data.Length == 0)
            {
                return "";
            }
            return data.Substring(0, data.Length - 2);
        }

        public static StockUnit ToModel(this stockUnit obj)
        {
            return new StockUnit
            {
                title = obj.MASTERstockUnit.name,
                id = obj.id
            };
        }
    }
}
