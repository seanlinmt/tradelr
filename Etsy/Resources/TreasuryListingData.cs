using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Etsy.Resources
{
    public class TreasuryListingData
    {
        public int user_id { get; set; }
        public string title { get; set; }
        public decimal price { get; set; }
        public int listing_id { get; set; }
        public string state { get; set; }
        public string shop_name { get; set; }
        public int image_id { get; set; }

    }
}
