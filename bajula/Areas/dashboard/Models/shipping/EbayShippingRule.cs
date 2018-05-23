using System.Collections.Generic;
using System.Linq;
using tradelr.Common.Models.currency;
using tradelr.DBML;

namespace tradelr.Areas.dashboard.Models.shipping
{
    public class EbayShippingRule
    {
        public long id { get; set; }
        public string name { get; set; }
        public string cost { get; set; }
        public string shipToLocations { get; set; }
    }

    public static class EbayShippingRuleHelper
    {
        public static IEnumerable<EbayShippingRule> ToModel(this IEnumerable<ebay_shippingrule> rows, Currency currency)
        {
            foreach (var row in rows)
            {
                yield return row.ToModel(currency);
            }
        }

        public static EbayShippingRule ToModel(this ebay_shippingrule row, Currency currency)
        {
            return new EbayShippingRule()
                       {
                           id = row.id,
                           name = row.ebay_shippingservice.description,
                           cost = row.cost.ToString("n" + currency.decimalCount),
                           shipToLocations = string.Join(", ", row.ebay_shippingrule_locations.Select(x => x.description).ToArray())
                       };
        }
    }
}