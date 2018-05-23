using System;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Xml.Serialization;
using tradelr.Common;
using tradelr.Library;
using tradelr.Logging;

namespace ColourLovers
{
    public class ColourLoverService
    {
        public ColourLoverService()
        {
            parameters = new NameValueCollection();
        }
        private const string ApiUrl = "http://www.colourlovers.com/api/{0}/{1}";
        private readonly NameValueCollection parameters;
        private string requestUrl;

        public T GetData<T>(ApiType api, UrlType type, string sortBy = "", int? numResults = null, int? resultOffset = null) where T : class
        {
            if (numResults.HasValue)
            {
                parameters.Add("numResults", numResults.Value.ToString());
            }

            if (resultOffset.HasValue)
            {
                parameters.Add("resultOffset", resultOffset.Value.ToString());
            }

            if (sortBy == "DESC")
            {
                parameters.Add("sortBy", sortBy);
            }

            requestUrl = string.Concat(string.Format(ApiUrl, api.ToDescriptionString(), type.ToDescriptionString()), parameters.ToQueryString(true));

            var xmlString = PostRequest();

            if (string.IsNullOrEmpty(xmlString))
            {
                return default(T);
            }
            T response;
            using (var reader = new StringReader(xmlString))
            {
                var serializer = new XmlSerializer(typeof(T));
                response = serializer.Deserialize(reader) as T;
            }

            return response;
        }

        private string PostRequest()
        {
            WebRequest req = WebRequest.Create(requestUrl);
            req.Method = "GET";
            try
            {
                var resp = req.GetResponse();
                using (var sr = new StreamReader(resp.GetResponseStream()))
                {
                    return sr.ReadToEnd();
                }
            }
            catch (Exception ex)
            {
                Syslog.Write(ex);
            }
            return "";
        }
    }
}
