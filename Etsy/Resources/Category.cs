using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Etsy.Resources
{
    public class Category
    {
        public string meta_title { get; set; }
        public string meta_keywords { get; set; }
        public string meta_description { get; set; }
        public string page_description { get; set; }
        public string page_title { get; set; }
        public string category_name { get; set; }
        public string short_name { get; set; }
        public string long_name { get; set; }
        public int num_children { get; set; }

    }
}
