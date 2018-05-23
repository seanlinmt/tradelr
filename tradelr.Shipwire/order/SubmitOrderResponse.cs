using System.Collections.Generic;
using System.Xml.Serialization;

namespace Shipwire.order
{
    [XmlRoot(ElementName = "SubmitOrderResponse")]
    public class SubmitOrderResponse
    {
        public string Status { get; set; }
        public int TotalOrders { get; set; }
        public int TotalItems { get; set; }
        public string TransactionId { get; set; }
        public OrderInformation OrderInformation { get; set; }
        public int ProcessingTime { get; set; }

        public Dictionary<string ,string> GetExceptions()
        {
            var errors = new Dictionary<string, string>();
            foreach (var order in OrderInformation.Order)
            {
                if (!string.IsNullOrEmpty(order.Exception.Text))
                {
                    errors.Add(order.id, order.Exception.Text);
                }
            }
            return errors;
        }
    }
}
