using System;
using System.Collections.Generic;
using System.IO;
using tradelr.Crypto;

namespace OpenSRS.Services
{
    public class Trust : OpenSRSService
    {
        // this registers GEOTrust Quick SSL (only allows a period of 1 - 5 years)
        private readonly MessageEnvelope message = new MessageEnvelope();

        public ResponseCode RegisterSSL(string domainname, int numberOfYears, Contact owner)
        {
            if (numberOfYears > 5)
            {
                throw new NotImplementedException();
            }

            var ms = new MemoryStream();

            // generate csr
            string csrstring = "";
            CSR.GenerateCsr(2048, domainname, owner.email, owner.city, owner.state, owner.countrycode, owner.org_name, ms);

            ms.Seek(0, SeekOrigin.Begin);
            using (var sr = new StreamReader(ms))
            {
                csrstring = sr.ReadToEnd();
            }

            message.SetField("protocol", "XCP");
            message.SetField("action", "sw_register");
            message.SetField("object", "trust_service");
            message.SetField("registrant_ip", ipaddress);

            var contactset = new List<DataItem>()
                                 {
                                     new DataItem("first_name", owner.first_name),
                                     new DataItem("last_name", owner.last_name),
                                     new DataItem("org_name", owner.org_name),
                                     new DataItem("address1", owner.address1),
                                     new DataItem("city", owner.city),
                                     new DataItem("state", owner.state),
                                     new DataItem("postal_code", owner.postcode),
                                     new DataItem("country", owner.countrycode),
                                     new DataItem("phone", owner.phone),
                                     new DataItem("email", owner.email)
                                 };

            if (owner.address2 != null)
            {
                contactset.Add(new DataItem("address2", owner.address2));
            }

            if (owner.address3 != null)
            {
                contactset.Add(new DataItem("address3", owner.address3));
            }

            if (owner.fax != null)
            {
                contactset.Add(new DataItem("fax", owner.fax));
            }

            message.AddAttributes(new[]
                                      {
                                          new DataItem("domain", domainname),
                                          new DataItem("csr", csrstring),
                                          new DataItem("product_type", "quickssl"),
                                          new DataItem("approver_email", "administrator@tradelr.com"),
                                          new DataItem("reg_type", "new"),
                                          new DataItem("handle", "save"),
                                          new DataItem("server_type", "iis"),
                                          new DataItem("period", numberOfYears.ToString()),
                                          new DataItem("contact_set", new[]
                                                                          {
                                                                              new DataItem("admin", contactset),
                                                                              new DataItem("billing", contactset),
                                                                              new DataItem("tech", contactset)
                                                                          })
                                      });

            var postdata = Serialize(message);

            SendRequest(postdata);

            var respmsg = responseObject as MessageEnvelope;
            if (respmsg == null || !respmsg.IsSuccess())
            {
                LogResponseMessage();
                return null;
            }

            LogResponseMessage();

            return respmsg.GetResponseCode();
        }
    }
}
