using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Web;
using tradelr.Common;
using tradelr.Common.Library.Imaging;
using tradelr.Library;

namespace tradelr.Models.store.customcss
{
    public class CustomCss
    {
        public string background { get; set; }
        public string text { get; set; }
        public string link { get; set; }
        public string navigation { get; set; }
        public string border { get; set; }
    }

    public static class CustomCssHelper
    {
        public static string ToDisplayCss(this CustomCss customcss)
        {
            var sb = new StringBuilder();
            if (!string.IsNullOrEmpty(customcss.background))
            {
                if (customcss.background.Contains("repeat"))
                {
                    sb.Append(".custom_background { background:" + customcss.background + "; }");
                }
                else
                {
                    sb.Append(".custom_background { background-color:" + customcss.background + "; }");
                }
            }

            if (!string.IsNullOrEmpty(customcss.text))
            {
                sb.Append(".custom_text { color: " + customcss.text + "; }");
            }

            if (!string.IsNullOrEmpty(customcss.link))
            {
                var top = customcss.link.FromRGBToColor();
                double hue;
                double saturation;
                double value;
                ColourHelper.ColourToHSV(top, out hue, out saturation, out value);
                value = value * 0.7; // make darker
                var bottom = ColourHelper.ColourFromHSV(hue, saturation, value);

                sb.Append(".custom_link > a { color: #" + bottom.ToHTMLColor() + "; }");
                sb.Append(".custom_link > a:hover { text-decoration:underline; }");
            }

            if (!string.IsNullOrEmpty(customcss.border))
            {
                sb.Append(".custom_border { border: " + customcss.border + " !important; }");
            }

            if(!string.IsNullOrEmpty(customcss.navigation))
            {
                var top = customcss.navigation.FromRGBToColor();
                double hue;
                double saturation;
                double value;
                ColourHelper.ColourToHSV(top, out hue, out saturation, out value);
                value = value * 0.8; // make darker
                var bottom = ColourHelper.ColourFromHSV(hue, saturation, value);
                sb.Append(".custom_navigation {");
                sb.AppendFormat("background-color: {0};", bottom.ToHTMLColor());
                sb.AppendFormat("background-image: -moz-linear-gradient(top, #{0}, #{1});", top.ToHTMLColor(), bottom.ToHTMLColor());
                sb.AppendFormat("background-image: -webkit-gradient(linear, left top, left bottom, from(#{0}), to(#{1}));", top.ToHTMLColor(), bottom.ToHTMLColor());
                sb.AppendFormat("-ms-filter: \"progid:DXImageTransform.Microsoft.gradient(startColorstr=#{0},endColorstr=#{1})\";", top.ToHTMLColor(), bottom.ToHTMLColor());
                sb.AppendFormat("filter:progid:DXImageTransform.Microsoft.gradient(startColorstr=#{0},endColorstr=#{1});", top.ToHTMLColor(), bottom.ToHTMLColor());
                sb.Append("}");
            }

            return sb.ToString();
        }
    }
}