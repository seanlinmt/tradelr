using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Etsy.Resources
{
    public class PaymentTemplate
    {
        public int payment_template_id { get; set; }
        public bool allow_check { get; set; }
        public bool allow_mo { get; set; }
        public bool allow_other { get; set; }
        public bool allow_paypal { get; set; }
        public string paypal_email { get; set; }
        public string name { get; set; }
        public string first_line { get; set; }
        public string second_line { get; set; }
        public string city { get; set; }
        public string state { get; set; }
        public string zip { get; set; }
        public int country_id { get; set; }
        public int user_id { get; set; }
        public Country Country { get; set; }
        public User User { get; set; }

    }
}
