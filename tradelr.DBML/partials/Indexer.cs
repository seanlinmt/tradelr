using System.Xml.Linq;
using tradelr.DBML.Lucene;

namespace tradelr.DBML
{
    public partial class TradelrRepository
    {
        public void AddActionToIndexingQueue(LuceneIndexType type, XElement data, long subdomainid, bool deleteOnly, long itemKey)
        {
            var single = new indexingQueue();
            single.subdomainid = subdomainid;
            single.serializedItem = data;
            single.itemKey = itemKey;
            single.deleteOnly = deleteOnly;
            single.type = (byte) type;

            db.indexingQueues.InsertOnSubmit(single);
        }
    }
}