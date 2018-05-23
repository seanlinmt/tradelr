using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using tradelr.Library;

namespace tradelr.DBML.Lucene.IndexingQueue
{
    [Serializable]
    public class ProductItem : BaseQueueItem
    {
        public string sku { get; set; }
        public string title { get; set; }
        public string category { get; set; }
        public string details { get; set; }

        public ProductItem()
        {
            
        }

        public ProductItem(string id) : base(id, LuceneIndexType.PRODUCTS)
        {
            
        }

        public ProductItem(product p) : this(p.id.ToString())
        {
            sku = string.Join(",", p.product_variants.Select(x => x.sku.ToLower()).ToArray());
            title = Utility.EmptyIfNull(p.title).ToLower();
            category = p.category.HasValue ? p.productCategory.MASTERproductCategory.name.ToLower() : "";
            details = Utility.EmptyIfNull(p.details).ToLower().StripHtmlTags();
        }
    }
}
