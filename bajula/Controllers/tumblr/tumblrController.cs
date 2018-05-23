using System;
using System.Web.Mvc;
using tradelr.Crypto;
using tradelr.DBML;
using tradelr.Libraries.ActionFilters;
using tradelr.Library.JSON;
using clearpixels.Logging;
using tradelr.Models.users;

namespace tradelr.Controllers.tumblr
{
    //[ElmahHandleError]
    [RoleFilter(role = UserRole.USER)]
    [PermissionFilter(permission = UserPermission.NETWORK_SETTINGS)]
    public class tumblrController : baseController
    {
        [HttpPost]
        public ActionResult Connected()
        {
            var tumblr = MASTERdomain.tumblrSites;
            if (tumblr != null)
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
        public ActionResult Credentials(string email, string password, string url)
        {
            var tumblr = MASTERdomain.tumblrSites;
            var cryptor = new AESCrypt();
            if (tumblr == null)
            {
                tumblr = new tumblrSite() { subdomainid = subdomainid.Value };
                repository.AddTumblr(tumblr);
            }

            tumblr.email = email;
            tumblr.password = cryptor.Encrypt(password, subdomainid.Value.ToString());

            // need to extract url
            string accountname = "";
            Uri address;
            try
            {
                if (url.IndexOf('.') == -1)
                {
                    // user only enter store name
                    url = string.Format("{0}.tumblr.com", url);
                }
                if (!url.StartsWith("http"))
                {
                    // user did not enter http
                    url = string.Format("http://{0}", url);
                }
                address = new Uri(url);
            }
            catch (Exception ex)
            {
                Syslog.Write(ex);
                return SendJsonErrorResponse("Please enter a valid blog URL");
            }

            if (address.Host.Split('.').Length > 2)
            {
                int index = address.Host.IndexOf(".");
                accountname = address.Host.Substring(0, index);
            }
            else
            {
                return SendJsonErrorResponse("Please enter a valid blog URL");
            }

            tumblr.accountname = accountname;
            repository.Save();
            return Json("Credentials saved".ToJsonOKMessage());
        }

        [HttpPost]
        public ActionResult Clear()
        {
            var tumblr = MASTERdomain.tumblrSites;
            if (tumblr != null)
            {
                repository.DeleteTumblr(tumblr);
            }

            return Json(true.ToJsonOKData());
        }
    }
}
