using System.Collections.Generic;
using tradelr.Common.Library.Imaging;
using tradelr.DBML.Helper;
using tradelr.Models.contacts;
using tradelr.Models.users;

namespace tradelr.Models.notifications
{
    public class Notification
    {
        public long id { get; set; }
        public string title { get; set; }
        public string body { get; set; }

        public long senderid { get; set; }
        public string senderName { get; set; }
        public string senderOrgName { get; set; }
        public string senderProfileUrl { get; set; }
        public string senderProfilePhoto { get; set; }
    }

    public static class NotificationHelper
    {
        public static IEnumerable<Notification> ToNotificationModel(this IEnumerable<DBML.message> values)
        {
            foreach (var value in values)
            {
                yield return new Notification()
                             {
                                 id = value.id,
                                 title = value.title,
                                 body = value.body,
                                 senderid = value.sender,
                                 senderName = value.user1.ToFullName(),
                                 senderOrgName = value.user1.organisation1.name,
                                 senderProfilePhoto = value.user1.ToProfilePhoto(Imgsize.THUMB, true).url,
                                 senderProfileUrl = value.user1.ToProfileUrl()
                             };
            }
            
        }
    }
}