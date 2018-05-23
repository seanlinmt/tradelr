using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ebay.Resources;
using eBay.Service.Call;
using eBay.Service.Core.Sdk;
using eBay.Service.Core.Soap;

namespace Ebay
{
    /// <summary>
    /// http://developer.ebay.com/devzone/xml/docs/reference/ebay/GetMyeBaySelling.html
    /// </summary>
    public class EbayService
    {
        // IMPORTANT!!! everytime site is added we need to generate
        // default ebay shipping profile for newly supported site
        // because profiles are per site
        // ALSO update EbayProductViewModel
        public static SiteCodeType[] SupportedSites = new []
                                                          {
                                                              SiteCodeType.US, 
                                                              SiteCodeType.Australia, 
                                                              SiteCodeType.Canada, 
                                                              SiteCodeType.Malaysia, 
                                                              SiteCodeType.Singapore, 
                                                              SiteCodeType.UK
                                                          };

#if DEBUG
        private const string Runname = "Sean_Lin-SeanLinfe-72a8--hyumhum";
        private const string DevId = "cb48fe2a-9054-486f-b504-ffcbfb373f8e";
        private const string AppId = "SeanLinfe-72a8-45e2-9e79-ac1fa28db56";
        private const string CertId = "8e562f77-c03c-4b69-a003-f518d1ce82b2";
        private const string ApiServerUrl = "https://api.sandbox.ebay.com/wsapi";
        private const string EpsServerUrl = "https://api.sandbox.ebay.com/ws/api.dll";
        private const string SignInUrl = "https://signin.sandbox.ebay.com/ws/eBayISAPI.dll?SignIn";
        private const string ViewItemUrl = "http://cgi.sandbox.ebay.com/ws/eBayISAPI.dll?ViewItem&amp;item={0}";
#else
        private const string Runname = "Sean_Lin-SeanLin6c-8e19--fxfudzb";
        private const string DevId = "cb48fe2a-9054-486f-b504-ffcbfb373f8e";
        private const string AppId = "SeanLin6c-8e19-49ad-9516-63b6ea0458a";
        private const string CertId = "e9972c47-83ad-4859-a6fb-55a46b6611b7";
        private const string ApiServerUrl = "https://api.ebay.com/wsapi";
        private const string EpsServerUrl = "https://api.ebay.com/ws/api.dll";
        private const string SignInUrl = "https://signin.ebay.com/ws/eBayISAPI.dll?SignIn";
        private const string ViewItemUrl = "http://cgi.ebay.com/ws/eBayISAPI.dll?ViewItem&amp;item={0}";
#endif
        private const string Redirecturl = SignInUrl + "&RuName={0}&SessID={1}";

        protected readonly ApiContext api;
        public string SessionID { get; private set; }
        public DateTime TokenExpires { get; private set; }

        public string responseXML { get; set; }

        public EbayService(string token)
        {
            api = new ApiContext
                      {
                          Version = "747",
                          Timeout = 60000,
                          RuName = Runname,
                          EPSServerUrl = EpsServerUrl,
                          SignInUrl = SignInUrl,
                          SoapApiServerUrl = ApiServerUrl,
                          ApiCredential = new ApiCredential() {ApiAccount = new ApiAccount(DevId, AppId, CertId)}
                      };

            if (!string.IsNullOrEmpty(token))
            {
                api.ApiCredential.eBayToken = token;
            }
        }

        public EbayService() : this("")
        {
            
        }

        public string GetRequestTokenUrl()
        {
            GetSessionIDCall gsc = new GetSessionIDCall(api);
            string sessionid = gsc.GetSessionID(Runname);
            SessionID = sessionid;

            return string.Format(Redirecturl, Runname, sessionid);
        }

        public string GetToken(string sessionid)
        {
            var ftc = new FetchTokenCall(api);
            api.ApiCredential.eBayToken = ftc.FetchToken(sessionid);
            TokenExpires = ftc.HardExpirationTime;
            return api.ApiCredential.eBayToken;
        }
    }
}
