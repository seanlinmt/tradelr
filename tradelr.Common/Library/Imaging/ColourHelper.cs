using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace tradelr.Common.Library.Imaging
{
    public static class ColourHelper
    {
        public static void ColourToHSV(Color color, out double hue, out double saturation, out double value)
        {
            int max = Math.Max(color.R, Math.Max(color.G, color.B));
            int min = Math.Min(color.R, Math.Min(color.G, color.B));

            hue = color.GetHue();
            saturation = (max == 0) ? 0 : 1d - (1d * min / max);
            value = max / 255d;
        }

        public static Color ColourFromHSV(double hue, double saturation, double value)
        {
            int hi = Convert.ToInt32(Math.Floor(hue / 60)) % 6;
            double f = hue / 60 - Math.Floor(hue / 60);

            value = value * 255;
            int v = Convert.ToInt32(value);
            int p = Convert.ToInt32(value * (1 - saturation));
            int q = Convert.ToInt32(value * (1 - f * saturation));
            int t = Convert.ToInt32(value * (1 - (1 - f) * saturation));

            if (hi == 0)
                return Color.FromArgb(255, v, t, p);
            else if (hi == 1)
                return Color.FromArgb(255, q, v, p);
            else if (hi == 2)
                return Color.FromArgb(255, p, v, t);
            else if (hi == 3)
                return Color.FromArgb(255, p, q, v);
            else if (hi == 4)
                return Color.FromArgb(255, t, p, v);
            else
                return Color.FromArgb(255, v, p, q);
        }

        public static string ToHTMLColor(this Color color)
        {
            return string.Concat(color.R.ToString("X"), color.G.ToString("X"), color.B.ToString("X"));
        }

        public static Color FromRGBToColor(this string rgb)
        {
            if (rgb.IndexOf('#') == -1)
            {
                // RGB() color
                var regex = new Regex(@"rgb\((.+),(.+),(.+)\)");
                var match = regex.Match(rgb);
                if (!match.Success)
                {
                    throw new InvalidDataException(rgb);
                }
                return Color.FromArgb(int.Parse(match.Groups[1].Value), int.Parse(match.Groups[2].Value),
                                           int.Parse(match.Groups[3].Value));
            }

            // Hex Color
            return ColorTranslator.FromHtml(rgb);
        }

    }
}
