using System;
using System.ComponentModel;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading;
using clearpixels.Logging;
using tradelr.DBML;
using tradelr.DBML.Helper;

namespace tradelr.Email
{
    public class Email
    {
#if DEBUG
        //public const string MAIL_SERVER = "smtp-proxy.tm.net.my";
        public const string MAIL_SERVER = "smtp.gmail.com";
#else
        public const string MAIL_SERVER = "mail.tradelr.com";
#endif
        public const string MAIL_SOURCE_ADDRESS = "mailer@tradelr.com";
        public const string MAIL_SUPPORT_ADDRESS = "support@tradelr.com";
        public const string MAIL_PASSWORD = "MAIL_PASSWORD";

        public string receiver { get; set; }
        public string sender { get; set; }
        public string heading { get; set; }
        public string subject { get; set; }
        public string message { get; set; }
        public string viewloc { get; set; }
        public long orderID { get; set; }
        public string orderType { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entry"></param>
        /// <param name="isAsync"></param>
        /// <param name="repository">if specified, email is queued</param>
        public static void SendMail(mail entry, bool isAsync, bool queueMail)
        {
            // need to check for invalid email address
            if (!entry.toEmail.IsEmail())
            {
                return;
            }

            if (queueMail)
            {
                // queue it instead
                using (var db = new tradelrDataContext())
                {
                    db.mails.InsertOnSubmit(entry);
                    db.SubmitChanges();
                }
                return;
            }

            var from = new MailAddress(MAIL_SOURCE_ADDRESS, "tradelr", Encoding.UTF8);
            MailAddress replyto = null;
            if (!string.IsNullOrEmpty(entry.fromEmail))
            {
                replyto = new MailAddress(entry.fromEmail, entry.fromName, Encoding.UTF8);
            }
            else
            {
                entry.fromEmail = MAIL_SOURCE_ADDRESS;
                entry.fromName = "tradelr";
            }
            var to = new MailAddress(entry.toEmail, entry.toName, Encoding.UTF8);
            var msg = new MailMessage(from, to)
                                  {
                                      Body = entry.body,
                                      IsBodyHtml = true,
                                      BodyEncoding = Encoding.UTF8,
                                      Subject = entry.subject,
                                      SubjectEncoding = Encoding.UTF8
                                  };
            if (replyto != null)
            {
                msg.ReplyToList.Add(replyto);
            }

#if DEBUG
            var smtp = new SmtpClient(MAIL_SERVER, 587) {EnableSsl = true};
            var cred = new NetworkCredential("tradelr.com@gmail.com", MAIL_PASSWORD);
            //smtp.Timeout = 10000;
#else
            SmtpClient smtp = new SmtpClient(MAIL_SERVER);
            NetworkCredential cred = new NetworkCredential(MAIL_SOURCE_ADDRESS, MAIL_PASSWORD);
#endif
            new Thread(() =>
                           {
                               smtp.Credentials = cred;
                               if (isAsync)
                               {
                                   smtp.SendCompleted += SendCompletedCallback;
                                   smtp.SendAsync(msg, entry);
                               }
                               else
                               {
                                   try
                                   {
                                       smtp.Send(msg);
                                   }
                                   catch (Exception ex)
                                   {
                                       Syslog.Write(ex);
                                   }
                               }
                           }).Start();
        }

        public static void SendMail(string toName, string toEmail, 
            string subject, string body, user sender, bool queueMail)
        {
            if (toName == null)
            {
                toName = "";
            }
            
            var entry = new mail
            {
                fromName = sender == null ? "tradelr.com": sender.organisation1.name,
                fromEmail = sender == null ? "" : sender.ToEmail(),
                toName = toName,
                toEmail = toEmail,
                subject = subject,
                body = body,
                reference = sender == null ? "": sender.id.ToString()
            };
            SendMail(entry, true, queueMail);
        }

        private static void SendCompletedCallback(object sender, AsyncCompletedEventArgs e)
        {
            // get the identifier
            var token = (mail)e.UserState;
            if (e.Error != null)
            {
                Syslog.Write(e.Error);
                if (e.Error.GetType() != typeof(SmtpFailedRecipientException))
                {
                    // reinsert back into database
                    using (var db = new tradelrDataContext())
                    {
                        db.mails.InsertOnSubmit(token);
                        db.SubmitChanges();
                    }
                }
            }

            // update order status
            //ITradelrRepository repository = new TradelrRepository();
            //repository.UpdateOrderStatus(token.orderID, token.userID, OrderStatus.SENT);

        }
    }

    
}
