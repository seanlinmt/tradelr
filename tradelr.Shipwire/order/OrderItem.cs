using System.Xml.Serialization;

namespace Shipwire.order
{
    public class OrderItem
    {
        [XmlAttribute(AttributeName = "num")]
        public int Number { get; set; }
        [XmlElement(ElementName = "Code")]
        public string Sku { get; set; }
        public int Quantity { get; set; }
        public string Description { get; set; }
        public decimal Length { get; set; }
        public decimal Width { get; set; }
        public decimal Height { get; set; }
        public decimal Weight { get; set; }
        public decimal DeclaredValue { get; set; }
    }
}
