using System;
using tradelr.DBML;
using tradelr.DBML.Helper;
using clearpixels.Logging;
using tradelr.Models.transactions;
using tradelr.Payment;

namespace tradelr.Models.payment
{
    public class Payment
    {
        public long orderID { get; set; }
        public TransactionType orderType { get; set; }

        public string notes { get; set; }
        public string invoiceAmount { get; set; }
        public string totalUnpaid { get; set; }
        public string currencyCode { get; set; }
        public PaymentMethodList paymentMethods { get; set; }

        public Payment()
        {
            paymentMethods = new PaymentMethodList();
        }
    }

    public static class PaymentHelper
    {
        public const string paypalLogo = "<img src='/Content/img/payments/paypal_logo.png' />";

        private static string ToPaymentDetails(this DBML.payment row, long viewerid)
        {
            string contactParameters;

            if (row.order.receiverUserid == viewerid)
            {
                contactParameters = row.order.owner.ToString();
            }
            else
            {
                contactParameters = row.order.receiverUserid.ToString();
            }

            if (row.order.receiverUserid.HasValue)
            {
                if (row.order.user.organisation1.subdomain != row.order.user1.organisation1.subdomain)
                {
                    if (row.order.receiverUserid == viewerid)
                    {
                        contactParameters = string.Concat(contactParameters, "/", row.order.user1.organisation1.subdomain);
                    }
                    else
                    {
                        contactParameters = string.Concat(contactParameters, "/", row.order.user.organisation1.subdomain);
                    }
                }
            }
            else
            {
                return
                string.Format(
                    "<a href='#'>{0}</a> <br/><strong>{1}</strong> {2}",
                    row.order.receiverUserid == viewerid ? row.order.user1.ToFullName() : row.order.user.ToFullName(),
                    row.method,
                    !string.IsNullOrEmpty(row.notes) ? row.notes : "");
            }

            return
                string.Format(
                    "<a href='/dashboard/contacts/{0}'>{1}</a> <br/><strong>{2}</strong> {3}",
                    contactParameters,
                    row.order.receiverUserid == viewerid ? row.order.user1.ToFullName() : row.order.user.ToFullName(),
                    row.method,
                    !string.IsNullOrEmpty(row.notes)?row.notes:"");
        }

        public static string ToPaymentLogos(this MASTERsubdomain value)
        {
            string logos = "";
            if (!string.IsNullOrEmpty(value.GetPaypalID()))
            {
                logos = paypalLogo;
            }
            return logos;
        }

        public static string ToPayPalPayKey(this string value)
        {
            var index = value.IndexOf(PaymentConstants.PaypalPayKeyString);
            if (index == -1)
            {
                return "";
            }
            return value.Substring(index + PaymentConstants.PaypalPayKeyString.Length);
        }

        private static string ToPaymentStatus(this PaymentStatus status, PaymentMethodType method)
        {
            string returnString = "";
            switch (status)
            {
                case PaymentStatus.New:
                    if (method == PaymentMethodType.Paypal)
                    {
                        
                    }
                    else
                    {
                        returnString = "<span class=\"payment_accept\">Accept</span><br/><span class=\"payment_reject\">Reject</span>";
                    }
                    break;
                case PaymentStatus.Chargeable:
                    if (method == PaymentMethodType.Paypal)
                    {

                    }
                    else
                    {
                        Syslog.Write(new NotSupportedException());
                    }
                    break;
                case PaymentStatus.Charging:
                    if (method == PaymentMethodType.Paypal)
                    {
                        returnString = "<span class='orderstatus_verifying'>verifying</span>";
                    }
                    else
                    {
                        Syslog.Write(new NotSupportedException());
                    }
                    break;
                case PaymentStatus.Accepted:
                    if (method == PaymentMethodType.Paypal)
                    {
                        returnString = "<span class='orderstatus_paid'>charged</span>";
                    }
                    else
                    {
                        returnString = "<span class='orderstatus_paid'>accepted</span>";
                    }
                    break;
                case PaymentStatus.Declined:
                    if (method == PaymentMethodType.Paypal)
                    {
                        returnString = "<span class='orderstatus_red'>declined</span>";
                    }
                    else
                    {
                        returnString = "<span class='orderstatus_red'>rejected</span>";
                    }
                    break;
                case PaymentStatus.Cancelled:
                    if (method == PaymentMethodType.Paypal)
                    {

                    }
                    else
                    {
                        returnString = "<span class='orderstatus_disputed'>cancelled</span>";
                    }
                    break;
                default:
                    Syslog.Write("Unknown payment status: " + status);
                    break;
            }
            return returnString;
        }
        
    }
}
