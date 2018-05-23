using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using clearpixels.OAuth;
using tradelr.DBML;
using tradelr.Models.ebay;

namespace tradelr.Libraries.scheduler
{
    public static partial class ScheduledTask
    {
        private static void EbayPollForOrders(ITradelrRepository repository)
        {
            var tokens =
                repository.GetSubDomains().SelectMany(x => x.oauth_tokens).Where(
                    y => y.type == OAuthTokenType.EBAY.ToString() && y.authorised);

            foreach (var token in tokens)
            {
                var worker = new EbayWorker(token.MASTERsubdomain, token.token_key);
                worker.PollForEbayOrders();

                token.MASTERsubdomain.ebay_lastsync = DateTime.UtcNow;

            }
        }
    }
}