using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;
using clearpixels.OAuth;
using Google.GData.Client;
using tradelr.Common.Constants;
using tradelr.Library.Constants;
using clearpixels.Logging;
using tradelr.Models.yahoo;
using HttpUtility = System.Web.HttpUtility;

namespace tradelr.Models.opensocial
{
    public class OpenSocialService
    {
        string oauth_token { get; set; }
        string oauth_secret { get; set; }
        string viewerid { get; set; }
        string url { get; set; }

        public OpenSocialService(ClientType type, string oauth_token, string oauth_secret, string viewerid)
        {
            this.oauth_token = oauth_token;
            this.oauth_secret = oauth_secret;
            this.viewerid = viewerid;

            switch (type)
            {
                case ClientType.YAHOO:
                    url = string.Format("http://social.yahooapis.com/v1/user/{0}/contacts?format=json&realm=yahooapis.com", viewerid);
                    break;
                default:
                    throw new ArgumentException(@"Unrecognised argument", "type");
            }
        }

        public Contacts FetchContacts()
        {
            var o = new OAuthBase();
            var nonce = OAuthBase.GenerateNonce();
            var timestamp = OAuthBase.GenerateTimeStamp();
            var sig = OAuthBase.GenerateSignature(new Uri(url), OAuthClient.OAUTH_YAHOO_CONSUMER_KEY,
                                          OAuthClient.OAUTH_YAHOO_CONSUMER_SECRET, oauth_token, oauth_secret, "GET",
                                          timestamp, nonce);
            sig = HttpUtility.UrlEncode(sig);
            var sb = new StringBuilder();
            sb.Append(url);
            sb.AppendFormat("&oauth_consumer_key={0}", OAuthClient.OAUTH_YAHOO_CONSUMER_KEY);
            sb.AppendFormat("&oauth_nonce={0}", nonce);
            sb.AppendFormat("&oauth_timestamp={0}", timestamp);
            sb.AppendFormat("&oauth_version={0}", "1.0");
            sb.AppendFormat("&oauth_signature_method={0}", "HMAC-SHA1");
            sb.AppendFormat("&oauth_signature={0}", sig);
            sb.AppendFormat("&oauth_token={0}", HttpUtility.UrlEncode(oauth_token));
            string content ="";
            WebResponse response;
            try
            {
                var request = WebRequest.Create(sb.ToString());
                response = request.GetResponse();
                using (StreamReader sr = new StreamReader(response.GetResponseStream()))
                {
                    content = sr.ReadToEnd();
                }
            }
            catch (WebException ex)
            {
                response = ex.Response;
                if (response != null)
                {
                    using (var sr = new StreamReader(response.GetResponseStream()))
                    {
                        var error = sr.ReadToEnd();
                        Syslog.Write(sb + " " + error);
                    }
                }
                return null;
            }
            var serializer = new JavaScriptSerializer();

            var result = serializer.Deserialize<ContactsResult>(content);

            return result.contacts;
        }
    }
}