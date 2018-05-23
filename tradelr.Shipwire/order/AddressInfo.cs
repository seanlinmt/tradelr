using System.Text.RegularExpressions;
using System.Xml.Serialization;

namespace Shipwire.order
{
    public class AddressInfo
    {
        [XmlAttribute(AttributeName = "type")]
        public string Type { get; set; }
        public Name Name { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
        public string Zip { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }

        public AddressInfo()
        {
            
        }

        public AddressInfo(string fullname, string address, string city, string state, string country, string zip, string phone, string email)
        {
            Type = "ship";
            Name = new Name(){ Full = fullname};

            if (address.Contains("\n"))
            {
                var regex = new Regex("\n");
                var lines = regex.Split(address);
                Address1 = lines[0];
                Address2 = lines[1];
            }
            else
            {
                Address1 = address;
            }

            City = city;
            State = state;
            Country = country;
            Zip = zip;
            Phone = phone;
            Email = email;
        }
    }
}
