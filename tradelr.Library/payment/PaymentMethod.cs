using System.ComponentModel;

namespace tradelr.Library.payment
{
    public enum PaymentMethod
    {
        [Description("Cash in Person")]
        CashInPerson,
        [Description("Cash on Delivery")]
        COD,
        [Description("Loan Check")]
        LoanCheck,
        [Description("Money Order / Cashier Check")]
        MoneyOrder,
        [Description("Moneybookers")]
        Moneybookers,
        [Description("Direct Bank Transfer")]
        BankTransfer,
        [Description("Paypal")]
        Paypal,
        [Description("Pay on Pickup")]
        PayOnPickup,
        [Description("Personal Check")]
        PersonalCheck,
        [Description("Other")]
        Other
    }
}
