using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace tradelr.Models.store.themes
{
    /// <summary>
    /// list of public themes
    /// </summary>
    public static class ThemeGallery
    {
        public static IEnumerable<Theme> list = new[]
                                                    {
                                                        new Theme()
                                                            {
                                                                title = "Default"
                                                            }
                                                    };

    }
}