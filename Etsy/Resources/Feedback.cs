using System;
using System.Collections.Generic;
using tradelr.Time;

namespace Etsy.Resources
{
    public class Feedback
    {
        public int feedback_id { get; set; }
        public int transaction_id { get; set; }
        public int creator_user_id { get; set; }
        public int target_user_id { get; set; }
        public int seller_user_id { get; set; }
        public int buyer_user_id { get; set; }
        public DateTime creation
        {
            get { return UnixTime.ToDateTime(creation_tsz); }
            set { creation_tsz = UnixTime.ToDouble(value); }
        }

        public double creation_tsz { get; set; }
        public string message { get; set; }
        public int value { get; set; }
        public int image_feedback_id { get; set; }
        public string image_url_25x25 { get; set; }
        public string image_url_fullxfull { get; set; }

        public User Author { get; set; }
        public User Buyer { get; set; }
        List<Listing> Listing { get; set; }
        public User Seller { get; set; }
        public User Subject { get; set; }
        public Transaction Transaction { get; set; }

    }
}
