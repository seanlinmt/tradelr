using System;
using System.Threading;
using System.Web.Mvc;
using clearpixels.OAuth;
using Ebay;
using tradelr.DBML;
using tradelr.Library;
using tradelr.Library.Constants;
using clearpixels.Logging;
using tradelr.Models.networks;
using tradelr.Models.subdomain;

namespace tradelr.Controllers.oauth
{
    public class oauthController : Controller
    {
        private string accountHostName;

        /// <summary>
        /// processes callback after user authorisation
        /// </summary>
        /// <param name="oauth_token"></param>
        /// <param name="oauth_verifier"></param>
        /// <returns></returns>
        public ActionResult Yahoo(string oauth_token, string oauth_verifier)
        {
            if (!saveToken(OAuthTokenType.YAHOO, oauth_token, oauth_verifier))
            {
                return Redirect("/Error");
            }

            return View("close");
        }

        public ActionResult Ebay(string username, string sid)
        {
            using (var repository = new TradelrRepository())
            {
                var odb = repository.GetOAuthToken(sid, OAuthTokenType.EBAY);

                if (odb == null)
                {
                    throw new Exception("Could not locate ebay token entry");
                }

                try
                {
                    var ebayservice = new EbayService();
                    var token = ebayservice.GetToken(odb.token_key);

                    odb.token_key = token;
                    odb.token_secret = "";
                    odb.guid = username;
                    odb.authorised = true;
                    odb.expires = ebayservice.TokenExpires;
                    repository.Save();

                    // sync with ebay
                    var ebay = new NetworksEbay(odb.MASTERsubdomain.id);
                    new Thread(() => ebay.StartSynchronisation(false)).Start();
                }
                catch (Exception ex)
                {
                    Syslog.Write(ex);
                }

                return Redirect(odb.MASTERsubdomain.ToHostName().ToDomainUrl("/dashboard/networks#ebay"));
            }
        }

        public ActionResult TradeMe(string oauth_token, string oauth_verifier)
        {
            if (!saveToken(OAuthTokenType.TRADEME, oauth_token, oauth_verifier))
            {
                return Redirect("/Error");
            }

            return Redirect(accountHostName.ToDomainUrl("/dashboard/networks#trademe"));
        }

        private bool saveToken(OAuthTokenType tokenType, string oauth_token, string oauth_verifier)
        {
            using (var repository = new TradelrRepository())
            {
                // oauth_token here would match request token saved in db
                // normally access token is different from request token
                var odb = repository.GetOAuthToken(oauth_token, tokenType);
                if (odb == null)
                {
                    return false;
                }

                accountHostName = odb.MASTERsubdomain.ToHostName();

                OAuthClient client = null;
                switch (tokenType)
                {
                    case OAuthTokenType.YAHOO:
                        client = new OAuthClient(tokenType, 
                            OAuthClient.OAUTH_YAHOO_CONSUMER_KEY,
                            OAuthClient.OAUTH_YAHOO_CONSUMER_SECRET, 
                            odb.token_key,
                            odb.token_secret,
                            oauth_verifier);
                        break;
                    case OAuthTokenType.TRADEME:
                        client = new OAuthClient(tokenType, OAuthClient.OAUTH_TRADEME_CONSUMER_KEY,
                            OAuthClient.OAUTH_TRADEME_CONSUMER_SECRET, 
                            odb.token_key,
                            odb.token_secret,
                            oauth_verifier,
                            "HMAC-SHA1", 
                            "MyTradeMeRead,MyTradeMeWrite");
                        break;
                    default:
                        break;
                }

                if (client == null ||
                    !client.GetAccessToken())
                {
                    return false;
                }

                odb.token_key = client.oauth_token;
                odb.token_secret = client.oauth_secret;
                odb.guid = client.guid;
                odb.authorised = true;
                repository.Save();
            }
            return true;
        }
    }
}
