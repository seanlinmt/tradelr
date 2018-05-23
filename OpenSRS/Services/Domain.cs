namespace OpenSRS.Services
{
    public class Domain : OpenSRSService
    {
        private readonly MessageEnvelope message = new MessageEnvelope();

        public ResponseCode IsDomainTaken(string domainname)
        {
            message.SetField("protocol", "XCP");
            message.SetField("action", "LOOKUP");
            message.SetField("object", "DOMAIN");
            message.SetField("registrant_ip", ipaddress);

            message.AddAttributes(new[]
                              {
                                  new DataItem("domain", domainname),
                                  new DataItem("no_cache", "1")
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

        public ResponseCode GetPrice(string domainname, int numberofYears, bool isnew, out decimal price)
        {
            message.SetField("protocol", "XCP");
            message.SetField("action", "GET_PRICE");
            message.SetField("object", "DOMAIN");

            message.AddAttributes(new[]
                              {
                                  new DataItem("domain", domainname),
                                  new DataItem("period", numberofYears.ToString()),
                                  new DataItem("reg_type", isnew?"new":"renewal")
                              });

            var postdata = Serialize(message);

            SendRequest(postdata);

            var respmsg = responseObject as MessageEnvelope;
            if (respmsg == null || !respmsg.IsSuccess())
            {
                LogResponseMessage();
                price = 0;
                return null;
            }

            decimal.TryParse(respmsg.GetAttribute("price"), out price);

            return respmsg.GetResponseCode();
        }

        public bool ModifyPrivacy(string cookie, PrivacyDisplayType type)
        {
            message.SetField("protocol", "XCP");
            message.SetField("action", "MODIFY");
            message.SetField("object", "DOMAIN");
            message.SetField("cookie", cookie);
            message.SetField("registrant_ip", ipaddress);

            message.AddAttributes(new[]
                              {
                                  new DataItem("data", "ca_whois_display_setting"),
                                  new DataItem("display", type.ToString())
                              });

            var postdata = Serialize(message);

            SendRequest(postdata);

            var respmsg = responseObject as MessageEnvelope;
            if (respmsg == null || !respmsg.IsSuccess())
            {
                LogResponseMessage();
                return false;
            }

            var code = respmsg.GetResponseCode();

            if (code.code == "200")
            {
                return true;
            }

            return false;
        }

        public ResponseCode RegisterDomain(string domainname, bool enablePrivacy, int period,
            Contact owner)
        {
            message.SetField("protocol", "XCP");
            message.SetField("action", "SW_REGISTER");
            message.SetField("object", "DOMAIN");
            message.SetField("registrant_ip", ipaddress);

            message.AddAttributes(new[]
                                      {
                                          new DataItem("auto_renew", "1"),
                                          new DataItem("custom_nameservers", "0"),
                                          new DataItem("dns_template", "tradelr"),
                                          new DataItem("f_lock_domain", "1"),
                                          new DataItem("domain", domainname),
                                          new DataItem("f_whois_privacy", enablePrivacy?"1":"0"),
                                          new DataItem("custom_tech_contact", "0"),
                                          new DataItem("reg_username", RSP_USERNAME),
                                          new DataItem("reg_password", RSP_PASSWORD),
                                          new DataItem("reg_type", "new"),
                                          new DataItem("period", period.ToString()),
                                          new DataItem("contact_set", new[]
                                            {
                                                new DataItem("owner",new[]
                                                {
                                                    new DataItem("first_name", owner.first_name),
                                                    new DataItem("last_name", owner.last_name),
                                                    new DataItem("org_name", owner.org_name),
                                                    new DataItem("address1", owner.address1),
                                                    new DataItem("address2", owner.address2),
                                                    new DataItem("address3", owner.address3),
                                                    new DataItem("city", owner.city),
                                                    new DataItem("state", owner.state),
                                                    new DataItem("postal_code", owner.postcode),
                                                    new DataItem("country", owner.countrycode),
                                                    new DataItem("phone", owner.phone),
                                                    new DataItem("fax", owner.fax),
                                                    new DataItem("email", owner.email)
                                                }),
                                                new DataItem("admin",new[]
                                                {
                                                    new DataItem("first_name", owner.first_name),
                                                    new DataItem("last_name", owner.last_name),
                                                    new DataItem("org_name", owner.org_name),
                                                    new DataItem("address1", owner.address1),
                                                    new DataItem("address2", owner.address2),
                                                    new DataItem("address3", owner.address3),
                                                    new DataItem("city", owner.city),
                                                    new DataItem("state", owner.state),
                                                    new DataItem("postal_code", owner.postcode),
                                                    new DataItem("country", owner.countrycode),
                                                    new DataItem("phone", owner.phone),
                                                    new DataItem("fax", owner.fax),
                                                    new DataItem("email", owner.email)
                                                }),
                                                new DataItem("billing",new[]
                                                {
                                                    new DataItem("first_name", owner.first_name),
                                                    new DataItem("last_name", owner.last_name),
                                                    new DataItem("org_name", owner.org_name),
                                                    new DataItem("address1", owner.address1),
                                                    new DataItem("address2", owner.address2),
                                                    new DataItem("address3", owner.address3),
                                                    new DataItem("city", owner.city),
                                                    new DataItem("state", owner.state),
                                                    new DataItem("postal_code", owner.postcode),
                                                    new DataItem("country", owner.countrycode),
                                                    new DataItem("phone", owner.phone),
                                                    new DataItem("fax", owner.fax),
                                                    new DataItem("email", owner.email)
                                                })
                                            }) // end contact set
                                      });

            var postdata = Serialize(message);

            SendRequest(postdata);

            var respmsg = responseObject as MessageEnvelope;
            if (respmsg == null || !respmsg.IsSuccess())
            {
                LogResponseMessage();
                return null;
            }

            // log this for debugging purposes
            LogResponseMessage();

            return respmsg.GetResponseCode();
        }
    }
}
