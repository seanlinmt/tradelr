using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace tradelr.Models.offline
{
    public enum CFlag
    {
        NONE = 0,
        UPDATE = 1, 
        DELETE = 2,
        CREATE = 3,
        CLEAR = 4
    }
}