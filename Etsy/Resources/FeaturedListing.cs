using System;
using tradelr.Time;

namespace Etsy.Resources
{
    public class FeaturedListing
    {
        public int featured_listing_id { get; set; }
        public int listing_id { get; set; }
        public DateTime activeTime
        {
            get { return UnixTime.ToDateTime(active_time); }
            set { active_time = UnixTime.ToDouble(value); }
        }

        public double active_time { get; set; }
        public Listing Listing { get; set; }
        public FeaturedListingPicker Picker { get; set; }

    }
}
