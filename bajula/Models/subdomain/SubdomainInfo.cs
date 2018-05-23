using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using tradelr.Models.account;

namespace tradelr.Models.subdomain
{
    public class SubdomainInfo : SubdomainStats
    {
        public string subdomain { get; set; }
        public string companyName { get; set; }
        public Dictionary<AccountDataType, long> accountData { get; set; }

        public SubdomainInfo()
        {
            accountData = new Dictionary<AccountDataType, long>();
        }
    }
}
