using System.Collections.Generic;
using clearpixels.Logging;

namespace tradelr.Models.account.plans
{
    public class AccountPlanLimits
    {
        public int? skus { get; set; }
        public int? invoices { get; set; }
        public int? locations { get; set; }

        public static readonly Dictionary<AccountPlanType, AccountPlanLimits> PLANS = new Dictionary
            <AccountPlanType, AccountPlanLimits>()
                                                                                 {
                                                                                     {
                                                                                         AccountPlanType.SINGLE,
                                                                                         new AccountPlanLimits()
                                                                                             {
                                                                                                 skus = 25,
                                                                                                 invoices = 100,
                                                                                                 locations = 5
                                                                                             }
                                                                                         },
                                                                                         {
                                                                                         AccountPlanType.BASIC,
                                                                                         new AccountPlanLimits()
                                                                                             {
                                                                                                 skus = 100,
                                                                                                 invoices = 500,
                                                                                                 locations = 10
                                                                                             }
                                                                                         },
                                                                                         {
                                                                                         AccountPlanType.PRO,
                                                                                         new AccountPlanLimits()
                                                                                             {
                                                                                                 skus = 500,
                                                                                                 invoices = 1000,
                                                                                                 locations = 50
                                                                                             }
                                                                                         },
                                                                                         {
                                                                                         AccountPlanType.ULTIMATE,
                                                                                         new AccountPlanLimits()
                                                                                             {
                                                                                                 skus = null,
                                                                                                 invoices = null,
                                                                                                 locations = null
                                                                                             }
                                                                                         }
                                                                                 };
    }

    public static class AccountPlanHelper
    {
        public static AccountPlanLimits ToAccountLimit(this AccountPlanType type)
        {
            if (AccountPlanLimits.PLANS.ContainsKey(type))
            {
                return AccountPlanLimits.PLANS[type];
            }
            Syslog.Write("can't find accountlimit key");
            return null;
        }
 
    }
}