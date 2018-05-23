using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using tradelr.DataAccess;
using tradelr.DBML;

namespace tradelr.Models.login
{
    public class OpenIDProvider : LoginProvider
    {
        public override bool DoesIDExist(string providerID)
        {
            using (var db = new tradelrDataContext())
            {
                var result = db.users.Where(x => x.openID == providerID).SingleOrDefault();
                return result != null;
            }
        }

        public override user GetUserByProviderID(string providerID)
        {
            using (var db = new tradelrDataContext())
            {
                return db.users.Where(x => x.openID == providerID).SingleOrDefault();
            }
        }
    }
}
