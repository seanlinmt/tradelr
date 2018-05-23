using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace tradelr.Models.store.customcss
{
    public enum CssSettingType
    {
        [Description("background-color")]
        BACKGROUND = 1,
        [Description("color")]
        COLOR = 2,
        ROUND = 4,
        SHADOW = 5,
        BACKGROUNDIMAGE = 6,
        BACKGROUNDGRADIENT = 7
    }
}