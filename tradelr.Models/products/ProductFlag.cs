using System;

namespace tradelr.Models.products
{
    [Flags]
    public enum ProductFlag
    {
        NONE = 0,
        INACTIVE = 1,
        ARCHIVED = 2
        // FEATURED = 4   // don't remove because this flag will still be in the db
    }
}