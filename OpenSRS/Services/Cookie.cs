namespace OpenSRS.Services
{
    public class Cookie : OpenSRSService
    {
        private readonly MessageEnvelope message = new MessageEnvelope();

        public string GetCookie(string domainname)
        {
            message.SetField("protocol", "XCP");
            message.SetField("action", "set");
            message.SetField("object", "cookie");
            message.SetField("registrant_ip", ipaddress);

            message.AddAttributes(new[]
                              {
                                  new DataItem("domain", domainname),
                                  new DataItem("reg_password", RSP_PASSWORD),
                                  new DataItem("reg_username", RSP_USERNAME)
                              });

            var postdata = Serialize(message);

            SendRequest(postdata);

            var respmsg = responseObject as MessageEnvelope;
            if (respmsg == null || !respmsg.IsSuccess())
            {
                LogResponseMessage();
                return null;
            }

            var code = respmsg.GetResponseCode();

            if (code.code == "200")
            {
                return respmsg.GetAttribute("cookie");
            }

            return "";
        }
    }
}
