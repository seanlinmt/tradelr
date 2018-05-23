using System;
using System.Linq;
using tradelr.DBML.Helper;

namespace tradelr.DBML.Lucene.IndexingQueue
{
    [Serializable]
    public class TransactionItem : BaseQueueItem
    {
        public string receiver { get; set; }
        public string receiverfullname { get; set; }
        public string sku { get; private set; }
        public string description { get; set; }


        public TransactionItem()
        {
            
        }

        public TransactionItem(string id) : base(id, LuceneIndexType.TRANSACTION)
        {
            
        }

        public TransactionItem(order o) : this(o.id.ToString())
        {
            receiver = o.user.ToFullName().ToLower();
            receiverfullname = o.user.ToFullName().ToLower();
            sku = string.Join(",", o.orderItems.Select(x => x.product_variant.sku).ToArray()).ToLower();;
            description = string.Join(",", o.orderItems.Select(x => x.description).ToArray()).ToLower();
        }
    }
}
