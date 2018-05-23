using System.Xml.Serialization;

namespace Shipwire.rate
{
    public class Period
    {
        [XmlAttribute]
        public string units { get; set; }

        [XmlText]
        public int value { get; set; }
    }
}
