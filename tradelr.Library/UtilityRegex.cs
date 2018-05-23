using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace tradelr.Library
{
    public static class UtilityRegex
    {
        public static string ConvertToSafeFileName(string unicodeString)
        {
            string retString = Regex.Replace(unicodeString, @"[\^\$&\+,/:;=\?@<>#%\{\}\|\\^~\[\]'`]", "");

            return retString;
        }

        public static string ExtractProductPrice(this string description)
        {
            if (string.IsNullOrEmpty(description))
            {
                return "";
            }
            var priceRegex = new Regex(@"[a-zA-Z\$]*(\d+\.\d+)");
            var m = priceRegex.Match(description);
            if (!m.Success)
            {
                return "";
            }
            return m.Groups[1].Value;
        }

        public static string ExtractProductSKU(this string description)
        {
            if (string.IsNullOrEmpty(description))
            {
                return "";
            }
            var skuRegex = new Regex(@"([a-zA-Z]+\d+)");
            var matches = skuRegex.Matches(description);
            for (int i = 0; i < matches.Count; i++)
            {
                var m = matches[i];
                if (m.Success)
                {
                    var sku = m.Groups[1].Value;
                    if (sku.Length > 3)
                    {
                        return sku;
                    }
                }
            }
            return "";
        }

        public static string GetFirstLine(this string description)
        {
            if (string.IsNullOrEmpty(description))
            {
                return "";
            }

            var firstlineRegex = new Regex("^(.*)", RegexOptions.Multiline);
            var m = firstlineRegex.Match(description);
            if (m.Success)
            {
                return m.Groups[1].Value;
            }
            return "";
        }

        public static string StripHtmlTags(this string html)
        {
            if (string.IsNullOrEmpty(html))
            {
                return "";
            }
            return Regex.Replace(html, @"<(.|\n)*?>", string.Empty);
        }

        public static string StripNonWord(this string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return "";
            }
            return Regex.Replace(text, @"\W+", " ");
        }

        public static string StripWhitespace(this string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return "";
            }
            return Regex.Replace(text, @"\s", "");
        }

        public static string StripSubjectSender(this string subject)
        {
            if (string.IsNullOrEmpty(subject))
            {
                return "";
            }
            return Regex.Replace(subject, @"\[.+\] ", " ");
        }

        public static bool HasNonword(this string value)
        {
            var regex = new Regex(@"\W+");

            return regex.Match(value).Success;
        }

        public static string AutoLinkUrls(this string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return text;
            }

            Regex r = new Regex(@"((www\.|(http|https|ftp|news|file)+\:\/\/)[&#95;.a-z0-9-]+\.[a-z0-9\/&#95;:@=.+?,##%&~-]*[^.|\'|\# |!|\(|?|,| |>|<|;|\)])", RegexOptions.IgnoreCase);

            return r.Replace(text, x => string.Format("<a target='_blank' href='{0}'>{0}</a>", x.Groups[1].Value));
        }
        
        public static string[] UrlsFromText(this string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return new string[0];
            }

            Regex r = new Regex(@"((www\.|(http|https|ftp|news|file)+\:\/\/)[&#95;.a-z0-9-]+\.[a-z0-9\/&#95;:@=.+?,##%&~-]*[^.|\'|\# |!|\(|?|,| |>|<|;|\)])", RegexOptions.IgnoreCase);
        
            var match = r.Match(text);
            if (!match.Success)
            {
                return new string[0];
            }

            var results = new string[match.Groups.Count - 1];
            for (int i = 0; i < match.Groups.Count - 1; i++)
            {
                results[i] = match.Groups[i + 1].Value;
            }
            return results;
        }
    }
}
