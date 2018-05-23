using System.ComponentModel;

namespace tradelr.Models.account
{
    public enum AccountDataType
    {
        [Description("purchase orders received")]
        ORDERS_RECEIVED,
        [Description("sales invoices received")]
        INVOICES_RECEIVED
    }
}
