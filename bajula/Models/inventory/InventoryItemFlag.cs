using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace tradelr.Models.inventory
{
    [Flags]
    public enum InventoryItemFlag
    {
        GBASE,
        ETSY,
        EBAY,
        DRAFT
    }
}