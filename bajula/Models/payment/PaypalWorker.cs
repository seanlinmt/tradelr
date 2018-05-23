using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using tradelr.DBML;
using tradelr.Library;
using tradelr.Library.Constants;
using tradelr.Models.subdomain;
using tradelr.Models.transactions;
using tradelr.Payment;
using tradelr.Common.Models.currency;
using PayPal.AdaptivePayments.Model;
using PayPal.AdaptivePayments;
using System.Configuration;
using System.Windows.Documents;

namespace tradelr.Models.payment
{
    public class PaypalWorker : IPaymentWorker
    {
        private string uniqueid { get; set; }
        private Transaction transaction { get; set; }
        private ITradelrRepository repository { get; set; }
        private Currency currency { get; set; }
        private string PaymentReceiverEmail { get; set; }
        private string returnUrl { get; set; }

        public PaypalWorker(string uniqueid, Transaction o, 
                                ITradelrRepository repo, 
                                string paypalEmail, 
                                int? currencyid,
                                string returnUrl)
        {
            this.uniqueid = uniqueid;
            this.transaction = o;
            this.repository = repo;
            this.currency = currencyid.ToCurrency();
            this.PaymentReceiverEmail = paypalEmail;
            this.returnUrl = returnUrl;
        }

        public string GetPaymentUrl()
        {
            var paymentReceiver = new Receiver
                                      {
                                          email = PaymentReceiverEmail, 
                                          amount = transaction.GetTotal()
                                      };

            var receivers = new ReceiverList(new List<Receiver>() { paymentReceiver });

            var payRequest = new PayRequest
                                 {
                                     actionType = "PAY",
                                     receiverList = receivers,
                                     currencyCode = currency.code,
                                     requestEnvelope = new RequestEnvelope
                                                           {
                                                               errorLanguage
                                                                   =
                                                                   "en_US"
                                                           },
                                     returnUrl = returnUrl,
                                     trackingId = uniqueid
                                 };
            payRequest.cancelUrl = payRequest.returnUrl;

            var configurationMap = Configuration.GetAcctAndConfig();

            var service = new AdaptivePaymentsService(configurationMap);

            var resp = service.Pay(payRequest);

            if (resp.responseEnvelope.ack == AckCode.FAILURE ||
               resp.responseEnvelope.ack == AckCode.FAILUREWITHWARNING)
            {
                // TODO: serialize this properly
                throw new Exception(resp.error.First().message);
            }

            if (resp.paymentExecStatus != "CREATED" && resp.paymentExecStatus != "COMPLETED")
            {
                // create the payment 
                string errorString = "";
                foreach (var error in resp.payErrorList.payError)
                {
                    errorString += error.error.message;
                }
                throw new Exception(errorString);
            }

            string paymentStatus = PaymentStatus.Charging.ToString();
            if (resp.paymentExecStatus == "COMPLETED")
            {
                paymentStatus = PaymentStatus.Accepted.ToString();
            }

            var pay = new DBML.payment
            {
                created = DateTime.UtcNow,
                paidAmount = transaction.GetTotal(),
                method = PaymentMethodType.Paypal.ToString(),
                orderid = transaction.GetID(),
                status = paymentStatus,
                reference = uniqueid,
                redirectUrl = PaymentConstants.PaypalRedirectUrl + resp.payKey
            };

            transaction.AddPayment(pay, true);

            // update invoice status
            repository.Save();

            return pay.redirectUrl;
        }
    }
}