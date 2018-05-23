using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace tradelr.Models.store
{
    public class StoreCoupon
    {
        public string code { get; set; }
        public string value { get; set; }
        public string type { get; set; }
        public string min { get; set; } // minimumpurchase
    }
}