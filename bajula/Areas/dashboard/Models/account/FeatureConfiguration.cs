using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using tradelr.DBML;

namespace tradelr.Areas.dashboard.Models.account
{
    public class FeatureConfiguration
    {
        // send transactions
        public bool organisationDetailsConfigured { get; set; }
        public bool personalDetailsConfigured { get; set; }

        // receive payments
        public bool paymentMethodsConfigured { get; set; }

        // online store
        public bool shippingConfigured { get; set; }

        // facebook store
        public bool facebookStoreConfigured { get; set; }


        public void GetInitialisedFeatures(MASTERsubdomain sd)
        {
            organisationDetailsConfigured = !string.IsNullOrEmpty(sd.organisation.name);

            var owner = sd.organisation.users.First();
            personalDetailsConfigured = !string.IsNullOrEmpty(owner.firstName) && !string.IsNullOrEmpty(owner.lastName);

            paymentMethodsConfigured = sd.paymentMethods.Count != 0;

        }

    }
}