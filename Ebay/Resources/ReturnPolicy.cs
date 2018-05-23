using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Ebay.Resources
{
    public enum ReturnPolicy
    {
        [Description("No returns accepted")]
        ReturnsNotAccepted,
        [Description("Returns Accepted")]
        ReturnsAccepted
    }
}
