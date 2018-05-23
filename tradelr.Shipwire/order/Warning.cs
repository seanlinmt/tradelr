using System.Xml.Serialization;

namespace Shipwire.order
{
    public class Warning
    {
        [XmlText]
        public string Text { get; set; }
    }
}
