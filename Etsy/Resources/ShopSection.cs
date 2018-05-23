using System.Collections.Generic;

namespace Etsy.Resources
{
    public class ShopSection
    {
        public int shop_section_id { get; set; }
        public string title { get; set; }
        public int user_id { get; set; }
        public int rank { get; set; }
        public int active_listing_count { get; set; }

        public List<Listing> Listings { get; set; }
        public Shop Shop { get; set; }
    }
}
