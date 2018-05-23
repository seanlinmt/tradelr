using System;
using System.IO;
using System.Net;
using System.Text;
using System.Xml;
using tradelr.Library.Constants;
using clearpixels.Logging;

namespace tradelr.Libraries
{
    public static class BitlyHelper
    {
        public static string ToShortLink(this string link)
        {
            return Bitly.Bit("tradelr", GeneralConstants.BITLY_API_KEY, link, "shorten");
        }
    }

    public class Bitly
    {
        public static string Bit(string username, string apiKey, string input, string type)
        {
            string btype;
            string xtype;
            string itype;
            switch (type.ToLower())
            {
                case "shorten":
                    btype = "shorten";
                    xtype = "shortUrl";
                    itype = "longUrl";
                    break;

                case "metah":
                    btype = "info";
                    xtype = "htmlMetaDescription";
                    itype = "hash";
                    break;

                case "metau":
                    btype = "info";
                    xtype = "htmlMetaDescription";
                    itype = "shortUrl";
                    break;

                case "expandh":
                    btype = "expand";
                    xtype = "longUrl";
                    itype = "hash";
                    break;

                case "expandu":
                    btype = "expand";
                    xtype = "longUrl";
                    itype = "shortUrl";
                    break;

                case "clicksu":
                    btype = "stats";
                    xtype = "clicks";
                    itype = "shortUrl";
                    break;

                case "clicksh":
                    btype = "stats";
                    xtype = "clicks";
                    itype = "hash";
                    break;

                case "useru":
                    btype = "info";
                    xtype = "shortenedByUser";
                    itype = "shortUrl";
                    break;

                case "userh":
                    btype = "info";
                    xtype = "shortenedByUser";
                    itype = "hash";
                    break;

                default:
                    return "";
            }
            StringBuilder url = new StringBuilder();
            url.Append("http://api.bit.ly/");
            url.Append(btype);
            url.Append("?version=2.0.1");
            url.Append("&format=xml");
            url.Append("&");
            url.Append(itype);
            url.Append("=");
            url.Append(input);
            url.Append("&login=");
            url.Append(username);
            url.Append("&apiKey=");
            url.Append(apiKey);

            try
            {
                var request = WebRequest.Create(url.ToString()).GetResponse().GetResponseStream();
                StreamReader responseStream = new StreamReader(request);
                string response = responseStream.ReadToEnd();
                responseStream.Close();
                string newdata = XmlParse_general(response, xtype);
                if (newdata == "Error")
                {
                    return "";
                }
                return newdata;
            }
            catch (Exception ex)
            {
                Syslog.Write(ex);
            }
            return "";
        }

        private static string XmlParse_general(string Url, string type)
        {
            XmlTextReader xmlrt1 = new XmlTextReader(new StringReader(Url));
            while (xmlrt1.Read())
            {
                string strNodeType = xmlrt1.NodeType.ToString();
                string strName = xmlrt1.Name;
                if ((strNodeType == "Element") && (strName == type))
                {
                    xmlrt1.Read();
                    return xmlrt1.Value;
                }
            }
            return "";
        }



    }
}