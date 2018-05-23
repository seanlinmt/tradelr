using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace tradelr.Common.Constants
{
    public static class GoogleConstants
    {
        // google
        public const string ANALYTICS_ID = "ANALYTICS_ID";
        public const string GOOGLE_OAUTH_CONSUMER_KEY = "tradelr.com";
        public const string GOOGLE_OAUTH_SECRET_KEY = "GOOGLE_OAUTH_SECRET_KEY";
#if DEBUG
        public const string GOOGLE_APIKEY = "GOOGLE_APIKEY";
        public const string GBASE_APIKEY = "GBASE_APIKEY";
#else
        public const string GOOGLE_APIKEY = "GOOGLE_APIKEY";
        public const string GBASE_APIKEY = "GBASE_APIKEY";
#endif
        public const string FEED_BLOGGER = "http://www.blogger.com/feeds/";
        public const string FEED_CONTACTS = "http://www.google.com/m8/feeds/";

    }
}
