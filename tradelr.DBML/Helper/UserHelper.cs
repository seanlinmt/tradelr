using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace tradelr.DBML.Helper
{
    public static class UserHelper
    {
        public static string ToFullName(this user usr)
        {
            var name = String.Format("{0} {1}", usr.firstName, usr.lastName);
            if (!String.IsNullOrEmpty(name.Trim()))
            {
                return name;
            }
            return usr.organisation1.name;
        }

        public static string ToEmail(this user usr)
        {
            if (!string.IsNullOrEmpty(usr.email))
            {
                return usr.email;
            }

            return usr.proxiedEmail;
        }
    }
}
