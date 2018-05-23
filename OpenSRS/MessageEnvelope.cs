using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace OpenSRS
{
    [XmlRoot(ElementName = "OPS_envelope")]
    public class MessageEnvelope
    {
        public MessageHeader header { get; set; }

        public MessageBody body { get; set; }

        public MessageEnvelope()
        {
            header = new MessageHeader();
            body = new MessageBody();
        }

        public void AddAttributes(IEnumerable<DataItem> attributes)
        {
            body.data_block.dt_assoc.Add(new DataItem("attributes", attributes));
        }

        public string GetAttribute(string key)
        {
            var attributes = body.data_block.dt_assoc
                    .Where(x => x.Key == "attributes")
                    .Select(x => x.dt_assoc)
                    .SingleOrDefault();

            if (attributes == null)
            {
                return null;
            }

            return attributes.Where(x => x.Key == key).Select(x => x.Value).SingleOrDefault();
        }

        public string GetField(string key)
        {
            return body.data_block.dt_assoc
                    .Where(x => x.Key == key)
                    .Select(x => x.Value)
                    .SingleOrDefault();
        }

        public void SetField(string key, string value)
        {
            body.data_block.dt_assoc.Add(new DataItem(key, value));
        }

        public bool IsSuccess()
        {
            var val = GetField("is_success");

            if (val == "1")
            {
                return true;
            }
            return false;
        }

        public ResponseCode GetResponseCode()
        {
            return new ResponseCode()
                       {
                           code = GetField("response_code"),
                           text = GetField("response_text")
                       };
        }
    }
}
