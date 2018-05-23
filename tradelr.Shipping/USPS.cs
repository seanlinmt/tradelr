using System;
using System.Text;
using System.Xml;
using System.IO;
using System.Net;

namespace Shipping
{
    public class USPS
    {

        private string m_userID;
        private string m_uri = "http://Production.ShippingAPIs.com/ShippingAPI.dll";

        #region Properties
        public string UserID
        {
            get { return m_userID; }
            set { m_userID = value; }
        }
        #endregion


        public USPS(string userID)
        {
            m_userID = userID;
        }

        public decimal GetRate(string weight, string shipperZipCode, string destinationZipCode, string serviceType)
        {
            WebRequest req = WebRequest.Create(m_uri);
            WebResponse rsp;
            req.Method = "POST";
            req.ContentType = "text/xml";
            StreamWriter writer = new StreamWriter(req.GetRequestStream());
            writer.WriteLine(BuildUSPSXml(weight, shipperZipCode, destinationZipCode , serviceType));
            writer.Close();
            rsp = req.GetResponse();

            //Get the Posted Data
            StreamReader reader = new StreamReader(rsp.GetResponseStream());
            string rawXml = reader.ReadToEnd();
            XmlDocument xml = new XmlDocument();
            xml.LoadXml(rawXml);
          
            decimal rate = 0;

            if (xml.SelectSingleNode(@"/RateV3Response/Package/Error/Number") == null)
            {
                //no error return the rate
                rate = Convert.ToDecimal(xml.SelectSingleNode(@"/RateV3Response/Package/Postage/Rate").InnerText);
            }

            return rate;

        }


        public string BuildUSPSXml(string weight, string shipperZipCode, string destinationZipCode, string serviceType)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("API=RateV3&XML=<RateV3Request USERID=\'").Append(m_userID).Append("\'>");
            sb.Append("<Package ID=\'1st\'>");
            sb.Append("   <Service>").Append(serviceType).Append("</Service>");
            sb.Append("   <ZipOrigination>").Append(shipperZipCode).Append("</ZipOrigination>");
            sb.Append("   <ZipDestination>").Append(destinationZipCode).Append("</ZipDestination>");
            sb.Append("   <Pounds>").Append(weight).Append("</Pounds>");
            sb.Append("   <Ounces>0</Ounces>");
            sb.Append("   <Size>REGULAR</Size>");
            sb.Append("   <Machinable>TRUE</Machinable>");
            sb.Append("</Package>");
            sb.Append("</RateV3Request>");
            return sb.ToString();

        }

    }
}
