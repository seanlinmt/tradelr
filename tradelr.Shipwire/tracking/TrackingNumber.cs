using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Shipwire.tracking
{
    public class TrackingNumber
    {
        [XmlAttribute]
        public string carrier { get; set; }
        [XmlAttribute]
        public string href { get; set; }
        [XmlText]
        public string Value { get; set; }
    }
}
