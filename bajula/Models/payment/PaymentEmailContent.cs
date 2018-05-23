using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace tradelr.Models.payment
{
    public class PaymentEmailContent
    {
        public string orderNumber { get; set; }
        public string status { get; set; }
        public string viewloc { get; set; }
    }
}
