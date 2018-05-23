using System;
using System.Text;
using System.Xml;
using System.IO;
using System.Net;

namespace Shipping
{
    public class UPS
    {
        private string m_accessNumber;
        private string m_userName;
        private string m_password;
        private string m_shipperNumber;
        private string m_pickupType;
        private string m_uri = "https://www.ups.com/ups.app/xml/Rate?";

        #region Properties

        public string AccessNumber
        {
            get { return m_accessNumber; }
            set { m_accessNumber = value; }
        }

        public string UserName
        {
            get { return m_userName; }
            set { m_userName = value; }
        }

        public string Password
        {
            get { return m_password; }
            set { m_password = value; }
        }

        public string ShipperNumber
        {
            get { return m_shipperNumber; }
            set { m_shipperNumber = value; }
        }

        public string Uri
        {
            get { return m_uri; }
            set { m_uri = value; }
        }

        #endregion

        #region Classes
        public UPS()
        {
        }

        public UPS(string accessNumber, string userName, string password, string shipperNumber, string pickupType)
        {
            m_accessNumber = accessNumber;
            m_userName = userName;
            m_password = password;
            m_shipperNumber = shipperNumber;
            m_pickupType = pickupType;
        }

        #endregion

        #region Methods

        public void setCredentials(string accessNumber, string userName, string password, string shipperNumber)
        {
            m_accessNumber = accessNumber;
            m_userName = userName;
            m_password = password;
            m_shipperNumber = shipperNumber;
            
        }


        /// <summary>
        /// Function returns the rate
        /// </summary>
        /// <param name="weight"></param>
        /// <param name="shipperZipCode"></param>
        /// <param name="shipperCountryCode"></param>
        /// <param name="destinationZipCode"></param>
        /// <param name="destinationCountryCode"></param>
        /// <param name="serviceCode"></param>
        /// <returns></returns>
        public decimal GetRate(string weight, string shipperZipCode, string shipperCountryCode, string destinationZipCode, string destinationCountryCode, string serviceCode)
        {
            
            WebRequest req = WebRequest.Create(m_uri);
            WebResponse rsp;
            req.Method = "POST";
            req.ContentType = "text/xml";
            StreamWriter writer = new StreamWriter(req.GetRequestStream());
            writer.WriteLine(buildUPSXml(weight, shipperZipCode, shipperCountryCode, destinationZipCode, destinationCountryCode, serviceCode));
            writer.Close();
            rsp = req.GetResponse();

            //Get the Posted Data
            StreamReader reader = new StreamReader(rsp.GetResponseStream());
            string rawXml = reader.ReadToEnd();
            XmlDocument xml = new XmlDocument();
            xml.LoadXml(rawXml);
            decimal rate = 0;
            //Use XPath to Navigate the Document
            int status = Convert.ToInt32(xml.SelectSingleNode(@"/RatingServiceSelectionResponse/Response/ResponseStatusCode").InnerText);
            if (status == 1)
            {
                //success
                rate = Convert.ToDecimal(xml.SelectSingleNode(@"/RatingServiceSelectionResponse/RatedShipment/TotalCharges/MonetaryValue").InnerText);
            }

            return rate;
        }

        public string buildUPSXml(string weight, string shipperZipCode, string shipperCountryCode, string destinationZipCode, string destinationCountryCode, string serviceCode)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("<?xml version='1.0'?>");
              sb.Append("	<AccessRequest xml:lang='en-US'>");
              sb.Append("		<AccessLicenseNumber>").Append(m_accessNumber).Append("</AccessLicenseNumber>");
              sb.Append("		<UserId>").Append(m_userName).Append("</UserId>");
              sb.Append("		<Password>").Append(m_password).Append("</Password>");
              sb.Append("	</AccessRequest>");
              sb.Append("<?xml version='1.0'?>");
              sb.Append("	<RatingServiceSelectionRequest xml:lang='en-US'>");
              sb.Append("		<Request>");
              sb.Append("			<TransactionReference>");
              sb.Append("				<CustomerContext>Rating and Service</CustomerContext>");
              sb.Append("				<XpciVersion>1.0001</XpciVersion>");
              sb.Append("			</TransactionReference>");
              sb.Append("			<RequestAction>Rate</RequestAction>");
              sb.Append("			<RequestOption>Rate</RequestOption>");
              sb.Append("		</Request>");
              sb.Append("		<PickupType>");
              sb.Append("			<Code>").Append(m_pickupType).Append("</Code>");
              sb.Append("		</PickupType>");
              sb.Append("		<Shipment>");
              sb.Append("			<Shipper>");
              sb.Append("				<Address>");
              sb.Append("					<PostalCode>").Append(shipperZipCode).Append("</PostalCode>");
              sb.Append("          <CountryCode>").Append(shipperCountryCode).Append("</CountryCode>");
              sb.Append("				</Address>");
              sb.Append("			</Shipper>");
              sb.Append("			<ShipTo>");
              sb.Append("				<Address>");
              sb.Append("					<PostalCode>").Append(destinationZipCode).Append("</PostalCode>");
              sb.Append("					<CountryCode>").Append(destinationCountryCode).Append("</CountryCode>");
              sb.Append("				</Address>");
              sb.Append("			</ShipTo>");
              sb.Append("			<Service>");
              sb.Append("				<Code>").Append(serviceCode).Append("</Code>");
              sb.Append("			</Service>");
              sb.Append("			<Package>");
              sb.Append("				<PackagingType>");
              sb.Append("					<Code>02</Code>");
              sb.Append("					<Description>Package</Description>");
              sb.Append("				</PackagingType>");
              sb.Append("				<Description>Rate Shopping</Description>");
              sb.Append("				<PackageWeight>");
              sb.Append("				    <UnitOfMeasurement>");
			  sb.Append("				    <Code>LBS</Code>");
              sb.Append("				    </UnitOfMeasurement>");
              sb.Append("					<Weight>").Append(weight).Append("</Weight>");
              sb.Append("				</PackageWeight>");
              sb.Append("			</Package>");
              sb.Append("			<ShipmentServiceOptions/>");
              sb.Append("		</Shipment>");
              sb.Append("</RatingServiceSelectionRequest>");
            return sb.ToString();
        }
        #endregion
    }

    
}
