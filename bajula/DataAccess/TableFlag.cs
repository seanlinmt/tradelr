using System;

namespace tradelr.DataAccess
{
    // used to mark all table row states
    public enum TableFlag
    {
        NONE = 0,
        ARCHIVED = 1,
        DELETED = 2,
    }
}