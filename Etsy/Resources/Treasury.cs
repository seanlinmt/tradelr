using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Etsy.Resources
{
    public class Treasury
    {
        public string id { get; set; }
        public string title { get; set; }
        public string description { get; set; }
        public int homepage { get; set; }
        public bool mature { get; set; }
        public string locale { get; set; }
        public int comment_count { get; set; }
        public List<string> tags { get; set; }
        TreasuryCounts counts { get; set; }
        public decimal hotness { get; set; }
        public string hotness_color { get; set; }
        public int user_id { get; set; }
        public string user_name { get; set; }
        public int user_avatar_id { get; set; }
        public int creation_tsz { get; set; }
        List<TreasuryListing> listings { get; set; }

    }
}
