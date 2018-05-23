using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using tradelr.DBML;
using clearpixels.Logging;

namespace tradelr.Libraries.Affiliate
{
    public static class AffiliateUtil
    {
        public static string GenerateAffiliateID()
        {
            string code = "";
            int count = 0;
            while (true)
            {
                code = Crypto.Utility.GetRandomString(10, true);

                using (var repository = new TradelrRepository())
                {
                    if (!repository.GetSubDomains().Any(x => x.affiliateID == code))
                    {
                        break;
                    }
                }

                if (count++ >= 32)
                {
                    Syslog.Write("Unable to generate affilliate ID");
                    break;
                }
            }

            return code;
        }
    }
}