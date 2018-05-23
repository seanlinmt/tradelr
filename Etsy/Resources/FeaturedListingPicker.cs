using System;
using System.Collections.Generic;
using tradelr.Time;

namespace Etsy.Resources
{
    public class FeaturedListingPicker
    {
        public int featured_listing_picker_id { get; set; }
        public string name { get; set; }
        public string url { get; set; }
        public DateTime active
        {
            get { return UnixTime.ToDateTime(active_tsz); }
            set { active_tsz = UnixTime.ToDouble(value); }
        }

        public double active_tsz { get; set; }
        public List<FeaturedListing> Featured { get; set; }
        public List<Listing> Listings { get; set; }

    }       
}
