using System.Collections.Generic;
using System.Xml.Serialization;
using Shipwire.order;

namespace Shipwire.rate
{
    public class RateRequest
    {
        public string EmailAddress { get; set; }
        public string Password { get; set; }

        [XmlElement(ElementName = "Order")]
        public List<Order> Orders { get; set; }

        public RateRequest()
        {
            
        }

        public RateRequest(string email, string password)
        {
            EmailAddress = email;
            Password = password;
            Orders = new List<Order>();
        }

        public void AddOrder(Order order)
        {
            Orders.Add(order);
        }
    }
}
