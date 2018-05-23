using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace tradelr.Models.facebook
{
    [Flags]
    public enum FacebookTokenSettings
    {
        NONE = 0x0,
        POST_STREAM = 0x1,
        CREATE_ALBUM = 0x2
    }
}