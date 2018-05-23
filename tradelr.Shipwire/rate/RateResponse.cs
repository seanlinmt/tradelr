using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Shipwire.rate
{
    [XmlRoot(ElementName = "RateResponse")]
    public class RateResponse
    {
        public string Status { get; set; }
        public string ErrorMessage { get; set; }
        [XmlElement]
        public List<ResponseOrder> Order { get; set; }

        public void ValidateResponse()
        {
            if (Status != "OK")
            {
                throw new Exception(ErrorMessage);
            }

            if (Order.Count != 1)
            {
                throw new NotImplementedException();
            }
        }
    }
}
