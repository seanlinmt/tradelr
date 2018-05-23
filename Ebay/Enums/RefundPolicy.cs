using System.ComponentModel;

namespace Ebay.Enums
{
    public enum RefundPolicy
    {
        [Description("Merchandise Credit")]
        MerchandiseCredit,

        [Description("Exchange")]
        Exchange,

        [Description("Money Back")]
        MoneyBack,
    }
}
