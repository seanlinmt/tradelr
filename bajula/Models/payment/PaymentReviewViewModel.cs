using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace tradelr.Models.payment
{
    public class PaymentReviewViewModel
    {
        public long paymentid { get; set; }
        public string date_created { get; set; }
        public string payment_notes { get; set; }
        public string payment_method { get; set; }
        public string payment_amount { get; set; }
        public string order_total { get; set; }

        public string order_title { get; set; }
        public string order_type { get; set; }
    }
}