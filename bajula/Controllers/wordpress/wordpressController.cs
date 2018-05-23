using System.Web.Mvc;
using tradelr.Crypto;
using tradelr.DBML;
using tradelr.Libraries.ActionFilters;
using tradelr.Library.JSON;
using tradelr.Models.users;

namespace tradelr.Controllers.wordpress
{
    //[ElmahHandleError]
    [RoleFilter(role = UserRole.USER)]
    [PermissionFilter(permission = UserPermission.NETWORK_SETTINGS)]
    public class wordpressController : baseController
    {
        [HttpPost]
        public ActionResult Connected()
        {
            var wordpress = MASTERdomain.wordpressSites;
            if (wordpress != null)
            {
                return Json(true.ToJsonOKData());
            }
            return Json(false.ToJsonOKData());
        }

        [HttpGet]
        public ActionResult Credentials()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Credentials(string username, string password, string url)
        {
            var wordpress = MASTERdomain.wordpressSites;
            var cryptor = new AESCrypt();
            if (wordpress == null)
            {
                wordpress = new wordpressSite() { subdomainid = subdomainid.Value};
                repository.AddWordpress(wordpress);
            }
            
            wordpress.email = username;
            wordpress.password = cryptor.Encrypt(password, subdomainid.Value.ToString());

            // validate url
            if (!url.Contains("http://"))
            {
                url = string.Concat("http://", url);
            }
            wordpress.url = url;
            repository.Save();
            return Json("Credentials saved".ToJsonOKMessage());
        }

        [HttpPost]
        public ActionResult Clear()
        {
            var wordpress = MASTERdomain.wordpressSites;
            if (wordpress != null)
            {
                repository.DeleteWordpress(wordpress);
            }

            return Json(true.ToJsonOKData());
        }
    }
}
