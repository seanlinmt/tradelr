using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Xml.Serialization;
using clearpixels.OAuth;
using Google.GData.Client;
using clearpixels.Logging;

namespace TradeMe
{
    /// <summary>
    /// http://developer.trademe.co.nz/api-documentation/
    /// </summary>
    public class RestBase
    {
        private string version = "v1";
        
#if DEBUG
        public const string CallbackUrl = "https://secure.localhost/oauth/trademe";
        private string BaseUrl = "https://api.tmsandbox.co.nz";
#else
        public const string CallbackUrl = "https://secure.tradelr.com/oauth/trademe";
        private string BaseUrl = "https://api.trademe.co.nz";
#endif


        private Uri requestUrl;

        // consist of the following parameter types
        protected string method;
        protected string action;
        protected string filePath;

        public RestBase(string oauth_key, string oauth_secret) 
        {
            this.oauth_key = oauth_key;
                this.oauth_secret = oauth_secret;
               
        }
                protected string oauth_key { get; set; }
        protected string oauth_secret { get; set; }

        private string resp_error;
        private string resp_json;
        private string resp_xml;

        protected RestBase()
        {
        }

        private string GetAction()
        {
            Debug.Assert(!string.IsNullOrEmpty(action));

            return string.Format(action, version, "XML");
        }

        protected T SendRequest<T>(object data)
        {
            // build the uri
            var actionUrl = GetAction();

            requestUrl = new Uri(string.Concat(BaseUrl, actionUrl));

            WebRequest req = WebRequest.Create(requestUrl);
            req.Method = method;
            req.ContentType = "text/xml; charset=utf-8";

            if (data != null)
            {

                var serializer = new XmlSerializer(data.GetType());

                var ms = new MemoryStream();
                serializer.Serialize(ms, data);
                ms.Seek(0, SeekOrigin.Begin);

                using (var sr = new StreamReader(ms))
                {
                    var content = sr.ReadToEnd();
                }

                serializer.Serialize(req.GetRequestStream(), data);
            }
            else
            {
                req.ContentLength = 0;
            }

            req.Headers.Add(OAuthUtil.GenerateHeader(requestUrl, OAuthClient.OAUTH_TRADEME_CONSUMER_KEY, OAuthClient.OAUTH_TRADEME_CONSUMER_SECRET, oauth_key, oauth_secret, method));
            
            WebResponse resp;
            try
            {
                resp = req.GetResponse();

                var serializer = new XmlSerializer(typeof(T), "http://api.trademe.co.nz/v1");
                return (T)serializer.Deserialize(resp.GetResponseStream()); 
 
            }
            catch (WebException ex)
            {
                resp = ex.Response;
                if (resp != null)
                {
                    using (var sr = new StreamReader(resp.GetResponseStream()))
                    {
                        resp_error = sr.ReadToEnd();

                        Syslog.Write("Trademe Error: " + requestUrl + " " + resp_error);
                    }
                }
            }
            catch (Exception ex)
            {
                Syslog.Write(string.Format("{0}:{1}", ex.Message, resp_json));
            }

            return default(T);
        }
    }
}
