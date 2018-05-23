using System.Collections.Generic;
using DotLiquid;

namespace tradelr.Models.liquid.models
{
    public class Search : Drop
    {
        public bool performed { get; set; }
        public string terms { get; set; }
        public List<Product.Product> results { get; set; }
        public int results_count 
        { 
            get { return results.Count; } 
        }
    }
}