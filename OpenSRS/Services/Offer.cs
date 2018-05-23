using System;
using System.Collections.Generic;

namespace OpenSRS.Services
{
    public class Offer : OpenSRSService
    {
        private readonly MessageEnvelope message = new MessageEnvelope();

        public string GenerateCodes(int quantity, OfferType type)
        {
            message.SetField("protocol", "XCP");
            message.SetField("action", "GENERATE_CODES");
            message.SetField("object", "OFFERS");

            message.AddAttributes(new[]
                              {
                                  new DataItem("quantity", quantity.ToString()),
                                  new DataItem("offer", type.ToString())
                              });

            var postdata = Serialize(message);

            SendRequest(postdata);

            var respmsg = responseObject as MessageEnvelope;
            if (respmsg == null || !respmsg.IsSuccess())
            {
                LogResponseMessage();
                return null;
            }

            return respmsg.GetAttribute("code");
        }

        public ResponseCode SetStatus(OfferStatus status, IEnumerable<string> codes)
        {
            message.SetField("protocol", "XCP");
            message.SetField("action", "SET_STATUS");
            message.SetField("object", "OFFERS");

            message.AddAttributes(new[]
                              {
                                  new DataItem("status", status.ToString())
                              });

            var postdata = Serialize(message);

            SendRequest(postdata);

            var respmsg = responseObject as MessageEnvelope;
            if (respmsg == null || !respmsg.IsSuccess())
            {
                LogResponseMessage();
                return null;
            }

            return respmsg.GetResponseCode();
        }

        public ResponseCode ViewCodes()
        {
            throw new NotImplementedException();
        }
    }
}
