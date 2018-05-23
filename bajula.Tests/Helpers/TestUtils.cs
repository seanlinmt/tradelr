using System;
using System.Linq;
using tradelr.DBML;

namespace tradelr.Tests.Helpers
{
    
    public class TestUtils
    {
        /*
         * public static long addTestMember()
        {
            
            using (var db = new tradelrDataContext())
            {
                user u = new user();
                u.fullName = "test test";
                u.role = (int)UserRole.ALL;
                u.created = DateTime.Now;
                u.lastLogin = DateTime.Now;
                db.users.InsertOnSubmit(u);
                db.SubmitChanges();
                return u.id;
            }
             
        }
* */
        public static void deleteTestMember()
        {
            using (var db = new tradelrDataContext())
            {
                var result = db.users.Where(x => x.fullName == "test test").SingleOrDefault();
                if (result != null)
                {
                    db.users.DeleteOnSubmit(result);
                    db.SubmitChanges();
                }
            }
        }
    }
}
