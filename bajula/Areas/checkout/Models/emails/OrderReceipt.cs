using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace tradelr.Areas.checkout.Models.emails
{
    public class OrderReceipt
    {
        public string shopname { get; set; }
        public string viewloc { get; set; }

        public string message { get; set; }

        public string date { get; set; }
        public string shippingAddress { get; set; }
        public string billingAddress { get; set; }

        public string subtotal { get; set; }
        public string discount { get; set; }
        public string shippingcost { get; set; }
        public string totalcost { get; set; }

        public IEnumerable<string> orderitems { get; set; } 
    }
}