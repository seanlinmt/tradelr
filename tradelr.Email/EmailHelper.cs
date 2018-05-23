using System.Web.Mvc;
using tradelr.DBML;
using tradelr.Email.Models;
using tradelr.Library;
using Controller = System.Web.Mvc.Controller;

namespace tradelr.Email
{
    public static class EmailHelper
    {
        private const string tradelrLogoPath = "";

        private static readonly string[] styles = new[]
                                  {
                                      "div, table {font: 12.5px/20px arial, sans-serif;}",
                                  };

        private static string ToLogo(this user sender)
        {
            if (sender == null)
            {
                return "<img src='http://www.tradelr.com/Content/img/tradelr.png' alt='tradelr.com' />";
            }

            if (!sender.organisation1.logo.HasValue)
            {
                return string.Format("<h2>{0}</h2>", sender.organisation1.name);
            }

            return string.Format("<img src='http://www.tradelr.com{0}' alt='{1}' />", sender.organisation1.image.url, sender.organisation1.name);
        }


        public static void SendEmail(this Controller controller, EmailViewType type,
                    object viewData, string subject, string destEmail, string destName, user sender)
        {
            string body = controller.RenderViewToString(type.ToDescriptionString(), viewData);

            // wrap body with div to format it
            body = string.Format("<style type='text/css'>{0}</style>{1}<div>{2}</div>",
                string.Join("", styles),
                sender.ToLogo(),
                body);

            Email.SendMail(destName, destEmail, subject, body, sender, true);
        }

        /// <summary>
        /// non TModel partial views
        /// </summary>
        /// <param name="type"></param>
        /// <param name="viewData"></param>
        /// <param name="subject"></param>
        /// <param name="destEmail"></param>
        /// <param name="destName"></param>
        /// <param name="ownerid"></param>
        public static void SendEmail(EmailViewType type,
                    ViewDataDictionary viewData, string subject, string destEmail, string destName, user sender, bool queueMail = true)
        {
            string body = ViewHelpers.RenderViewToString(type.ToDescriptionString(), viewData);

            // wrap body with div to format it
            body = string.Format("<style type='text/css'>{0}</style>{1}<div>{2}</div>",
                string.Join("", styles),
                tradelrLogoPath, 
                body);
            Email.SendMail(destName, destEmail, subject, body, sender, queueMail);
        }

        public static void SendEmailNow(EmailViewType type,
                    ViewDataDictionary viewData, string subject, string destEmail, string destName, user sender)
        {
            SendEmail(type, viewData, subject, destEmail, destName, sender, false);
        }

        public static string ToMailToLink(this string email, string subject = "")
        {
            string emailLink = email;
            if (!string.IsNullOrEmpty(subject))
            {
                emailLink += "?subject=" + subject;
            }
            return string.Concat("<a href=\"mailto:", emailLink, "\" >", email, "</a>");
        }

        public static string ToObfuscatedEmail(this string email)
        {
            email = email.Replace(".", " dot ");
            email = email.Replace("@", " at ");
            return email;
        }
    }
}