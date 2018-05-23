using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Shipwire.inventory
{
    public class InventoryUpdateResponse
    {
        public string Status { get; set; }
        public string ErrorMessage { get; set; }

        [XmlElement(ElementName = "Product")]
        public List<Product> Products { get; set; }

        public int TotalProducts { get; set; }
        public string ProductCode { get; set; }
    }
}
