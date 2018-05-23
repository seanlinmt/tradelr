using System;

namespace tradelr.DBML
{
    public partial class address
    {
        public void SetAddress(string first_name, string last_name, string company, string address, string city,
            string postcode, string phone, int? country, string states_canadian, string states_other, string states_us)
        {
            this.first_name = first_name;
            this.last_name = last_name;
            organisation_name = company;
            
            this.city = city;
            street_address = address;
            this.phone = phone;
            this.postcode = postcode;

            this.country = country;

            if (country.HasValue && country.Value == 185 && !string.IsNullOrEmpty(states_us))
            {
                state = states_us;
            }
            else if (country.HasValue && country.Value == 32 && !string.IsNullOrEmpty(states_canadian))
            {
                state = states_canadian;
            }
            else
            {
                state = states_other;
            }
        }

        partial void OncountryChanging(Nullable<int> value)
        {
            if (value.HasValue && value.Value == 0)
            {
                throw new ArgumentException("Country cannot be zero");
            }
        }
    }
}
