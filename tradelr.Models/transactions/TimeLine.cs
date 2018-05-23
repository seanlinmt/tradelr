using System.ComponentModel;

namespace tradelr.Models.transactions
{
    public enum Timeline
    {
        [Description("only last 24 hours")]
        Last24Hours,
        [Description("only last week")]
        LastWeek,
        [Description("only last month")]
        LastMonth,
        [Description("only last quarter")]
        LastQuarter,
        [Description("only last year")]
        LastYear,
        [Description("only last 5 years")]
        Last5Years
    }
}