using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace tradelr.Areas.dashboard.Models.store.blog
{
    public enum Commenting
    {
        [Description("comments are disabled")]
        OFF,
        [Description("comments are allowed and are posted immediately")]
        ON,
        [Description("comments are allowed but requires approval")]
        MODERATED
    }
}