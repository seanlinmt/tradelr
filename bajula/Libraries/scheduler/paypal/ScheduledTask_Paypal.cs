using System;
using tradelr.DBML;
using clearpixels.Logging;
using tradelr.Models.payment;
using tradelr.Models.transactions;
using tradelr.Payment;
using PayPal.AdaptivePayments;
using PayPal.AdaptivePayments.Model;

namespace tradelr.Libraries.scheduler
{
    public static partial class ScheduledTask
    {
        private static bool PaypalHandleBuyerPaidAndWeNeedToVerify(ITradelrRepository repository)
        {
            bool haveChanges = false;
            var paymentsNeededVerifying = repository.GetPayments(PaymentMethodType.Paypal, PaymentStatus.Charging);
            foreach (var payment in paymentsNeededVerifying)
            {
                var configurationMap = Configuration.GetAcctAndConfig();

                var service = new AdaptivePaymentsService(configurationMap);

                var detailsRequest = new PaymentDetailsRequest
                                         {
                                             payKey = payment.redirectUrl.ToPayPalPayKey(),
                                             requestEnvelope = new RequestEnvelope
                                                                   {
                                                                       errorLanguage
                                                                           =
                                                                           "en_US"
                                                                   },
                                             trackingId = payment.reference
                                         };

                PaymentDetailsResponse resp = null;
                bool haveError = false;
                try
                {
                    resp = service.PaymentDetails(detailsRequest);
                }
                catch (Exception ex)
                {
                    var affectedInvoice = payment.order;
                    Syslog.Write(
                                 string.Format(
                                     "Failed to process payment for order {0}, payment amount of {1} deleted",
                                     affectedInvoice.id, payment.paidAmount));
                    Syslog.Write(ex);
                    haveError = true;
                }
                if (!haveError && resp.responseEnvelope.ack == AckCode.SUCCESS)
                {
                    var transaction = new Transaction(payment.order, repository, null);

                    switch (resp.status)
                    {
                        case "COMPLETED":
                                transaction.UpdatePaymentStatus(payment, PaymentStatus.Accepted);

                                transaction.AddPaidAmount(payment.paidAmount);  // <------- order status is updated inside here
                                
                                // send download links if digital order
                                if (transaction.HasDigitalOrderItems())
                                {
                                    transaction.SendDownloadLinksEmail();
                                }
                                haveChanges = true;
                            break;
                        case "ERROR":
                                transaction.UpdatePaymentStatus(payment, PaymentStatus.Declined);    

                                haveChanges = true;
                            break;
                        case "EXPIRED":
                                repository.DeletePayment(payment);
                                haveChanges = true;
                            break;
                    }
                }
            }
            return haveChanges;
        }

    }
}