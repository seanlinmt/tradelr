using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using tradelr.Models.products;

namespace tradelr.Models.category
{
    public class CategoryViewModel
    {
        public string categoryTitle { get; set; }
        public IEnumerable<ProductBase> products { get; set; }
        public long catid { get; set; }
        public string productIdsInCategory { get; set; }
    }
}