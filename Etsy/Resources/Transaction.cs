using System;
using System.Collections.Generic;
using tradelr.Time;

namespace Etsy.Resources
{
    public class Transaction
    {
        public int transaction_id { get; set; }
        public string title { get; set; }
        public string description { get; set; }
        public int seller_user_id { get; set; }
        public int buyer_user_id { get; set; }
        public DateTime creation
        {
            get { return UnixTime.ToDateTime(creation_tsz); }
            set { creation_tsz = UnixTime.ToDouble(value); }
        }

        public double creation_tsz { get; set; }
        public DateTime paidTime
        {
            get { return UnixTime.ToDateTime(paid_tsz); }
            set { paid_tsz = UnixTime.ToDouble(value); }
        }

        public double paid_tsz { get; set; }
        public DateTime shippedTime
        {
            get { return UnixTime.ToDateTime(shipped_tsz); }
            set { shipped_tsz = UnixTime.ToDouble(value); }
        }

        public double shipped_tsz { get; set; }
        public decimal price { get; set; }
        public string currency_code { get; set; }
        public int quantity { get; set; }
        public List<string> tags { get; set; }
        public List<string> materials { get; set; }
        public int image_listing_id { get; set; }
        public int receipt_id { get; set; }
        public decimal shipping_cost { get; set; }
        public int listing_id { get; set; }
        public int seller_feedback_id { get; set; }
        public int buyer_feedback_id { get; set; }
        public string transaction_type { get; set; }
        public string url { get; set; }

        public Type Field { get; set; }
        public User Buyer { get; set; }
        public Listing Listing { get; set; }
        public Receipt Receipt { get; set; }
        public User Seller { get; set; }

    }
}
