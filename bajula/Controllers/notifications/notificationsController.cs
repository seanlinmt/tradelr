using System.Web.Mvc;

using tradelr.Libraries.ActionFilters;
using tradelr.Models.notifications;
using tradelr.Models.users;

namespace tradelr.Controllers.notifications
{
    //[ElmahHandleError]
    [RoleFilter(role = UserRole.CREATOR)]
    public class notificationsController : baseController
    {
        public ActionResult Index()
        {
            // get all notifications
            var viewdata = new NotificationViewData(baseviewmodel)
                               {
                                   notificationList = repository.GetLinkRequestNotifications(sessionid.Value).ToNotificationModel()
                               };
            
            return View(viewdata);
        }

    }
}
