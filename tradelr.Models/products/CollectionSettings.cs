using System;

namespace tradelr.Models.products
{
    [Flags]
    public enum CollectionSettings
    {
        NONE = 0,
        VISIBLE = 1,
        PERMANENT = 2
    }
}