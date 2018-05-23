using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Etsy
{
    public class SessionInfo
    {
        public string oauth_key { get; set; }
        public string oauth_secret { get; set; }

        public SessionInfo(string key, string secret)
        {
            oauth_key = key;
            oauth_secret = secret;
        }
    }
}
