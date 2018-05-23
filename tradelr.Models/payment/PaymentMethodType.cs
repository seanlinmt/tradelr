using System.ComponentModel;

namespace tradelr.Models.payment
{
    public enum PaymentMethodType
    {
        [Description("Custom")]
        Custom = 3,
        [Description("Paypal")]
        Paypal = 4
    }
}