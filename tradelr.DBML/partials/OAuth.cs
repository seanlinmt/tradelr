using System;
using System.Linq;
using clearpixels.OAuth;
using clearpixels.Logging;

namespace tradelr.DBML
{
    public partial class TradelrRepository
    {
        private void DeleteOAuthTokenIfExpired(oauth_token token)
        {
            if (token != null && token.expires.HasValue && DateTime.UtcNow > token.expires.Value)
            {
                db.oauth_tokens.DeleteOnSubmit(token);
                db.SubmitChanges();
            }
        }

        public oauth_token GetOAuthToken(long subdomainid, OAuthTokenType type, bool? authorised = null)
        {
            var token = db.oauth_tokens.Where(x => x.subdomainid == subdomainid && x.type == type.ToString());
            if (authorised.HasValue)
            {
                token = token.Where(x => x.authorised == authorised.Value);
            }

            var result = token.SingleOrDefault();

            DeleteOAuthTokenIfExpired(result);

            return result;
        }

        public oauth_token GetOAuthToken(string oauth_token, OAuthTokenType type)
        {
            var result = db.oauth_tokens.SingleOrDefault(x => x.token_key == oauth_token && x.type == type.ToString());
            
            DeleteOAuthTokenIfExpired(result);

            return result;
        }

        public oauth_token GetOAuthToken(long subdomainid, string appid, OAuthTokenType type)
        {
            var result = db.oauth_tokens.SingleOrDefault(x => x.subdomainid == subdomainid &&
                                                              x.appid == appid && x.type == type.ToString());

            DeleteOAuthTokenIfExpired(result);

            return result;
        }

        /// <summary>
        /// checks if there is an existing entry by subdomainid, sessionid and type
        /// </summary>
        /// <param name="oauthToken"></param>
        public void AddOAuthToken(oauth_token oauthToken)
        {
            // if exist then just update it
            var exist = db.oauth_tokens.SingleOrDefault(x => x.type == oauthToken.type && 
                                                             x.subdomainid == oauthToken.subdomainid && 
                                                             x.appid == oauthToken.appid);
            if (exist != null)
            {
                exist.token_key = oauthToken.token_key;
                exist.token_secret = oauthToken.token_secret;
                exist.expires = oauthToken.expires;
                exist.authorised = oauthToken.authorised;
            }
            else
            {
                db.oauth_tokens.InsertOnSubmit(oauthToken);
            }
            db.SubmitChanges();
        }

        public void DeleteOAuthToken(long subdomainid, OAuthTokenType type)
        {
            var oauth =
                db.oauth_tokens.SingleOrDefault(x => x.subdomainid == subdomainid && x.type == type.ToString());
            if (oauth == null)
            {
                Syslog.Write("Can't find {0} token for subdomainid {1}", type, subdomainid);
                return;
            }
            db.oauth_tokens.DeleteOnSubmit(oauth);
            db.SubmitChanges();
        }
    }
}