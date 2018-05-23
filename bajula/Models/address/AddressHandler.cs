using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using tradelr.DBML;
using tradelr.DBML.Helper;

namespace tradelr.Models.address
{
    public class AddressHandler
    {
        private ITradelrRepository repository { get; set; }
        private organisation target_org { get; set; }
        private order target_order { get; set; }

        public AddressHandler(organisation o, ITradelrRepository repository)
        {
            this.repository = repository;
            target_org = o;
        }

        public AddressHandler(order o, ITradelrRepository repository)
        {
            this.repository = repository;
            target_order = o;
        }

        public static string GetState(int? country, string state_us, string state_ca, string state_other)
        {
            if (country.HasValue && country.Value == 185 && !string.IsNullOrEmpty(state_us))
            {
                return state_us;
            }

            if (country.HasValue && country == 32 && !string.IsNullOrEmpty(state_ca))
            {
                return state_ca;
            }
            return state_other;
        }

        /// <summary>
        /// used when a user account is initially created to populate shipping and billing addresses
        /// </summary>
        /// <param name="firstName"></param>
        /// <param name="lastName"></param>
        public void CopyShippingAndBillingAddressFromOrgAddress(string firstName, string lastName)
        {
            // billing
            var dbAddressBilling = GetBillingAddress();

            dbAddressBilling.first_name = firstName;
            dbAddressBilling.last_name = lastName;
            dbAddressBilling.organisation_name = target_org.name ?? "";

            dbAddressBilling.city = target_org.city.HasValue?target_org.MASTERcity.name:"";
            dbAddressBilling.street_address = target_org.address ?? "";
            dbAddressBilling.phone = target_org.phone ?? "";
            dbAddressBilling.postcode = target_org.postcode ?? "";

            dbAddressBilling.country = target_org.country;

            dbAddressBilling.state = target_org.state ?? "";

            // shipping
            var dbAddressShipping = GetShippingAddress();

            dbAddressShipping.first_name = firstName;
            dbAddressShipping.last_name = lastName;
            dbAddressShipping.organisation_name = target_org.name ?? "";

            dbAddressShipping.city = target_org.city.HasValue ? target_org.MASTERcity.name : "";
            dbAddressShipping.street_address = target_org.address ?? "";
            dbAddressShipping.phone = target_org.phone ?? "";
            dbAddressShipping.postcode = target_org.postcode ?? "";

            dbAddressShipping.country = target_org.country;

            dbAddressShipping.state = target_org.state ?? "";
        }

        public void SetShippingAndBillingAddresses(string billing_first_name, string billing_last_name, string billing_company, string billing_address, string billing_city, long? billing_citySelected,
            string billing_postcode, string billing_phone, int? billing_country, string billing_states_canadian, string billing_states_other, string billing_states_us,
            string shipping_first_name, string shipping_last_name, string shipping_company, string shipping_address, string shipping_city, long? shipping_citySelected, string shipping_postcode, string shipping_phone,
            int? shipping_country, string shipping_states_canadian, string shipping_states_other, string shipping_states_us,
            bool ship_same_billing)
        {
            var dbAddressBilling = GetBillingAddress();
            

            dbAddressBilling.SetAddress(billing_first_name, billing_last_name, billing_company, billing_address,
                                          billing_city, billing_postcode, billing_phone, billing_country, billing_states_canadian,
                                          billing_states_other, billing_states_us);

            if (!billing_citySelected.HasValue && !string.IsNullOrEmpty(billing_city))
            {
                repository.AddCity(billing_city);
            }

            var dbAddressShipping = GetShippingAddress();

            if (!ship_same_billing)
            {
                dbAddressShipping.SetAddress(shipping_first_name, shipping_last_name, shipping_company, shipping_address,
                                          shipping_city, shipping_postcode, shipping_phone, shipping_country, shipping_states_canadian,
                                          shipping_states_other, shipping_states_us);

                if (!shipping_citySelected.HasValue && !string.IsNullOrEmpty(shipping_city))
                {
                    repository.AddCity(shipping_city);
                }
            }
            else
            {
                repository.CopyDataMembers(dbAddressBilling, dbAddressShipping);
            }
        }

        private DBML.address GetBillingAddress()
        {
            // check org first
            if (target_org != null)
            {

                if (!target_org.billingAddressID.HasValue)
                {
                    target_org.address2 = new DBML.address();
                }

                return target_org.address2;
            }

            if (!target_order.billingAddressID.HasValue)
            {
                target_order.address1 = new DBML.address();
            }

            return target_order.address1;
        }

        private DBML.address GetShippingAddress()
        {
            // check org first
            if (target_org != null)
            {

                if (!target_org.shippingAddressID.HasValue)
                {
                    target_org.address1 = new DBML.address();
                }

                return target_org.address1;
            }

            if (!target_order.shippingAddressID.HasValue)
            {
                target_order.address = new DBML.address();
            }

            return target_order.address;
        }
    }
}