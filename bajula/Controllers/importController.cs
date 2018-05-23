using System.Linq;
using System.Web.Mvc;
using clearpixels.OAuth;
using Google.GData.Client;
using tradelr.Common.Constants;
using tradelr.Libraries.ActionFilters;
using tradelr.Library;
using tradelr.Library.Constants;
using tradelr.Library.JSON;
using clearpixels.Logging;
using tradelr.Models.contacts;
using tradelr.Models.opensocial;
using tradelr.Models.users;
using HttpUtility = System.Web.HttpUtility;

namespace tradelr.Controllers
{
    
    //[ElmahHandleError]
    [RoleFilter(role = UserRole.CREATOR)]
    public class importController : baseController
    {
        private ActionResult Index()
        {
            return View(baseviewmodel);
        }

        public ActionResult googleContacts()
        {
            var continueUrl = string.Concat(GeneralConstants.HTTP_HOST, "/callback?sd=", accountHostname, "&path=",
                                            HttpUtility.UrlEncode("/dashboard/contacts/import"),
                                            "&type=", ContactImportType.GOOGLE);
            var authsubUrl = AuthSubUtil.getRequestUrl(continueUrl, GoogleConstants.FEED_CONTACTS, false, false);

            return Redirect(authsubUrl);
        }

        public ActionResult yahooContacts()
        {
            var oauth = repository.GetOAuthToken(subdomainid.Value, OAuthTokenType.YAHOO, true);
            if (oauth == null)
            {
                Syslog.Write("Unable to find oauth_token for yahoo contacts import, domain:" + accountHostname + " " + sessionid.Value);
                return Json("Not authorized".ToJsonFail());
            }

            var opensocial = new OpenSocialService(ClientType.YAHOO, oauth.token_key, oauth.token_secret, oauth.guid);
            var contacts = opensocial.FetchContacts();
            if (contacts == null)
            {
                return Json("Unable to retrieve contacts".ToJsonFail());
            }

            var viewdata = contacts.ToModel().OrderBy(x => x.companyName);

            var view = this.RenderViewToString("~/Areas/dashboard/Views/contacts/contactList.ascx", viewdata);
            return Json(view.ToJsonOKData());
        }
    }
}
