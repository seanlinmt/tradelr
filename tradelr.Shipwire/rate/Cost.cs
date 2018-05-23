using System.Xml.Serialization;

namespace Shipwire.rate
{
    public class Cost
    {
        [XmlAttribute]
        public string currency { get; set; }
        [XmlText]
        public decimal value { get; set; }
    }
}
