using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace tradelr.Areas.dashboard.Models.theme
{
    public class ThemeSettingsViewModel
    {
        public string ThemeTitle { get; set; }
        public string SettingsHtml { get; set; }
        public string SettingsJson { get; set; }
        public List<SelectListItem> presetList { get; set; }
        public ThemeType themeType { get; set; }

        public ThemeSettingsViewModel()
        {
            presetList = new List<SelectListItem>();
        }
    }
}