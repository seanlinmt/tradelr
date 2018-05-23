using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using tradelr.DBML;

namespace tradelr.Models.store.themes
{
    public class ThemeViewModel
    {
        public Theme current { get; set; }
        public List<Theme> gallery { get; set; }

        public ThemeViewModel()
        {
            gallery = new List<Theme>();
        }
    }

    public static class ThemeViewModelHelper
    {
        
    }
}