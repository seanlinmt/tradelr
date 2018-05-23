using System;

namespace tradelr.Models.subdomain
{
    [Flags]
    public enum SubdomainFlags
    {
        IS_SUPPLIER                 = 0x1,// don't delete this as there will be existing entries in db
        STORE_ENABLED               = 0x2,
        OFFLINE_ENABLED             = 0x4,
        USE_SHIPWIRE_DEPRECATED     = 0x8, // don't delete this as there will be existing entries in db
        ETSY_AUTORENEW              = 0x10, // don't delete this as there will be existing entries in db
        TRACK_INVENTORY_DEPRECATED  = 0x20 // don't delete this as there will be existing entries in db
    }
}
