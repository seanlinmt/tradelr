using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Shipwire.inventory
{
    public class Product
    {
        [XmlAttribute]
        public string code { get; set; }
        [XmlAttribute]
        public int quantity { get; set; }
    }
}
