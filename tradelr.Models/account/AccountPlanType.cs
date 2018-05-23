using System.ComponentModel;

namespace tradelr.Models.account
{
    public enum AccountPlanType
    {
        [Description("Unknown")]
        UNKNOWN,
        [Description("Single")]
        SINGLE,
        [Description("Basic")]
        BASIC,
        [Description("Pro")]
        PRO,
        [Description("Ultimate")]
        ULTIMATE
    }
}