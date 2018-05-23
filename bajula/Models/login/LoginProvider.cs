using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using tradelr.DataAccess;
using tradelr.DBML;

namespace tradelr.Models.login
{
    public abstract class LoginProvider
    {
        public abstract bool DoesIDExist(string providerID);
        public abstract user GetUserByProviderID(string providerID);
    }
}
