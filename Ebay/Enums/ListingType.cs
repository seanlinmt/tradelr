using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Ebay.Enums
{
    public enum ListingType
    {
        [Description("Auction")]
        Chinese,
        [Description("Fixed Price")]
        FixedPriceItem
    }
}
