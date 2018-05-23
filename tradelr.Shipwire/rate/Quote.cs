using System.Xml.Serialization;

namespace Shipwire.rate
{
    public class Quote
    {
        [XmlAttribute]
        public string method { get; set; }

        public string Warehouse { get; set; }
        public string Service { get; set; }
        public Cost Cost { get; set; }
        public DeliveryEstimate DeliveryEstimate { get; set; }
    }
}
