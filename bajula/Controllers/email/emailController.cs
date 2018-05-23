using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using tradelr.DBML;
using tradelr.DBML.Helper;
using tradelr.Email;
using tradelr.Email.Models;
using tradelr.Libraries.ActionFilters;
using tradelr.Libraries.Helpers;
using tradelr.Library;
using tradelr.Library.JSON;
using tradelr.Models.email.store;
using tradelr.Models.subdomain;
using tradelr.Models.users;

namespace tradelr.Controllers.email
{
    //[ElmahHandleError]
    public class emailController : baseController
    {
        [ValidateInput(false)]
        [RoleFilter(role = UserRole.SEAN)]
        public ActionResult creators(string content, string subject)
        {
            content = HttpUtility.HtmlDecode(content);
            subject = HttpUtility.HtmlDecode(subject);

            var users = db.users.Where(x => (x.role & (int) UserRole.CREATOR) != 0);
            foreach (var user in users)
            {
                var email = user.email;
                var mail = new mail
                               {
                                   fromEmail = Email.Email.MAIL_SOURCE_ADDRESS,
                                   fromName = "tradelr",
                                   body = content,
                                   subject = subject,
                                   toEmail = email,
                                   toName = string.IsNullOrEmpty(user.ToFullName())? "": user.ToFullName()
                               };
                Email.Email.SendMail(mail, false, true);
            }
            return Json("".ToJsonOKData());
        }

        [RoleFilter(role = UserRole.SEAN)]
        public ActionResult Test2()
        {
            var entry = new mail
            {
                toName = "sean lin",
                toEmail =
                    "seanlinmt@gmail.com",
                subject = "test",
                body = "test",
                reference = ""
            };
            Email.Email.SendMail(entry, true, true);
            return Content("done");
        }


        /// <summary>
        /// resends verification email
        /// </summary>
        /// <returns></returns>
        public ActionResult Verification()
        {
            var user = MASTERdomain.organisation.users.First();
            var viewdata = new ViewDataDictionary()
                                   {
                                       {"host", MASTERdomain.ToHostName().ToDomainUrl()},
                                       {"confirmCode", user.confirmationCode},
                                       {"email", user.email}
                                   };
            EmailHelper.SendEmailNow(EmailViewType.ACCOUNT_CONFIRMATION, viewdata, "New Account Details and Email Verification Link",
                           user.email, user.ToFullName(), null);
            return new EmptyResult();
        }
    }
}