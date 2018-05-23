using System;
using System.Globalization;
using System.Xml.Serialization;

namespace Shipwire.tracking
{
    public class Order
    {
        [XmlAttribute]
        public string id { get; set; }

        [XmlAttribute(AttributeName = "shipped")]
        public string _shipped
        {
            get { return shipped?"YES":"NO"; }
            set { shipped = value.ToLowerInvariant() == "yes"; }
        }

        [XmlIgnore]
        public bool shipped { get; set; }

        [XmlAttribute]
        public string shipper { get; set; }
        [XmlAttribute]
        public string shipperFullName { get; set; }

        [XmlAttribute(AttributeName = "shipDate")]
        public string _shipDate
        {
            get { return shipDate.ToString("yyyy-MM-dd HH:mm:ss"); }
            set { shipDate = DateTime.ParseExact(value, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture); }
        }
        
        [XmlIgnore]
        public DateTime shipDate { get; set; }

        [XmlAttribute(AttributeName = "expectedDeliveryDate")]
        public string _expectedDeliveryDate
        {
            get { return expectedDeliveryDate.ToString("yyyy-MM-dd HH:mm:ss"); }
            set { expectedDeliveryDate = DateTime.ParseExact(value, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture); }
        }

        [XmlIgnore]
        public DateTime expectedDeliveryDate { get; set; }

        [XmlAttribute]
        public decimal handling { get; set; }
        [XmlAttribute]
        public decimal shipping { get; set; }
        [XmlAttribute]
        public decimal packaging { get; set; }
        [XmlAttribute]
        public decimal insurance { get; set; }
        [XmlAttribute]
        public decimal total { get; set; }

        public TrackingNumber TrackingNumber { get; set; }

    }
}
