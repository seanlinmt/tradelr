using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace tradelr.Areas.dashboard.Models.product
{
    public class ProductVariantTransaction
    {
        public string sku { get; set; }
        public string variant_name { get; set; }
        public IEnumerable<ProductTransaction> products_sold { get; set; }
        public IEnumerable<ProductTransaction> products_bought { get; set; } 
    }
}