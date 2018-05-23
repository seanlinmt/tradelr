using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace ColourLovers
{
    [XmlRoot(ElementName = "palettes")]
    public class Palettes
    {
        [XmlAttribute(AttributeName = "numResults")]
        public string numResults { get; set; }
        [XmlAttribute(AttributeName = "totalResults")]
        public string totalResults { get; set; }

        [XmlElement(ElementName = "palette")]
        public List<Palette> palettes { get; set; }
    }
}
