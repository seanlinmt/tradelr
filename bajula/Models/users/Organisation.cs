using System.Collections.Generic;
using System.Linq;
using System.Text;
using tradelr.Common;
using tradelr.Common.Models.currency;
using tradelr.DataAccess;
using tradelr.DBML;
using tradelr.Libraries;
using tradelr.Libraries.Helpers;
using tradelr.Library;
using tradelr.Library.geo;
using tradelr.Models.subdomain;

namespace tradelr.Models.users
{
    // changing the following will break contacts/add.js
    public class Organisation
    {
        public string id { get; set; }
        public string companyName { get; set; }
        public string address { get; set; }
        public string city { get; set; }
        public string coPhone { get; set; }
        public string postcode { get; set; }
        public string fax { get; set; }
        public int? country { get; set; }
        public string state { get; set; }
        public int? currency { get; set; }
    }

    public static class OrganisationHelper
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="org"></param>
        /// <param name="standAlone">true: includes ul tag</param>
        /// <returns></returns>
        public static string ToOrganisationAddress(this organisation org, bool standAlone, bool useBreaks = false)
        {
            var sb = new StringBuilder();
            if (standAlone)
            {
                sb.Append("<ul>");
            }
            if (!string.IsNullOrEmpty(org.address))
            {
                if (useBreaks)
                {
                    sb.AppendFormat("{0}<br />", org.address.ToHtmlBreak());
                }
                else
                {
                    sb.AppendFormat("<li>{0}</li>", org.address.ToHtmlBreak());
                }
                
            }

            if (useBreaks)
            {
                sb.AppendFormat("{0}<br />{1} {2}<br />",
                org.city.HasValue ? org.MASTERcity.name : "",
                org.state.ToStateName(org.country.HasValue?org.country.Value.ToString():""),
                org.postcode);
            }
            else
            {
                sb.AppendFormat("<li>{0}</li><li>{1} {2}</li>",
                org.city.HasValue ? org.MASTERcity.name : "",
                org.state.ToStateName(org.country.HasValue ? org.country.Value.ToString() : ""),
                org.postcode);
            }
            

            if (org.country.HasValue)
            {
                if (useBreaks)
                {
                    sb.AppendFormat("{0}<br />", Country.GetCountry(org.country.Value).name);
                }
                else
                {
                    sb.AppendFormat("<li>{0}</li>", Country.GetCountry(org.country.Value).name);
                }
            }

            if (standAlone)
            {
                sb.Append("</ul>");
            }
            
            return sb.ToString();
        }

        public static string ToFullOrganisationAddress(this organisation org)
        {
            var sb = new StringBuilder();
            sb.Append("<ul><li>");
            sb.Append(org.name);
            sb.Append("</li>");

            sb.Append(org.ToOrganisationAddress(false));

            if (!string.IsNullOrEmpty(org.phone))
            {
                sb.Append("<li>");
                sb.Append(org.phone);
                sb.Append("</li>");
            }

            return sb.ToString();
        }

        public static IEnumerable<Organisation> ToModelWithCurrency(this IEnumerable<organisation> values)
        {
            foreach (var value in values)
            {
                var currency = value.MASTERsubdomain.currency.ToCurrency();
                yield return new Organisation
                {
                    currency = currency.id,
                    companyName = value.name,
                    id = value.id.ToString()
                };
            }
        }

        public static Organisation ToModel(this organisation org)
        {
            return new Organisation()
                       {
                           address = org.address,
                           city = org.city.HasValue?org.MASTERcity.name:"",
                           companyName = org.name,
                           coPhone = org.phone,
                           country = org.country,
                           fax = org.fax,
                           id = org.id.ToString(),
                           postcode = org.postcode,
                           state = org.state
                       };
        }
    }

}
