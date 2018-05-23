using System.Collections.Generic;
using System.Xml.Serialization;

namespace OpenSRS
{
    public class DataArrayItem
    {
        [XmlAttribute(AttributeName = "key")]
        public string Key { get; set; }

        [XmlArrayItem(ElementName = "item")]
        public List<DataItem> dt_assoc { get; set; }
    }
}
