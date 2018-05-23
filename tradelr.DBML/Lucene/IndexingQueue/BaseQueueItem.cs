using System;
using System.Xml;
using System.Xml.Serialization;
using System.Xml.Linq;

namespace tradelr.DBML.Lucene.IndexingQueue
{
    [Serializable]
    public class BaseQueueItem : IQueueItem
    {
        public string id { get; set; }
        public LuceneIndexType indexType { get; set; }

        public BaseQueueItem()
        {
            
        }

        public BaseQueueItem(string id, LuceneIndexType indexType)
        {
            this.id = id;
            this.indexType = indexType;
        }
        
        public static XElement Serialize<T>(T obj)
        {
            var x = new XmlSerializer(typeof(T));
            var doc = new XDocument();

            using (XmlWriter xw = doc.CreateWriter())
            {
                x.Serialize(xw, obj);
                xw.Close();
            }

            return doc.Root;
        }


        public static T Deserialize<T>(XElement xml) where T : class
        {
            var x = new XmlSerializer(typeof(T));
            using (XmlReader xr = xml.CreateReader())
            {
                return x.Deserialize(xr) as T;
            }
        }
    }
}
