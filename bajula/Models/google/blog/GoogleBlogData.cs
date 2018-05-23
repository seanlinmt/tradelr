using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Google.GData.Client;
using tradelr.Common;
using tradelr.DBML;

namespace tradelr.Models.google
{
    public class GoogleBlogData
    {
        public string name { get; set; }
        public string blogHref { get; set; }
        public string blogUri { get; set; }
    }
}