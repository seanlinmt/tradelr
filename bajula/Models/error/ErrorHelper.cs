using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace tradelr.Models.error
{
    public static class ErrorHelper
    {
        public static string CreateErrorPage(string msg, string redirecturl)
        {
            return string.Format("/Error?msg={0}&redirect={1}", HttpUtility.UrlEncode(msg),
                                 HttpUtility.UrlEncode(redirecturl));
        }
    }
}