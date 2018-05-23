using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Ebay.Enums
{
    public enum ReturnsWithin
    {
        [Description("60 Days")]
        Days_60,

        [Description("30 Days")]
        Days_30,

        [Description("14 Days")]
        Days_14,

        [Description("7 Days")]
        Days_7,

        [Description("3 Days")]
        Days_3
    }
}
