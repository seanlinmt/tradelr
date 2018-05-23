using System.Collections.Generic;
using System.Web.Mvc;
using tradelr.Areas.dashboard.Models.contact;
using tradelr.Models;
using tradelr.Models.contacts.viewmodel;

namespace tradelr.Areas.dashboard.Models.account
{
    public class AccountViewModel : ContactViewModel
    {
        public SelectList timezoneList { get; set; }
        public SelectList currencyList { get; set; }

        // payment gateways
        public string googleMerchantID { get; set; }
        public string googleMerchantKey { get; set; }
        public string paypalID { get; set; }

        // flags
        public bool offlineEnabled { get; set; }

        // customdomains
        public string customDomain { get; set; }
        public IEnumerable<SelectListItem> TLDExtensionList { get; set; }

        // account and trial period
        public string trialExpiryDate { get; set; }
        public bool isInTrialPeriod { get; set; }
        public bool notSubscribed { get; set; }

        // affiliates
        public string affiliateID { get; set; }

        public string affiliateTo { get; set; }

        public long orgid { get; set; }

        public AccountViewModel(BaseViewModel viewmodel) : base(viewmodel)
        {
        }
    }
}