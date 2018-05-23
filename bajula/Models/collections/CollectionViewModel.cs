using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using tradelr.DBML;
using tradelr.Models.products;

namespace tradelr.Models.collections
{
    public class CollectionViewModel 
    {
        public long id { get; set; }
        public string title { get; set; }
        public string permalink { get; set; }
        public string details { get; set; }
        public bool visible { get; set; }
        public string productids { get; set; }
        public IEnumerable<ProductBase> products { get; set; }
        public string fullUrl { get; set; }
    }
}