using System;
using System.Collections.Generic;
using tradelr.Time;

namespace Etsy.Resources
{
    public class Order
    {
        public int order_id { get; set; }
        public int user_id { get; set; }
        public DateTime creation
        {
            get { return UnixTime.ToDateTime(creation_tsz); }
            set { creation_tsz = UnixTime.ToDouble(value); }
        }

        public double creation_tsz { get; set; }
        public List<Receipt> Receipts { get; set; }
        public User User { get; set; }

    }
}
