using System.ComponentModel;

namespace tradelr.Models.activity
{
    public enum ActivityType
    {
        [Description("contact")]
        CONTACT,
        [Description("purchase order")]
        ORDER,
        [Description("invoice")]
        INVOICE,
        [Description("product")]
        PRODUCT,
        [Description("profile")]
        PROFILE,
        [Description("inventory")]
        INVENTORY
    }
}