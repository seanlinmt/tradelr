using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace tradelr.Library.Constants
{
    public static class FontType
    {
        public static bool IsSanSerif(this string font)
        {
            return SansSerifFonts.Contains(font.ToLower());
        }

        public static bool IsSerif(this string font)
        {
            return SerifFonts.Contains(font.ToLower());
        }

        public static bool IsMonospace(this string font)
        {
            return MonospaceFonts.Contains(font.ToLower());
        }

        public static readonly string[] MonospaceFonts = new[]
                                                        {
                                                           "courier", "courier new",
                                                           "dejavu sans mono",
                                                           "lucida console",
                                                           "monaco", "monospace"
                                                        };

        public static readonly string[] SansSerifFonts = new[]
                                                        {
                                                            "arial",
                                                            "charcoal",
                                                            "helvetica",
                                                            "impact",
                                                            "lucida", "lucida grande", "lucida sans","lucida sans unicode",
                                                            "sans-serif",
                                                            "trebuchet ms",
                                                            "verdana"
                                                        };

        public static readonly string[] SerifFonts = new[]
                                                    {
                                                        "baskerville", "book antiqua",
                                                        "caslon",
                                                        "garamond", "georgia",
                                                        "palatino", "palatino linotype",
                                                        "serif",
                                                        "times", "times new roman",
                                                        "utopia"
                                                    };
    }
}
