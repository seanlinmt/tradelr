using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using tradelr.Common.Models.currency;
using tradelr.DBML;
using tradelr.Library;
using tradelr.Models.shipping;

namespace tradelr.Areas.dashboard.Models.shipping
{
    public class ShippingProfile
    {
        public long id { get; set; }
        public string title { get; set; }
        public ShippingProfileType type { get; set; }
        public Currency currency { get; set; }
        public List<ShippingGroup> shippingGroups { get; set; }
        public List<ShippingRule> flatrateRules { get; set; }
        public string everywhereElseCost { get; set; }
        public bool applyEverywhereElseCost { get; set; }
        public bool shipwireEnabled { get; set; }
        public bool IsPermanent { get; set; }

        public ShippingProfile()
        {
            shippingGroups = new List<ShippingGroup>();
            flatrateRules = new List<ShippingRule>();
        }

        public static void UpdateFlatrateShipping(long profileid, decimal?[] shipping_cost, int?[] shipping_destination, long subdomainid)
        {
            using (var repository = new TradelrRepository())
            {
                repository.DeleteShippingRules(profileid, subdomainid);
                for (int i = 0; i < shipping_destination.Length; i++)
                {
                    var dest = shipping_destination[i];
                    var cost = shipping_cost[i];
                    if (cost.HasValue)
                    {
                        var rule = new shippingRule()
                        {
                            cost = cost.Value,
                            secondaryCost = cost.Value,
                            country = dest,
                            matchvalue = 0,
                            name = ShippingProfileType.FLATRATE.ToDescriptionString(),
                            profileid = profileid,
                            ruletype = (byte)RuleType.PRICE,
                            state = ""
                        };
                        repository.AddShippingRule(rule);
                    }
                }
            }
        }
    }

    public static class ShippingProfileHelper
    {
        public static bool UseShipwire(this IEnumerable<shippingProfile> profiles)
        {
            var types = profiles.Select(x => x.type).Distinct().ToArray();

            // if only shipwire then shipwire it is
            if (types.Length == 1 && types[0] == ShippingProfileType.SHIPWIRE.ToString())
            {
                return true;
            }
            return false;
        }

        public static IEnumerable<SelectListItem> ToShippingProfiles(this MASTERsubdomain domain, long? selected = null)
        {
            if (selected.HasValue)
            {
                return domain.shippingProfiles.OrderBy(x => x.title).Select(x => new SelectListItem()
                {
                    Value = x.id.ToString(),
                    Text = x.title,
                    Selected = x.id == selected.Value
                });
            }

            return domain.shippingProfiles.OrderBy(x => x.title).Select(x => new SelectListItem()
                                                           {
                                                               Value = x.id.ToString(),
                                                               Text = x.title,
                                                               Selected = x.permanent
                                                           });
        }
    }

}