using System.Collections.Generic;
using System.Linq;
using tradelr.Common.Models.currency;
using tradelr.DBML;
using tradelr.Models.shipping;

namespace tradelr.Areas.dashboard.Models.shipping
{
    public class EbayShippingProfile
    {
        public IEnumerable<EbayShippingRule> domesticRules { get; set; }
        public IEnumerable<EbayShippingRule> internationalRules { get; set; }
    }

    public static class EbayShippingProfileHelper
    {
        public static EbayShippingProfile ToModel(this ebay_shippingprofile row)
        {
            var currency = row.MASTERsubdomain.currency.ToCurrency();

            var result = new EbayShippingProfile()
                             {
                                 domesticRules = row.ebay_shippingrules
                                     .Where(x => !x.ebay_shippingservice.isInternational)
                                     .ToModel(currency),
                                 internationalRules = row.ebay_shippingrules
                                     .Where(x => x.ebay_shippingservice.isInternational)
                                     .ToModel(currency)
                             };

            return result;
        }
    }
}