using System.Collections.Generic;
using System.Xml.Serialization;

namespace Shipwire.rate
{
    public class ResponseOrder
    {
        [XmlAttribute]
        public int sequence { get; set; }

        public List<Quote> Quotes { get; set; }
    }
}
