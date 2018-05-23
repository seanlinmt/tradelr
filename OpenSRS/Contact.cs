using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Serialization;

namespace OpenSRS
{
    public class Contact
    {
        public string first_name { get; set; }
        public string last_name { get; set; }
        public string org_name { get; set; }
        public string address1 { get; set; }
        public string address2 { get; set; }
        public string address3 { get; set; }
        public string city { get; set; }
        public string state { get; set; }
        public string postcode { get; set; }
        public string countrycode { get; set; }
        public string phone { get; set; }
        public string fax { get; set; }
        public string email { get; set; }

        public Contact()
        {
            address1 = null;
            address2 = null;
            address3 = null;
        }

        public Contact(string first_name, string last_name, string org_name, string address, string city, string state, string postcode,
            string countrycode, string phone, string fax, string email) : this()
        {
            this.first_name = first_name;
            this.last_name = last_name;
            this.org_name = org_name;

            var address_segments = Regex.Split(address, "\r\n|\r|\n");
            int count = 0;
            
            foreach (var addressSegment in address_segments)
            {
                switch (count++)
                {
                    case 0:
                        address1 = addressSegment;
                        break;
                    case 1:
                        address2 = addressSegment;
                        break;
                    case 2:
                        address3 = addressSegment;
                        break;
                }
            }

            this.city = city;
            this.state = state;
            this.postcode = postcode;
            this.countrycode = countrycode;
            this.phone = phone;
            this.fax = fax == String.Empty ? null : fax;
            this.email = email;

        }
    }
}
