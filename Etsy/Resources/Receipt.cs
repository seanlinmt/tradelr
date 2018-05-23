using System;
using System.Collections.Generic;
using tradelr.Time;

namespace Etsy.Resources
{
    public class Receipt
    {
        public int receipt_id { get; set; }
        public int order_id { get; set; }
        public int seller_user_id { get; set; }
        public int buyer_user_id { get; set; }
        public DateTime creation
        {
            get { return UnixTime.ToDateTime(creation_tsz); }
            set { creation_tsz = UnixTime.ToDouble(value); }
        }

        public double creation_tsz { get; set; }
        public string name { get; set; }
        public string first_line { get; set; }
        public string second_line { get; set; }
        public string city { get; set; }
        public string state { get; set; }
        public string zip { get; set; }
        public int country_id { get; set; }
        public string payment_method { get; set; }
        public string payment_email { get; set; }
        public string message_from_seller { get; set; }
        public string message_from_buyer { get; set; }
        public bool was_paid { get; set; }
        public decimal total_price { get; set; }
        public decimal total_shipping_cost { get; set; }
        public string currency_code { get; set; }
        public string message_from_payment { get; set; }
        public bool was_shipped { get; set; }
        public string buyer_email { get; set; }
        public string seller_email { get; set; }

        public User Buyer { get; set; }
        public Country Country { get; set; }
        public List<Listing> Listings { get; set; }
        public Order Order { get; set; }
        public User Seller { get; set; }
        public List<Transaction> Transactions { get; set; }

    }
}
