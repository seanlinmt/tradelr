using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace ColourLovers
{
    [XmlRoot(ElementName = "patterns")]
    public class Patterns
    {
        [XmlAttribute(AttributeName = "numResults")]
        public string numResults { get; set; }
        [XmlAttribute(AttributeName = "totalResults")]
        public string totalResults { get; set; }

        [XmlElement(ElementName = "pattern")]
        public List<Pattern> patterns { get; set; }
    }
}
