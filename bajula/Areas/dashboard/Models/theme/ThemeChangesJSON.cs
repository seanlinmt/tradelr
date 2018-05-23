using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace tradelr.Areas.dashboard.Models.theme
{
    public class ThemeChangesJSON
    {
        public string[] names { get; set; }
        public string[] contents { get; set; }
        public bool ismobile { get; set; }
    }
}