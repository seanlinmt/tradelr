using System.Web.Mvc;
using tradelr.Controllers;
using tradelr.Libraries.ActionFilters;
using tradelr.Models.users;

namespace tradelr.Areas.dashboard.Controllers
{
    //[ElmahHandleError]
    [TradelrHttps]
    public class marketingController : baseController
    {
        //
        // GET: /marketing/
        [RoleFilter(role = UserRole.USER)]
        [PermissionFilter(permission = UserPermission.NETWORK_STORE)]
        public ActionResult Index()
        {
            return View(baseviewmodel);
        }
    }
}
