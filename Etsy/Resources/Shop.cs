using System;
using System.Collections.Generic;
using tradelr.Time;

namespace Etsy.Resources
{
    public class Shop
    {
        public int shop_id { get; set; }
        public string shop_name { get; set; }
        public string first_line { get; set; }
        public string second_line { get; set; }
        public string city { get; set; }
        public string state { get; set; }
        public string zip { get; set; }
        public int country_id { get; set; }
        public int user_id { get; set; }
        public DateTime creation
        {
            get { return UnixTime.ToDateTime(creation_tsz); }
            set { creation_tsz = UnixTime.ToDouble(value); }
        }

        public double creation_tsz { get; set; }
        public string title { get; set; }
        public string announcement { get; set; }
        public string currency_code { get; set; }
        public bool is_vacation { get; set; }
        public string vacation_message { get; set; }
        public string sale_message { get; set; }

        public DateTime last_updated
        {
            get { return UnixTime.ToDateTime(last_updated_tsz); }
            set { last_updated_tsz = UnixTime.ToDouble(value); }
        }

        public double last_updated_tsz { get; set; }
        public int listing_active_count { get; set; }
        public string login_name { get; set; }
        public decimal? lat { get; set; }
        public decimal? lon { get; set; }
        public string alchemy_message { get; set; }
        public bool is_refusing_alchemy { get; set; }
        public string policy_welcome { get; set; }
        public string policy_payment { get; set; }
        public string policy_shipping { get; set; }
        public string policy_refunds { get; set; }
        public string policy_additional { get; set; }
        public DateTime policy_updated
        {
            get { return UnixTime.ToDateTime(policy_updated_tsz); }
            set { policy_updated_tsz = UnixTime.ToDouble(value); }
        }

        public double policy_updated_tsz { get; set; }
        public string vacation_autoreply { get; set; }
        public string ga_code { get; set; }
        public string name { get; set; }
        public string url { get; set; }
        public string image_url_760x100 { get; set; }
        public int num_favorers { get; set; }

        public List<Listing> Listings { get; set; }
        public List<Receipt> Receipts { get; set; }
        public List<ShopSection> Sections { get; set; }
        public List<Transaction> Transactions { get; set; }
        public User User { get; set; }

    }
}
