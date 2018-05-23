using System;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;
using System.Web.Script.Serialization;
using clearpixels.Facebook.Helpers;
using clearpixels.Facebook.Resources;
using clearpixels.Logging;

namespace clearpixels.Facebook
{
    public class RestBase
    {
        protected string method { get; set; }
        protected string filePath { get; set; }

        private JavaScriptSerializer serializer { get; set; }
        private string accesstoken { get; set; }
        private ResponseError respError { get; set; }

        protected RestBase(string token)
        {
            accesstoken = token;
            serializer = new JavaScriptSerializer();
        }

        public Error GetError()
        {
            if (respError == null)
            {
                return null;
            }
            return respError.error;
        }

        protected T SendRequest<T>(string path, NameValueCollection parameters = null, bool isMultiPart = false) where T : class 
        {
            WebRequest req;
            var requestUrl = string.Format("https://graph.facebook.com/{0}?access_token={1}", path, accesstoken);
            if (method == "POST" || method == "PUT" || method == "DELETE")
            {
                if (isMultiPart)
                {
                    var boundaryid = DateTime.Now.Ticks.ToString("x");
                    byte[] boundarybytes = Encoding.ASCII.GetBytes("\r\n--" + boundaryid + "\r\n");

                    req = WebRequest.Create(requestUrl);
                    req.ContentType = "multipart/form-data; boundary=" + boundaryid;
                    req.Method = "POST";

                    Stream rs = req.GetRequestStream();

                    Debug.Assert(parameters != null);
                    const string formdataTemplate = "Content-Disposition: form-data; name=\"{0}\"\r\n\r\n{1}";
                    foreach (string key in parameters.Keys)
                    {
                        rs.Write(boundarybytes, 0, boundarybytes.Length);
                        string formitem = string.Format(formdataTemplate, key, parameters[key]);
                        byte[] formitembytes = Encoding.UTF8.GetBytes(formitem);
                        rs.Write(formitembytes, 0, formitembytes.Length);
                    }
                    rs.Write(boundarybytes, 0, boundarybytes.Length);

                    const string headerTemplate = "Content-Disposition: file; name=\"{0}\"; filename=\"{1}\"\r\nContent-Type: {2}\r\n\r\n";
                    string header = string.Format(headerTemplate, "image", DateTime.UtcNow.Ticks, "multipart/form-data");
                    byte[] headerbytes = Encoding.UTF8.GetBytes(header);
                    rs.Write(headerbytes, 0, headerbytes.Length);

                    FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
                    byte[] buffer = new byte[4096];
                    int bytesRead = 0;
                    while ((bytesRead = fileStream.Read(buffer, 0, buffer.Length)) != 0)
                    {
                        rs.Write(buffer, 0, bytesRead);
                    }
                    fileStream.Close();

                    byte[] trailer = Encoding.ASCII.GetBytes("\r\n--" + boundaryid + "--\r\n");
                    rs.Write(trailer, 0, trailer.Length);

                    rs.Close();
                }
                else
                {
                    req = WebRequest.Create(requestUrl);
                    req.Method = method;
                    using (var sw = new StreamWriter(req.GetRequestStream()))
                    {
                        sw.Write(parameters.ToQueryString(true).Substring(1)); // skips the ?
                    }
                }
            }
            else
            {
                if (parameters != null)
                {
                    requestUrl = string.Concat(requestUrl, "&", parameters.ToQueryString(true).Substring(1));
                }
                req = WebRequest.Create(requestUrl);
                req.Method = "GET";
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
                    return serializer.Deserialize<T>(jsonString);
                }
            }
            catch (WebException ex)
            {
                // handle any errors
                resp = ex.Response;
                if (resp != null)
                {
                    using (var sr = new StreamReader(resp.GetResponseStream()))
                    {
                        var errorString = sr.ReadToEnd();
                        Syslog.Write("Facebook Error: " + requestUrl + " " + errorString);
                        respError = serializer.Deserialize<ResponseError>(errorString);
                    }
                }
            }
            catch (Exception ex)
            {
                Syslog.Write(string.Format("{0}:{1}", ex.Message, jsonString));
            }

            return default(T);
        }
    }
}
