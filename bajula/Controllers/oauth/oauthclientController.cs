using System.Web.Mvc;
using clearpixels.OAuth;
using tradelr.DBML;
using tradelr.Library;

namespace tradelr.Controllers.oauth
{
    public class oauthclientController : Controller
    {
        public ActionResult Yahoo(long subdomainid, long appid)
        {
            var client = new OAuthClient(OAuthTokenType.YAHOO, OAuthClient.OAUTH_YAHOO_CONSUMER_KEY,
                                         OAuthClient.OAUTH_YAHOO_CONSUMER_SECRET, "secure".ToTradelrDomainUrl("/oauth/yahoo"), "plaintext");

            if (!client.GetRequestToken())
            {
                return Redirect("/Error");
            }

            var oauthdb = new oauth_token
                              {
                                  token_key = client.oauth_token,
                                  token_secret = client.oauth_secret,
                                  type = OAuthTokenType.YAHOO.ToString(),
                                  subdomainid = subdomainid,
                                  appid = appid.ToString(),
                                  authorised = false
                              };

            using (var repository = new TradelrRepository())
            {
                repository.AddOAuthToken(oauthdb);
            }
            return Redirect(client.authorize_url);
        }
    }
}
