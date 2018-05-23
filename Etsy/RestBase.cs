using System;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;
using System.Web.Script.Serialization;
using Google.GData.Client;
using clearpixels.Logging;
using AuthenticationException = System.Security.Authentication.AuthenticationException;

namespace Etsy
{
    public class RestBase
    {
        private Uri requestUrl;
        private bool isPrivate;

        private readonly JavaScriptSerializer serializer;

        // all parameters
        Parameters parameters;
        
        // consist of the following parameter types
        protected string method;
        protected string URI;
        protected readonly Identifier id;
        protected string filePath;

        protected SessionInfo info;

        protected RestBase()
        {
            id = new Identifier();
            serializer = new JavaScriptSerializer();
        }

        private string GetAction(string uri)
        {
            var result = uri.Replace(":shop_id", id.shop_id)
                .Replace(":listing_id", id.listing_id)
                .Replace(":tag", id.tag_id)
                .Replace(":subtag", id.subtag_id);
            if ((result.Contains(":user_id") && string.IsNullOrEmpty(id.user_id)) ||
                method == "POST" || method == "PUT" || method == "DELETE")
            {
                isPrivate = true;
                id.user_id = "__SELF__";
                result = string.Concat("private", result.Replace(":user_id", id.user_id));
            }
            else
            {
                isPrivate = false;
                parameters.AddParameter("api_key", Constants.ApplicationKey);
                result = string.Concat("public", result.Replace(":user_id", id.user_id));
            }
            return result;
        }

        

        protected ResponseData<T> SendRequest<T>(Parameters prms = null, bool isMultiPart = false)
        {
            parameters = prms ?? new Parameters();

            // build the uri
            var actionUrl = GetAction(URI);

            WebRequest req;
            if (method == "POST" || method =="PUT" || method == "DELETE")
            {
                requestUrl = new Uri(string.Concat(Constants.BaseUrl, actionUrl));
                if (isMultiPart)
                {
                    string boundary = "---------------------------" + DateTime.Now.Ticks.ToString("x");
                    byte[] boundarybytes = Encoding.ASCII.GetBytes("\r\n--" + boundary + "\r\n");

                    req = WebRequest.Create(requestUrl);
                    req.ContentType = "multipart/form-data; boundary=" + boundary;
                    req.Method = "POST";

                    Stream rs = req.GetRequestStream();

                    const string formdataTemplate = "Content-Disposition: form-data; name=\"{0}\"\r\n\r\n{1}";
                    foreach (string key in parameters.parameters.Keys)
                    {
                        rs.Write(boundarybytes, 0, boundarybytes.Length);
                        string formitem = string.Format(formdataTemplate, key, parameters.parameters[key]);
                        byte[] formitembytes = Encoding.UTF8.GetBytes(formitem);
                        rs.Write(formitembytes, 0, formitembytes.Length);
                    }
                    rs.Write(boundarybytes, 0, boundarybytes.Length);

                    const string headerTemplate = "Content-Disposition: form-data; name=\"{0}\"; filename=\"{1}\"\r\nContent-Type: {2}\r\n\r\n";
                    string header = string.Format(headerTemplate, "image", DateTime.UtcNow.Ticks, "multipart/form-data");
                    byte[] headerbytes = Encoding.UTF8.GetBytes(header);
                    rs.Write(headerbytes, 0, headerbytes.Length);

                    var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
                    byte[] buffer = new byte[4096];
                    int bytesRead = 0;
                    while ((bytesRead = fileStream.Read(buffer, 0, buffer.Length)) != 0)
                    {
                        rs.Write(buffer, 0, bytesRead);
                    }
                    fileStream.Close();

                    byte[] trailer = Encoding.ASCII.GetBytes("\r\n--" + boundary + "--\r\n");
                    rs.Write(trailer, 0, trailer.Length);
                    rs.Close();
                }
                else
                {
                    req = WebRequest.Create(requestUrl);
                    req.Method = method;
                    using (var sw = new StreamWriter(req.GetRequestStream()))
                    {
                        sw.Write(parameters.ToQueryString().Substring(1)); // skips the ?
                    }
                }
                
            }
            else
            {
                requestUrl = new Uri(string.Concat(Constants.BaseUrl, actionUrl, parameters.ToQueryString()));
                req = WebRequest.Create(requestUrl);
                req.Method = "GET";
            }

            if (isPrivate)
            {
                if (!string.IsNullOrEmpty(info.oauth_key) && !string.IsNullOrEmpty(info.oauth_secret))
                {
                    req.Headers.Add(OAuthUtil.GenerateHeader(requestUrl, Constants.ApplicationKey, Constants.ApplicationSecret, info.oauth_key, info.oauth_secret, method));
                }
                else
                {
                    throw new AuthenticationException("etsy not authenticated: " + requestUrl);
                }
            }

            string jsonString = "";
            WebResponse resp;
            try
            {
                resp = req.GetResponse();
                
                using (var sr = new StreamReader(resp.GetResponseStream()))
                {
                    jsonString = sr.ReadToEnd();
                }

                if (!string.IsNullOrEmpty(jsonString))
                {
                    return serializer.Deserialize<ResponseData<T>>(jsonString);
                }
            }
            catch (WebException ex)
            {
                resp = ex.Response;
                if (resp != null)
                {
                    using (var sr = new StreamReader(resp.GetResponseStream()))
                    {
                        var error = sr.ReadToEnd();
                        Syslog.Write("Etsy Error: " + requestUrl + " " + error);
                    }
                }
            }
            catch (Exception ex)
            {
                Syslog.Write(string.Format("{0}:{1}", ex.Message, jsonString));
            }

            return null;
        }
    }
}
