using System;
using tradelr.Time;

namespace Etsy.Resources
{
    public class BillPayment
    {
        public int bill_payment_id { get; set; }
        public DateTime creation
        {
            get { return UnixTime.ToDateTime(creation_tsz); }
            set { creation_tsz = UnixTime.ToDouble(value); }
        }

        public double creation_tsz { get; set; }
        public string type { get; set; }
        public int user_id { get; set; }
        public decimal amount { get; set; }
        public string currency_code { get; set; }
        public int creation_month { get; set; }
        public int creation_year { get; set; }

    }
}
