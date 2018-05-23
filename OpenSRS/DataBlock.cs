using System.Collections.Generic;
using System.Xml.Serialization;

namespace OpenSRS
{
    public class DataBlock
    {
        [XmlArrayItem(ElementName = "item")]
        public List<DataItem> dt_assoc { get; set; }

        public DataBlock()
        {
            dt_assoc = new List<DataItem>();
        }
    }
}
