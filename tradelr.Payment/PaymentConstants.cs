using PayPal;
using System.Collections.Generic;
using PayPal.Manager;

namespace tradelr.Payment
{
    public static class PaymentConstants
    {
        public const string PaypalPayKeyString = "paykey=";
#if DEBUG
        public static readonly Account PaypalProfile = new Account()
        {
            APIUserName = "APIUserName",
            APIPassword = "APIPassword",
            APISignature = "APISignature",
            ApplicationId = "ApplicationId"
        };
        public const string PaypalRedirectUrl = "https://www.sandbox.paypal.com/webscr?cmd=_ap-payment&paykey=";
        public const string PaypalPostAddress = "https://www.sandbox.paypal.com/cgi-bin/webscr";
        // we're not using any of the saved buttons but we're manually doing it
        //public const string PaypalSubscriptionSingle = "";
        //public const string PaypalSubscriptionBasic = "PaypalSubscriptionBasic";
        //public const string PaypalSubscriptionPro = "PaypalSubscriptionPro";
        //public const string PaypalSubscriptionUltimate = "PaypalSubscriptionUltimate";
        public const string PaypalIPNUrl = "http://baizura.com/ipn";
        public const string PaypalSubscribeEmail = "PaypalSubscribeEmail";
#else
        //public const string PaypalSubscriptionSingle = "PaypalSubscriptionSingle";
        //public const string PaypalSubscriptionBasic = "PaypalSubscriptionBasic";
        //public const string PaypalSubscriptionPro = "PaypalSubscriptionPro";
        //public const string PaypalSubscriptionUltimate = "PaypalSubscriptionUltimate";
        public const string PaypalIPNUrl = "http://www.tradelr.com/paypal/ipn";
        public const string PaypalSubscribeEmail = "administrator@tradelr.com";
        public const string PaypalPostAddress = "https://www.paypal.com/cgi-bin/webscr";
        public static readonly BaseAPIProfile PaypalProfile = new BaseAPIProfile()
        {
            APIUsername = "APIUsername.tradelr.com",
            APIPassword = "APIPassword",
            APISignature =
                "APISignature",
            Environment = "https://svcs.paypal.com/",
            ApplicationID = "ApplicationID",
            RequestDataformat = "SOAP11"
        };
        public const string PaypalRedirectUrl = "https://www.paypal.com/webscr?cmd=_ap-payment&paykey=";
#endif


    }
}
