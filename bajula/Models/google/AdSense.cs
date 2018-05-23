using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace tradelr.Models.google
{
    public class AdSense
    {
        public string publisherID { get; set; }
        public string advertID { get; set; }

        public AdSense(string publisher, string ad)
        {
            publisherID = publisher;
            advertID = ad;
        }
    }
}