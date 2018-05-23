using System;
using System.Collections.Specialized;
using System.Web;

namespace clearpixels.Facebook.Helpers
{
    public static class FacebookUtility
    {
        public static string ToQueryString(this NameValueCollection nvc, bool encode)
        {
            if (encode)
            {
                return "?" + string.Join("&", Array.ConvertAll(nvc.AllKeys, key => string.Format("{0}={1}", HttpUtility.UrlEncode(key), HttpUtility.UrlEncode(nvc[key]))));
            }
            else
            {
                return "?" + string.Join("&", Array.ConvertAll(nvc.AllKeys, key => string.Format("{0}={1}", key, nvc[key])));
            }
        }

        
    }
}
