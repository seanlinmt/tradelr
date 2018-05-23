using System.ComponentModel;

namespace tradelr.Models.transactions
{
    public enum TransactionType
    {
        ALL = 0,
        [Description("Order")]
        ORDER = 1,
        [Description("Invoice")]
        INVOICE = 2
    }
}
