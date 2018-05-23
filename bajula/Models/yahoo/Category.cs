using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace tradelr.Models.yahoo
{
    public class Category
    {
        public string id { get; set; }
        public string uri { get; set; }
        public DateTime created { get; set; }
        public DateTime updated { get; set; }
        public string name { get; set; }
    }
}