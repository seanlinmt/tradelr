using System;
using System.Linq;
using clearpixels.Models;
using tradelr.Library.Constants;
using tradelr.Library.payment;
using tradelr.Models.account;
using tradelr.Models.subdomain;

namespace tradelr.DBML
{
    public partial class MASTERsubdomain
    {
        public bool trialExpired
        {
            get
            {
                return trialExpiry.HasValue && accountTypeStatus.HasValue &&
                       DateTime.UtcNow > trialExpiry.Value &&
                       ((AccountPlanPaymentStatus) accountTypeStatus).HasFlag(AccountPlanPaymentStatus.TRIAL);
            }
        }

        public bool IsStoreEnabled()
        {
            return (flags & (int)SubdomainFlags.STORE_ENABLED) != 0;
        }

        public string GetPaypalID()
        {
            var paypal = paymentMethods.SingleOrDefault(x => x.method == PaymentMethod.Paypal.ToString());
            if (paypal != null)
            {
                return paypal.identifier;
            }

            return "";
        }

        public string ToHostName()
        {
            if (!String.IsNullOrEmpty(customDomain))
            {
                return customDomain;
            }

            return String.Format("{0}.{1}", name, GeneralConstants.SUBDOMAIN_HOST);
        }

        public IdName ToIdName()
        {
            return new IdName(id, name);
        }
    }
}
