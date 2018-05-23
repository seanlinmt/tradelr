using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using tradelr.DBML;

namespace tradelr.Models.store.themes
{
    public class Theme
    {
        public string title { get; set; }
        public string thumbnail { get; set; }
    }

    public static class ThemeHelper
    {
        public static Theme ToModel(this theme row)
        {
            if (row == null)
            {
                return new Theme();
            }

            return new Theme()
                       {
                           thumbnail = row.url,
                           title = row.title
                       };
        }
    }

}