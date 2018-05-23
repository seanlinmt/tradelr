using System.Collections.Generic;
using System.Xml.Serialization;

namespace Shipwire.order
{
    public class Order
    {
        [XmlAttribute]
        public string id { get; set; }
        public string Warehouse { get; set; }
        public AddressInfo AddressInfo { get; set; }
        public string Shipping { get; set; }

        [XmlElement(ElementName = "Item")]
        public List<OrderItem> items { get; set; }

        private int itemcount { get; set; }

        public Order()
        {
            
        }

        public Order(AddressInfo addressInfo)
        {
            AddressInfo = addressInfo;
            Warehouse = "00";
            itemcount = 0;
            items = new List<OrderItem>();
        }

        public Order(string orderid, AddressInfo addressInfo, string shipping)
            : this(addressInfo)
        {
            id = orderid;
            Shipping = shipping;
        }
        public void AddItem(OrderItem item)
        {
            item.Number = itemcount++;
            items.Add(item);
        }
    }
}
