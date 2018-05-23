using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace tradelr.Models.transactions
{
    public class OrderShippedEmailContent
    {
        public string orderNumber { get; set; }
        public string shippingAddress { get; set; }
        public string sender { get; set; }
        public string viewloc { get; set; }
    }
}
