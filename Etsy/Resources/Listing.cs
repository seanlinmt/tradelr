using System;
using System.Collections.Generic;
using tradelr.Time;

namespace Etsy.Resources
{
    public class Listing
    {
        public int listing_id { get; set; }
        public string state { get; set; }
        public int user_id { get; set; }
        public string title { get; set; }
        public string description { get; set; }
        public DateTime creation
        {
            get { return UnixTime.ToDateTime(creation_tsz); }
            set { creation_tsz = UnixTime.ToDouble(value); }
        }

        public double creation_tsz { get; set; }

        public DateTime ending
        {
            get { return UnixTime.ToDateTime(ending_tsz); }
            set { ending_tsz = UnixTime.ToDouble(value); }
        }

        public double ending_tsz { get; set; }

        public DateTime original_creation
        {
            get { return UnixTime.ToDateTime(original_creation_tsz); }
            set { original_creation_tsz = UnixTime.ToDouble(value); }
        }

        public double original_creation_tsz { get; set; }

        public DateTime last_modified
        {
            get { return UnixTime.ToDateTime(last_modified_tsz); }
            set { last_modified_tsz = UnixTime.ToDouble(value); }
        }

        public double last_modified_tsz { get; set; }

        public decimal price { get; set; }
        public string currency_code { get; set; }
        public int quantity { get; set; }
        public List<string> tags { get; set; }
        public dynamic materials { get; set; }
        public int? shop_section_id { get; set; }
        public int? featured_rank { get; set; }

        public DateTime state_changed
        {
            get { return UnixTime.ToDateTime(state_tsz); }
            set { state_tsz = UnixTime.ToDouble(value); }
        }

        public double state_tsz { get; set; }

        public int? hue { get; set; }
        public int? saturation { get; set; }
        public int? brightness { get; set; }
        public bool? is_black_and_white { get; set; }
        public string url { get; set; }
        public int views { get; set; }
        public int num_favorers { get; set; }

        public List<FavoriteListing> FavoredBy { get; set; }
        public List<ListingImage> Images { get; set; }
        public ListingImage MainImage { get; set; }
        public ListingPayment PaymentInfo { get; set; }
        public ShopSection Section { get; set; }
        public List<ShippingInfo> ShippingInfo { get; set; }
        public Shop Shop { get; set; }
        public User User { get; set; }

    }
}
