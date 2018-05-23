using System.ComponentModel;

namespace tradelr.Models.shipping
{
    public enum ShippingProfileType
    {
        NONE,
        [Description("Flat Rate")]
        FLATRATE,
        [Description("Calculated")]
        CALCULATED,
        [Description("Shipwire")]
        SHIPWIRE
    }
}