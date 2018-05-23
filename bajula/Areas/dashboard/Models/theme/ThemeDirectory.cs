using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace tradelr.Areas.dashboard.Models.theme
{
    public class ThemeDirectory
    {
        public string foldername { get; set; }
        public List<ThemeFile> files { get; set; }

        public ThemeDirectory()
        {
            files = new List<ThemeFile>();
        }
    }
}