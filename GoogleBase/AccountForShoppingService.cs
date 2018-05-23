using System;
using Google.GData.Client;

namespace GoogleBase
{
    /// <summary>
    /// http://code.google.com/apis/shopping/content/multiclientaccounts/mcaccounts.html
    /// </summary>
    public class AccountForShoppingService : Service
    {
        public const String GAccountForShoppingService = "structuredcontent";

        public AccountForShoppingService(String applicationName)
            : base(GAccountForShoppingService, applicationName)
        {
            this.NewAtomEntry += new FeedParserEventHandler(this.OnParsedNewEntry);
        }

        public string Insert(AccountFeed feed, AccountEntry entry)
        {
            var created = base.Insert(feed, entry);

            foreach (var link in created.Links)
            {
                if (link.Rel == "self")
                {
                    var segments = link.HRef.Content.Split(new[] {'/'});
                    return segments[segments.Length - 1];
                }
            }

            return "";
        }

        public AccountFeed Query(AccountQuery feedQuery)
        {
            return base.Query(feedQuery) as AccountFeed;
        }

        public AccountEntry Update(AccountEntry entry)
        {
            return base.Update(entry);
        }

        public void Delete(AccountEntry entry)
        {
            base.Delete(entry);
        }
    }
}
