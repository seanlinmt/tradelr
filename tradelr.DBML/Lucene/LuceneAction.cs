using System;
using System.Xml.Linq;
using tradelr.DBML.Lucene.IndexingQueue;

namespace tradelr.DBML.Lucene
{
    public class LuceneAction
    {
        public long itemKey { get; set; }
        public LuceneIndexType type { get; set; }
        public string subdomainName { get; set; }
        public dynamic data { get; set; }
        public bool deleteOnly { get; set; }
    }

    public static class LuceneActionHelper
    {
        public static LuceneAction ToModel(this indexingQueue item)
        {
            var action = new LuceneAction
            {
                type = (LuceneIndexType)item.type,
                subdomainName = item.MASTERsubdomain.name,
                deleteOnly = item.deleteOnly,
                itemKey = item.itemKey
            };

            switch (action.type)
            {
                case LuceneIndexType.CONTACTS:
                    action.data = BaseQueueItem.Deserialize<ContactItem>(item.serializedItem);
                    break;
                case LuceneIndexType.PRODUCTS:
                    action.data = BaseQueueItem.Deserialize<ProductItem>(item.serializedItem);
                    break;
                case LuceneIndexType.TRANSACTION:
                    action.data = BaseQueueItem.Deserialize<TransactionItem>(item.serializedItem);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return action;
        }
    }
}
