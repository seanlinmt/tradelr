using System.Collections.Generic;
using System.Xml.Serialization;

namespace OpenSRS
{
    public class DataItem
    {
        [XmlAttribute(AttributeName = "key")]
        public string Key { get; set; }

        [XmlText]
        public string Value { get; set; }

        [XmlArrayItem(ElementName = "item")]
        public List<DataItem> dt_assoc { get; set; }

        [XmlArrayItem(ElementName = "item")]
        public List<DataArrayItem> dt_array { get; set; }

        public DataItem(string key, string value)
        {
            Key = key;
            Value = value;
        }

        public DataItem(string key, IEnumerable<DataItem> attr)
            : this()
        {
            Key = key;
            dt_assoc.AddRange(attr);
        }

        public DataItem()
        {
            dt_assoc = new List<DataItem>();
        }

    }
}
