using System;
using System.Web.Mvc;
using tradelr.DBML.Helper;
using tradelr.DataAccess;
using tradelr.DBML;
using tradelr.Libraries;
using tradelr.Libraries.ActionFilters;
using tradelr.Library.Constants;
using tradelr.Library.JSON;
using tradelr.Models.users;

namespace tradelr.Controllers.support
{
    
    //[ElmahHandleError]
    public class supportController : baseController
    {
#if !DEBUG
        [OutputCache(Duration = GeneralConstants.DURATION_1DAY_SECS, VaryByParam = "None")]
#endif
        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create(string firstname, string lastname, string email, string message)
        {
            var msg = new adminSupportMessage
                          {
                              created = DateTime.UtcNow,
                              email = email,
                              message = string.IsNullOrEmpty(message) ? "" : message,
                              firstName = string.IsNullOrEmpty(firstname) ? "" : firstname,
                              lastName = string.IsNullOrEmpty(lastname) ? "" : lastname
                          };
            repository.AddSupportMessage(msg);

            // send support a email
            Email.Email.SendMail("tradelr support", Email.Email.MAIL_SUPPORT_ADDRESS, "support message", message, null, true);

            return Json("".ToJsonOKMessage());
        }

        [HttpGet]
        public ActionResult Message()
        {
            return View();
        }

        /// <summary>
        /// don't give support access to normal user, any messages by user should be for the store owner
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        [HttpPost]
        [RoleFilter(role = UserRole.USER)]
        public ActionResult Message(string message)
        {
            var usr = db.GetUserById(sessionid.Value, subdomainid.Value);
            var msg = new adminSupportMessage
            {
                created = DateTime.UtcNow,
                email = usr.email,
                message = string.IsNullOrEmpty(message) ? "Empty message" : message,
                firstName = string.IsNullOrEmpty(usr.firstName) ? "" : usr.firstName,
                lastName = string.IsNullOrEmpty(usr.lastName) ? "" : usr.lastName
            };
            repository.AddSupportMessage(msg);

            Email.Email.SendMail("support", Email.Email.MAIL_SUPPORT_ADDRESS, string.Format("Support message: {0} {1}", usr.ToFullName(), usr.email), message, usr, true);

            return Json("OK".ToJsonOKMessage());
        }
    }
}