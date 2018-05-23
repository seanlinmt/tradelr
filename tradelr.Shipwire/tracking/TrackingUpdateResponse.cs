using System.Collections.Generic;
using System.Xml.Serialization;

namespace Shipwire.tracking
{
    public class TrackingUpdateResponse
    {
        public string Status { get; set; }
        
        [XmlElement(ElementName = "Order")]
        public List<Order> Orders { get; set; }

        public int TotalOrders { get; set; }
        public int TotalShippedOrders { get; set; }
        public int TotalProducts { get; set; }
        public string Bookmark { get; set; }
    }
}
