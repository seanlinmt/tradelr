using System;
using System.Collections.Generic;
using tradelr.Time;

namespace Etsy.Resources
{
    public class User
    {
        // fields
        public int user_id { get; set; }
        public string login_name { get; set; }
        public string primary_email { get; set; }
        public DateTime creation
        {
            get { return UnixTime.ToDateTime(creation_tsz); }
            set { creation_tsz = UnixTime.ToDouble(value); }
        }

        public double creation_tsz { get; set; }
        public int? referred_by_user_id { get; set; }
        public FeedbackInfo feedback_info { get; set; }

        // associations
        public List<UserAddress> Addresses { get; set; }
        public List<Receipt> BuyerReceipts { get; set; }
        public List<Transaction> BuyerTransactions { get; set; }
        public List<BillCharge> Charges { get; set; }
        public List<FavoriteUser> FavoredBy { get; set; }
        public List<FavoriteListing> FavoriteListings { get; set; }
        public List<FavoriteUser> FavoriteUsers { get; set; }
        public List<Feedback> FeedbackAsAuthor { get; set; }
        public List<Feedback> FeedbackAsBuyer { get; set; }
        public List<Feedback> FeedbackAsSeller { get; set; }
        public List<Feedback> FeedbackAsSubject { get; set; }
        public List<Order> Orders { get; set; }
        public List<BillPayment> Payments { get; set; }
        public UserProfile Profile { get; set; }
        public List<Shop> Shops { get; set; }

    }
}
