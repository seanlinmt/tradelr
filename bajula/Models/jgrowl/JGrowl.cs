using System;
using System.Collections.Generic;

using tradelr.DBML;
using tradelr.Library.Constants;
using tradelr.Models.account;
using tradelr.Models.account.plans;
using tradelr.Models.users;

namespace tradelr.Models.jgrowl
{
    public class JGrowl
    {
        public static readonly JGrowl USER_UNVERIFIED =
            new JGrowl("You have not yet verified your email address. Please check your spam mail folder.", false);

        public static readonly  JGrowl USER_UNREGISTERED =
#if PAYWALL
            new JGrowl("To update your profile, you will need to <a href=\"/pricing\">create an account</a>", true);
#else
 new JGrowl("To update your profile, you will need to <a href=\"" + GeneralConstants.HTTP_HOST + "/pricing\">create an account</a>", true);
#endif
        public static readonly JGrowl USER_CHANGEPASSWORD =
            new JGrowl("Please <a href=\"#\" onclick=\"dialogBox_open('/dashboard/account/password');return false;\">change your password</a>", true);

        public JGrowl()
        {
            
        }

        public JGrowl(string msg, bool stick)
        {
            sticky = stick;
            message = msg;
        }

        public JGrowl(string msg, string parameter, bool stick)
            :this(string.Format(msg, parameter), stick)
        {
        }

        public bool sticky { get; set; }
        public string message { get; set; }
    }

    public static class JGrowlHelper
    {
        public static List<JGrowl> ToNotification(this string info)
        {
            var notifications = new List<JGrowl> {new JGrowl {message = info, sticky = true}};
            return notifications;
        }

        public static List<JGrowl> ToNotification(this user info, bool trialExpired)
        {
            var notifications = new List<JGrowl>();

            // check that trial period has not expired
            if (trialExpired && (info.role & (int)UserRole.ADMIN) != 0)
            {
                var growl = new JGrowl(
                        "Your free trial period has expired.", true);
                notifications.Add(growl);
            }

            // check if user has verified email
            var role = (UserRole)info.role;
            if ((role & UserRole.UNVERIFIED) != 0)
            {
                notifications.Add(JGrowl.USER_UNVERIFIED);
            }

            // check if user is registered
            /*
            if ((role & UserRole.ADMIN) == 0)
            {
                notifications.Add(JGrowl.USER_UNREGISTERED);
            }
             * */

            if ((info.settings & (int)UserSettings.PASSWORD_RESET) != 0)
            {
                notifications.Add(JGrowl.USER_CHANGEPASSWORD);
            }
            return notifications;
        }
    }
}
