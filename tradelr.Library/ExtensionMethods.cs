using System;
using tradelr.Library.Constants;

namespace tradelr.Library
{
    public static class ExtensionMethods
    {
        public static string ToDomainUrl(this string hostname, string pathAndQuery, bool onlyUnsecure)
        {
            if (onlyUnsecure)
            {
                return string.Concat("http://", hostname, pathAndQuery);
            }
#if SUPPORT_HTTPS
            return string.Concat("https://", hostname, pathAndQuery);
#else
            return string.Concat("http://", hostname, pathAndQuery);
#endif
        }

        public static string ToDomainUrl(this string hostname)
        {
            return hostname.ToDomainUrl("", false);
        }

        public static string ToDomainUrl(this string hostname, bool onlyUnsecure)
        {
            return hostname.ToDomainUrl("", onlyUnsecure);
        }

        public static string ToDomainUrl(this string hostname, string pathAndQuery)
        {
            return hostname.ToDomainUrl(pathAndQuery, false);
        }

        public static string ToTradelrDomainUrl(this string subdomain, string pathAndQuery)
        {
            return string.Concat("http://", subdomain, ".", GeneralConstants.SUBDOMAIN_HOST, pathAndQuery);
        }
    }
}
