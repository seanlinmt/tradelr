using System;

namespace tradelr.Models.account
{
    [Flags]
    public enum AccountPlanPaymentStatus
    {
        NONE = 0,
        PENDING = 0x0001,
        TRIAL = 0x0002
    }
}