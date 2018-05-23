using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace tradelr.Models.liquid.models.Blog
{
    public class BlogFeedItem
    {
        public int id { get; set; }
        public string title { get; set; }
        public string content { get; set; }
        public DateTime updated { get; set; }
    }
}