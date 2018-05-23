using System.Text;
using System.Web.Mvc;
using tradelr.Libraries.ActionFilters;
using tradelr.Library;
using tradelr.Library.JSON;
using tradelr.Library.geo;
using tradelr.Models.users;

namespace tradelr.Controllers.users
{
    
    //[ElmahHandleError]
    [RoleFilter(role = UserRole.USER)]
    public class orgController : baseController
    {
        public ActionResult Address(long id)
        {
            var org = repository.GetOrganisation(id);
            if (org == null)
            {
                return Content("");
            }
            StringBuilder sb = new StringBuilder();
            bool addressIncomplete = false;

            // build address
            if (!string.IsNullOrEmpty(org.address))
            {
                sb.Append(org.address.ToHtmlBreak());
                sb.Append("<br/>");
            }
            else
            {
                addressIncomplete = true;
            }

            if (org.city.HasValue)
            {
                sb.Append(org.MASTERcity.name);
            }
            else
            {
                addressIncomplete = true;
            }
            if (!string.IsNullOrEmpty(org.postcode))
            {
                sb.Append(" ");
                sb.Append(org.postcode);
                sb.Append("<br/>");
            }
            else if (org.city.HasValue)
            {
                sb.Append("<br/>");
            }
            else
            {
                addressIncomplete = true;
            }
            if (org.country.HasValue)
            {
                sb.Append(Country.GetCountry(org.country.Value).name);
                sb.Append("<br/>");
            }
            else
            {
                addressIncomplete = true;
            }

            if (org.subdomain == subdomainid.Value)
            {
                var usr = repository.GetPrimaryUser(id);
                sb.Append("<i><a href=\"/dashboard/contacts/edit/" + usr.id + "\">Edit Contact</a></i>");
            }

            return Content(sb.ToString());
        }

        [RoleFilter(role = UserRole.USER)]
        [AcceptVerbs(HttpVerbs.Post)]
        [PermissionFilter(permission = UserPermission.CONTACTS_VIEW)]
        public ActionResult List(long id)
        {
            var org = repository.GetOrganisation(id, subdomainid.Value);
            if (org == null)
            {
                return Json("Organisation not found".ToJsonFail());
            }
            return Json(org.ToModel().ToJsonOKData());
        }
    }
}
