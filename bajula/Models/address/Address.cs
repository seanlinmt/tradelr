using System.Text;
using Shipwire.order;
using tradelr.DBML;
using tradelr.DBML.Helper;
using tradelr.Library;
using tradelr.Library.geo;

namespace tradelr.Models.address
{
    public class Address
    {
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string companyName { get; set; }
        public string streetAddress { get; set; }
        public string city { get; set; }
        public string postcode { get; set; }
        public string state { get; set; }
        public string country { get; set; }
        public int? countryid { get; set; }
        public string phone { get; set; }

        public Address()
        {
            streetAddress = "";
        }
    }

    public static class AddressHelper
    {
        public static Address ToModel(this DBML.address row)
        {
            if (row == null)
            {
                return new Address();
            }

            return new Address()
                       {
                           firstName = row.first_name,
                           lastName = row.last_name,
                           companyName = row.organisation_name,
                           streetAddress = row.street_address,
                           city = row.city,
                           postcode = row.postcode,
                           state = row.state,
                           countryid = row.country,
                           country = row.country.ToCountry().name,
                           phone = row.phone
                       };
        }

        public static string ToHtmlString(this DBML.address row)
        {
            if (row == null)
            {
                return "";
            }

            var hasValue = false;

            var sb = new StringBuilder();
            sb.Append("<ul style=\"list-style: none outside none;\">");
            if (!string.IsNullOrEmpty(row.first_name) && !string.IsNullOrEmpty(row.last_name))
            {
                sb.AppendFormat("<li>{0} {1}</li>", row.first_name, row.last_name);
                hasValue = true;
            }

            if (!string.IsNullOrEmpty(row.organisation_name))
            {
                sb.AppendFormat("<li>{0}</li>", row.organisation_name);
                hasValue = true;
            }

            if (!string.IsNullOrEmpty(row.street_address))
            {
                sb.AppendFormat("<li>{0}</li>", row.street_address.ToHtmlBreak());
                hasValue = true;
            }

            sb.AppendFormat("<li>{0}</li><li>{1} {2}</li>",
                row.city,
                row.state.ToStateName(row.country.HasValue ? row.country.Value.ToString() : ""),
                row.postcode);


            if (row.country.HasValue)
            {
                sb.AppendFormat("<li>{0}</li>", Country.GetCountry(row.country.Value).name);
                hasValue = true;
            }

            sb.Append("</ul>");

            if (!hasValue)
            {
                return "";
            }

            return sb.ToString();
        }

    }
}