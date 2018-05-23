using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using tradelr.Common.Constants;
using tradelr.Library.Constants;
using clearpixels.Logging;

namespace tradelr.Libraries
{
    public class AbuseWorker
    {
        private long productid { get; set; }
        private string subdomainname { get; set; }

        public AbuseWorker(string subdomain, long productid)
        {
            this.productid = productid;
            this.subdomainname = subdomain;
        }

        public void CheckForRestrictedKeywords(string product_description)
        {
            foreach (var keyword in GeneralConstants.KEYWORD_RESTRICTED)
            {
                if (product_description.IndexOf(keyword) != -1)
                {
                    Syslog.Write("Product {0} on {1} contains restricted keyword {2}", productid, subdomainname,
                                               keyword);
                }
            }
        }
    }
}