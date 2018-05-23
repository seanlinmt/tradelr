using System.Xml.Serialization;

namespace Shipwire.order
{
    public class Exception
    {
        [XmlAttribute]
        public string type { get; set; }

        [XmlText]
        public string Text { get; set; }
    }
}
