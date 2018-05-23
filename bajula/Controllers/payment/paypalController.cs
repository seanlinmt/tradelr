using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;
using tradelr.Common;
using tradelr.DataAccess;
using tradelr.DBML;
using clearpixels.Logging;
using tradelr.Models.account;
using tradelr.Models.account.plans;
using tradelr.Payment;

namespace tradelr.Controllers.payment
{
    // https://cms.paypal.com/us/cgi-bin/?cmd=_render-content&content_ID=developer/e_howto_admin_IPNIntro#id091F0M006Y4
    public class paypalController : Controller
    {
        [HttpPost]
        public void ipn(string txn_id, string custom, string mc_gross, string payment_status, string receiver_email)
        {
            //Per PayPal Order Management / Integration Guide Pg.25
            //we have to validate message by sending message back to paypal
            //Post back to either sandbox or live
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(PaymentConstants.PaypalPostAddress);

            //Set values for the request back
            req.Method = "POST";
            req.ContentType = "application/x-www-form-urlencoded";
            byte[] param = Request.BinaryRead(Request.ContentLength);
            string strRequest = Encoding.ASCII.GetString(param);
            strRequest += "&cmd=_notify-validate";
            req.ContentLength = strRequest.Length;

            //for proxy
            //WebProxy proxy = new WebProxy(new Uri("http://url:port#"));
            //req.Proxy = proxy;

            //Send the request to PayPal and get the response
            StreamWriter streamOut = new StreamWriter(req.GetRequestStream(), Encoding.ASCII);
            streamOut.Write(strRequest);
            streamOut.Close();
            StreamReader streamIn = new StreamReader(req.GetResponse().GetResponseStream());
            string strResponse = streamIn.ReadToEnd();
            streamIn.Close();

            string response;
            Request.InputStream.Position = 0;
            using (var sr = new StreamReader(Request.InputStream))
            {
                response = sr.ReadToEnd();
            }
            var subdomainid = long.Parse(custom);
            try
            {
                if (strResponse == "VERIFIED")
                {
                    using (var repository = new TradelrRepository())
                    {
                        var sd = repository.GetSubDomain(subdomainid);
                        if (payment_status.ToLower() == "completed" && receiver_email == PaymentConstants.PaypalSubscribeEmail)
                        {
                            // payment verified
                            sd.accountTypeStatus = (int)AccountPlanPaymentStatus.NONE;
                            sd.accountTransactionID = txn_id;
                            Syslog.Write(string.Concat("SUBSCRIBE:", subdomainid, ":oldplan:", sd.accountType, ":newplan:", sd.accountTypeNew));
                            if (sd.accountType != sd.accountTypeNew)
                            {
                                sd.accountType = sd.accountTypeNew;
                            }
                            else
                            {
                                Syslog.Write("Payment received for subdomain ID:" + subdomainid);
                            }
                            repository.Save();
                        }
                    }
                    Syslog.Write("VALID IPN:" + HttpUtility.HtmlEncode(response));
                }
                else
                {
                    if (strResponse == "INVALID")
                    {

                        Syslog.Write("INVALID IPN:" + HttpUtility.HtmlEncode(response));
                    }
                    else
                    {
                        Syslog.Write("UNKNOWN IPN:" + HttpUtility.HtmlEncode(response));
                    }
                }
            }
            catch (Exception ex)
            {
                Syslog.Write(ex);
                Syslog.Write(string.Format("Exception {0}: {1}", subdomainid ,HttpUtility.HtmlEncode(response)));
            }
            
        }
    }
}
