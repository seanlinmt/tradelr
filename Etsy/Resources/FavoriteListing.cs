using System;
using tradelr.Time;

namespace Etsy.Resources
{
    public class FavoriteListing
    {
        public int listing_id { get; set; }
        public int user_id { get; set; }
        public string listing_state { get; set; }
        public DateTime created
        {
            get { return UnixTime.ToDateTime(create_date); }
            set { create_date = UnixTime.ToDouble(value); }
        }

        public double create_date { get; set; }

        public Listing Listing { get; set; }
        public User User { get; set; }
    }
}
