using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using clearpixels.OAuth;
using tradelr.DBML.Helper;
using tradelr.DBML;
using tradelr.Email.Models;
using tradelr.Library;
using tradelr.Library.Constants;
using tradelr.Library.JSON;
using clearpixels.Logging;
using user = tradelr.DBML.user;

namespace tradelr.Models.message
{
    public class Message
    {
        public long id { get; set; }
        public long subdomainid { get; set; }

        public user sender { get; set; }
        public long recipientid { get; set; }
        public string recipientname { get; set; }
        public string body { get; set; }
        public string title { get; set; }

        // destinations
        public long destFbid { get; set; }
        public string destTwitter { get; set; }
        public string destEmail { get; set; }

        public DateTime created { get; set; }
        public bool read { get; set; }
        public TransportType transportType { get; set; }
        public MessageType messageType { get; set; }

        public Message()
        {
            
        }

        public Message(user targetuser, user sender, long subdomainid)
        {
            if (!string.IsNullOrEmpty(targetuser.email))
            {
                transportType = TransportType.EMAIL;
                destEmail = targetuser.email;
            }
            else if (!string.IsNullOrEmpty(targetuser.proxiedEmail))
            {
                transportType = TransportType.EMAIL;
                destEmail = targetuser.proxiedEmail;
            }
            else if (!string.IsNullOrEmpty(targetuser.FBID))
            {
                transportType = TransportType.FACEBOOK;
                destFbid = long.Parse(targetuser.FBID);
            }
            this.sender = sender;
            this.subdomainid = subdomainid;
            recipientid = targetuser.id;
            recipientname = targetuser.ToFullName();
        }

        public ErrorData SendMessage(Controller controller, ITradelrRepository repository, EmailViewType emailtype, 
                    object viewData, string subject, string viewloc = "", MessageType msgtype = MessageType.EMAIL)
        {
            // save notification types
            switch (msgtype)
            {
                case MessageType.LINKREQUEST:
                    var message = new DBML.message
                                      {
                                          sender = sender.id,
                                          recipient = recipientid,
                                          title = subject,
                                          body = "",
                                          readStatus = false,
                                          senderDeleted = false,
                                          recipientDeleted = false,
                                          created = DateTime.UtcNow,
                                          type = MessageType.LINKREQUEST.ToString()
                                      };
                    repository.AddMessage(message);
                    break;
                default:
                    break;
            }

            ////// send message
            var error = new ErrorData();
            if (controller != null)
            {
                body = controller.RenderViewToString(emailtype.ToDescriptionString(), viewData);
            }
            else
            {
                throw new NotImplementedException();
            }
            
            // wrap body with div to format it
            body = string.Concat("<div style=\"font: 12.5px/20px arial, sans-serif;\"", body, "</div>");

            // subject may contain HTML tags
            subject = subject.StripHtmlTags();

            switch (transportType)
            {
                case TransportType.EMAIL:
                    Email.Email.SendMail(recipientname, destEmail, subject, body, sender, true);
                    break;
                case TransportType.FACEBOOK:
                    Syslog.Write("fbuser no email");
                    error.success = false;
                    error.message = "Facebook contact did not grant email permission";
                    break;
            }
            return error;
        }
    }

    public static class MessageHelper
    {
        public static IEnumerable<Message> ToModel(this IQueryable<DBML.message> values)
        {
            foreach (var value in values)
            {
                yield return new Message()
                {
                    id = value.id,
                    body = value.body,
                    created = value.created,
                };
            }
        }
    }
}
