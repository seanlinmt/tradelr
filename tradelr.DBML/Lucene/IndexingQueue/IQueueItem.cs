using System;
using System.Xml;
using System.Xml.Serialization;
using System.Xml.Linq;

namespace tradelr.DBML.Lucene.IndexingQueue
{
    public interface IQueueItem
    {
        string id { get; set; }
        LuceneIndexType indexType { get; set; }
    }
}
