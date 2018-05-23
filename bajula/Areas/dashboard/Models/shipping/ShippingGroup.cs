using System.Collections.Generic;
using tradelr.Models.shipping;

namespace tradelr.Areas.dashboard.Models.shipping
{
    public class ShippingGroup
    {
        public string name { get; set; }
        public List<ShippingRule> rules { get; set; }
        public List<ShippingGroup> subgroup { get; set; }

        public ShippingGroup()
        {
            rules = new List<ShippingRule>();
            subgroup = new List<ShippingGroup>();
        }

        public void AddSubGroup(ShippingGroup group)
        {
            subgroup.Add(group);
        }

        public void AddRule(ShippingRule rule)
        {
            rules.Add(rule);
        }
    }
}