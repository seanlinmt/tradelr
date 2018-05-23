using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace ColourLovers
{
    public class Colours
    {
        [XmlElement]
        public List<string> hex { get; set; }
    }
}
