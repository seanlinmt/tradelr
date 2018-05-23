using System.Collections.Generic;
using System.Xml.Serialization;

namespace Shipwire.order
{
    [XmlRoot(ElementName = "Order")]
    public class OrderResponse
    {
        public int number { get; set; }
        public string id { get; set; }  // value not returned
        public string status { get; set; }

        public Exception Exception { get; set; }
        public List<Warning> WarningList { get; set; }
        public Shipping Shipping { get; set; }
    }
}
