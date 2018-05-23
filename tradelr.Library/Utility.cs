using System;
using System.Collections.Specialized;
using System.IO;
using System.Text.RegularExpressions;
using System.Web;
using tradelr.Library.Constants;
using System.Linq;
using tradelr.Time;

namespace tradelr.Library
{
    public static class Utility
    {
        public static int GetDecimalPlaces(this decimal value)
        {
            var str = value.ToString();
            var dotindex = str.IndexOf('.');
            if (dotindex == -1)
            {
                return 0;
            }
            return str.Length - dotindex;
        }

        public static string UrlEncode(string data)
        {
            var encoded = HttpUtility.UrlEncode(data);
            var regex = new Regex("%.{2}");
            var eval = new MatchEvaluator(x => x.Value.ToUpper());
            var uppercased = regex.Replace(encoded, eval);
            return uppercased;
        }

        public static bool IsNumber(this object value)
        {
            if (value is sbyte) return true;
            if (value is byte) return true;
            if (value is short) return true;
            if (value is ushort) return true;
            if (value is int) return true;
            if (value is uint) return true;
            if (value is long) return true;
            if (value is ulong) return true;
            if (value is float) return true;
            if (value is double) return true;
            if (value is decimal) return true;
            return false;
        }

        public static bool IsOnSubdomain(string[] host, out string hostSegment)
        {
            if (
#if DEBUG
                host.Length == 1 
#else
                host.Length == 2
#endif
            )
            {
                hostSegment = "";
                return false;
            }
#if DEBUG
            hostSegment = host[host.Length - 2].ToLower();
#else
            hostSegment = host[host.Length - 3].ToLower();
#endif

            if (GeneralConstants.SUBDOMAIN_RESTRICTED.Contains(hostSegment) ||
                hostSegment == GeneralConstants.SUBDOMAIN_HOST)
            {
                return false;
            }
            return true;
        }

        public static string ToCommaSeparator(this string original)
        {
            if (string.IsNullOrEmpty(original))
            {
                return "";
            }
            return original.Replace("\n", ", ");
        }

        public static string ToHtmlBreak(this string original)
        {
            var htmlstring = "";
            if (!string.IsNullOrEmpty(original))
            {
                htmlstring = original.Replace(Environment.NewLine, "<br/>");
            }

            return htmlstring;
        }

        public static string ToHtmlParagraph(this string original)
        {
            var htmlstring = "";
            if (string.IsNullOrEmpty(original))
            {
                return "";
            }
            htmlstring = "<p>" +
                         original.Replace(Environment.NewLine + Environment.NewLine, "</p><p>")
                                    .Replace(Environment.NewLine, "<br/>")
                                    .Replace("</p><p>", "</p>" + Environment.NewLine + "<p>")
                         + "</p>";


            return htmlstring;
        }

        public static string ToHtmlLink(this string link, string text, bool newWindow = false)
        {
            if (newWindow)
            {
                return string.Concat("<a target=\"_blank\" href=\"", link, "\" >", text, "</a>");
            }
            return string.Concat("<a href=\"", link, "\" >", text, "</a>");
        }

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

        public static string ToJSTime(this DateTime mstime)
        {
            return "/Date(" + UnixTime.ToInt64(mstime) + ")/";
        }

        public static string ToMaxLength(this string original, int maxlength)
        {
            if (original.Length > maxlength)
            {
                return original.Substring(0, maxlength);
            }
            return original;
        }

        /// <summary>
        /// returns an array where first element is first name and second is last name
        /// </summary>
        /// <param name="fullname"></param>
        /// <returns></returns>
        public static string[] SplitFullName(string fullname)
        {
            var name = new Regex(@"([^\s,]+)(\s|,)+(.*)");
            var match = name.Match(fullname);
            var result = new string[2];
            if (!match.Success)
            {
                result[0] = fullname;
                result[1] = "";
            }
            else
            {
                result[0] = match.Groups[1].Value;
                result[1] = match.Groups[3].Value;
            }
            return result;
        }

        public static string ToPerma(this string value)
        {
            return value.Trim().ToLower().ToSafeUrl().ToMaxLength(100);
        }

        public static string ToSafeUrl(this string str)
        {
            // to replace a bunch of -- with just one -
            Regex removeDuplicateDashRegex = new Regex("(-){2,}");
            return removeDuplicateDashRegex.Replace(Regex.Replace(str, @"[^\w]", "-"), "-");
        }

        public static string GenerateUniqueCode()
        {
            return Guid.NewGuid().ToString("N");
        }

        public static void CopyStream(Stream input, Stream output)
        {
            var buffer = new byte[8 * 1024];
            int len;
            while ((len = input.Read(buffer, 0, buffer.Length)) > 0)
            {
                output.Write(buffer, 0, len);
            }
        }

        public static string EmptyIfNull(string value)
        {
            if (value == null)
            {
                return String.Empty;
            }
            return value;
        }

        
    }
}
