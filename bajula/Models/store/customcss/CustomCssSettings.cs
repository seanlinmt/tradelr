using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace tradelr.Models.store.customcss
{
    public class CustomCssSettings
    {
        public string target { get; set; }
        public string display { get; set; }
        public string value { get; set; }
        public CssSettingType type { get; set; }
    }
}