using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace tradelr.Models.facebook
{
    public class CallbackState
    {
        public string csrf_token { get; set; }
        public string return_url { get; set; }
        public string domain_name { get; set; }
        public string plan_name { get; set; }
        public string affiliate { get; set; }

        public bool isLogin { get; set; }
        public bool isRegistration { get; set; }
        public bool isLink { get; set; }
        public bool requestPageTokens { get; set; }
    }
}