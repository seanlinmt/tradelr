using tradelr.Models.shipping;

namespace tradelr.Areas.dashboard.Models.shipping.viewmodel
{
    public class ShippingRuleViewModel : ShippingRule
    {
        public bool isMetric { get; set; }
    }
}