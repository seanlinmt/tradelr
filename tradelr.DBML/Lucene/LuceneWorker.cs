using System;
using System.Xml.Linq;
using clearpixels.Models;
using tradelr.DBML.Lucene.IndexingQueue;

namespace tradelr.DBML.Lucene
{
    public class LuceneWorker
    {
        private IdName subdomain;
        private ITradelrRepository repository;

        public LuceneWorker(tradelrDataContext context, IdName subdomain)
        {
            repository = new TradelrRepository(context);
            this.subdomain = subdomain;
        }

        public LuceneWorker(ITradelrRepository repo, IdName subdomain)
        {
            repository = repo;
            this.subdomain = subdomain;
        }

        public void AddToIndex(LuceneIndexType type, dynamic data)
        {
            var action = GetLuceneAction(type, data, false, data.id);
            if (!LuceneUtil.Instance.ModifyIndex(action))
            {
                AddActionToQueue(action);
            }
        }

        public void DeleteFromIndex(LuceneIndexType type, long id)
        {
            var action = GetLuceneAction(type, null, true, id);
            if (!LuceneUtil.Instance.ModifyIndex(action))
            {
                AddActionToQueue(action);
            }
        }

        private LuceneAction GetLuceneAction(LuceneIndexType type, dynamic data, bool deleteOnly, long itemKey)
        {
            var action = new LuceneAction
            {
                type = type,
                subdomainName = subdomain.name,
                deleteOnly = deleteOnly,
                itemKey = itemKey
            };

            if (data == null)
            {
                action.data = BaseQueueItem.Serialize(new BaseQueueItem(itemKey.ToString(), type));
            }
            else
            {
                switch (type)
                {
                    case LuceneIndexType.CONTACTS:
                        action.data = new ContactItem(data);
                        break;
                    case LuceneIndexType.PRODUCTS:
                        action.data = new ProductItem(data);
                        break;
                    case LuceneIndexType.TRANSACTION:
                        action.data = new TransactionItem(data);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException("type");
                }
            }
            
            return action;
        }

        private void AddActionToQueue(LuceneAction action)
        {
            XElement serialized;
            switch (action.type)
            {
                case LuceneIndexType.CONTACTS:
                    serialized = BaseQueueItem.Serialize<ContactItem>(action.data);
                    break;
                case LuceneIndexType.PRODUCTS:
                    serialized = BaseQueueItem.Serialize<ProductItem>(action.data);
                    break;
                case LuceneIndexType.TRANSACTION:
                    serialized = BaseQueueItem.Serialize<TransactionItem>(action.data);
                    break;
                default:
                    throw new ArgumentOutOfRangeException("type");
            }
            repository.AddActionToIndexingQueue(action.type, serialized, long.Parse(subdomain.id), action.deleteOnly, action.itemKey);
            repository.Save();

        }
    }
}
