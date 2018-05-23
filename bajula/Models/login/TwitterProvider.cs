using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using tradelr.DataAccess;
using tradelr.DBML;

namespace tradelr.Models.login
{
    public class TwitterProvider : LoginProvider
    {
        public override bool DoesIDExist(string id)
        {
            throw new NotImplementedException();
        }

        public override user GetUserByProviderID(string id)
        {
            throw new NotImplementedException();
        }

    }
}
