using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace tradelr.Models.notifications
{
    public class NotificationViewData : BaseViewModel
    {
        public IEnumerable<Notification> notificationList { get; set; }

        public NotificationViewData(BaseViewModel viewmodel) : base(viewmodel)
        {
        }
    }
}