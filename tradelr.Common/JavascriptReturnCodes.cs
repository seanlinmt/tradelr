using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace tradelr.Common
{
    public enum JavascriptReturnCodes
    {
        ENTRYFOUND = 0,
        INUSE = 1,
        ISLINKED = 2,
        LOGOUT = 3, // when user deleted fb connect account
        OK = 4,
        NOSHIPPINGADDRESS = 5,
        NOTLOGGEDIN = 6,
        NOTFOUND = 7,
        NOPERMISSION = 8,
        NOTOKEN = 9
    }
}
