using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace tradelr.Areas.dashboard.Models.store.general
{
    public class GeneralSettings
    {
        public long orgid { get; set; }
        public bool store_enabled { get; set; }
        public string facebookCoupon { get; set; }
        public string motd { get; set; }
        public string storeName { get; set; }
    }
}