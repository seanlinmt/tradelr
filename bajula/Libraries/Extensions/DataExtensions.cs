using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace tradelr.Libraries.Extensions
{
    public static class DataExtensions
    {
        public static string ToJQGridSelect(this IEnumerable<DictionaryEntry> values)
        {
            if (values.Count() == 0)
            {
                return ":";
            }
            StringBuilder sb = new StringBuilder();
            sb.Append(":;");
            foreach (var entry in values)
            {
                sb.Append(entry.Value);
                sb.Append(":");
                sb.Append(entry.Key);
                sb.Append(";");
            }
            var data = sb.ToString();
            return data.Substring(0, data.Length - 1);
        }
    }

    
}
