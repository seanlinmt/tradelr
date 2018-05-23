using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace tradelr.Models.google.analytics
{
    public class VisitorStat
    {
        public string country { get; set; }
        public DateTime visitDate { get; set; }
        public int visitCount { get; set; }
        public int pageViews { get; set; }
        public double timeOnSite { get; set; }
        public string referrerPath { get; set; }
        public string referrerHostname { get; set; }
        public string keyword { get; set; }
    }
}