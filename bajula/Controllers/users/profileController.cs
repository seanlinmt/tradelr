using System.Web.Mvc;
using tradelr.Areas.dashboard.Models.contact;
using tradelr.Common.Library.Imaging;
using tradelr.Libraries.ActionFilters;
using tradelr.Models.contacts;

namespace tradelr.Controllers.users
{
    //[ElmahHandleError]
    public class profileController : baseController
    {
        /// <summary>
        /// used by someone with no login to view their profile.
        /// Eg. Person added as a contact
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Index(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return ReturnError("empty viewid");
            }
            var profile = repository.GetUserByViewId(id);

            if (profile == null)
            {
                return ReturnError("cannot find order for viewid :" + id);
            }

            //SetSessionAuthInfo(profile.id, (UserRole)profile.role, profile.permissions, profile.created);
            var viewdata = new ContactViewModel(baseviewmodel)
                               {
                                   contact = profile.ToModel(sessionid, subdomainid.Value,Imgsize.SMALL)
                               };

            return View("~/Areas/Dashboard/Views/contacts/Show.aspx", viewdata);
        }
    }
}