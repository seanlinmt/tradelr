using System;
using System.IO;
using System.Net;
using System.Text;
using System.Web.Security;
using System.Xml;
using System.Xml.Serialization;
using clearpixels.Logging;
using tradelr.Library.Constants;

namespace OpenSRS
{
    public class OpenSRSService
    {
#if DEBUG
        private const string URL_BASE = "https://horizon.opensrs.net:55443";
        private const string NAMESERVER1 = "default.opensrs.org";
        private const string NAMESERVER2 = "default2.opensrs.org";
#else
        private const string URL_BASE = "https://rr-n1-tor.opensrs.net:55443";
        private const string NAMESERVER1 = "ns1.opensrs.net";
        private const string NAMESERVER2 = "ns2.opensrs.net";
#endif

        protected const string RSP_USERNAME = "seanlinmt";
        protected const string RSP_PASSWORD = "tuaki1976";
#if DEBUG
        private const string PRIVATE_KEY = "1be9b34f47588a8abac2af5cc1fc784aad0cc4e68603dd70d4da5f5b1af41be587a351b2738a41c26a195c99b03dcee5923b8b2974760f96";
#else
        private const string PRIVATE_KEY = "105d89db86eb104064a0e00e30568d3c494bfecec0afd6791f2235ed0393a4c7cd9ab7feaa9493799dc7b3468f1daef6a65092961c2cf948";
#endif

        public static Contact TechnicalContact = new Contact("Sean", "Lin", "Clear Pixels Limited",
                                              "24 Malabar Drive", "Auckland City", "Auckland",
                                              "1051", "NZ", "+60.165760616", "",
                                              "administrator@tradelr.com");

        protected object responseObject { get; set; }
        protected string ipaddress { get; set; }
        
        public OpenSRSService()
        {
            ipaddress = GeneralConstants.SERVER_IP;
        }

        protected void LogResponseMessage()
        {
            var xml = Serialize(responseObject);
            Syslog.Write(xml);
        }

        protected void SendRequest(string postData)
        {
            postData =
                string.Format(
                    @"<?xml version='1.0' encoding='UTF-8' standalone='no' ?>
                        <!DOCTYPE OPS_envelope SYSTEM 'ops.dtd'>{0}",
                    postData);

            var request = WebRequest.Create(URL_BASE);

            request.Method = "POST";
            request.ContentType = "text/xml";
            request.Headers.Add("X-Username", RSP_USERNAME);
            request.Headers.Add("X-Signature", md5Hash(
                        md5Hash(postData + PRIVATE_KEY) + PRIVATE_KEY));

            byte[] dataArray = Encoding.ASCII.GetBytes(postData);

            request.ContentLength = dataArray.Length;

            Stream dataStream = request.GetRequestStream();
            dataStream.Write(dataArray, 0, dataArray.Length);
            dataStream.Close();

            WebResponse response = request.GetResponse();
            dataStream = response.GetResponseStream();
            var serializer = new XmlSerializer(typeof(MessageEnvelope));

            responseObject = serializer.Deserialize(dataStream);
        }

        protected string Serialize(object obj)
        {
            var xsn = new XmlSerializerNamespaces();
            xsn.Add("", "");
            var xws = new XmlWriterSettings();
            xws.OmitXmlDeclaration = true;
            var builder = new StringBuilder();

            var serializer = new XmlSerializer(typeof(MessageEnvelope));
            serializer.Serialize(XmlWriter.Create(builder, xws), obj, xsn);

            return builder.ToString();
        }

        //Used to convert to MD5
        private string md5Hash(string str)
        {
            //Must have Imports System.Web.Security in General Declarations
            string hash = FormsAuthentication.HashPasswordForStoringInConfigFile(str, "MD5");
            return hash.ToLower();
        }
    }
}
