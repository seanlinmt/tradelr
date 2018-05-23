using System.Collections.Generic;
using System.Xml.Serialization;

namespace Shipwire.order
{
    public class OrderList
    {
        public string EmailAddress { get; set; }
        public string Password { get; set; }
        public string Server { get; set; }
        public string Referer { get; set; }

        [XmlElement(ElementName = "Order")]
        public List<Order> Orders { get; set; }

        public OrderList()
        {
            
        }

        public OrderList(string email, string password)
        {
            EmailAddress = email;
            Password = password;
            Referer = "5191";
            Orders = new List<Order>();
#if DEBUG
            Server = "Test";
#else
            Server = "Production";
#endif
        }

        public void AddOrder(Order order)
        {
            Orders.Add(order);
        }
    }
}
