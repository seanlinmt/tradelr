using System.Text;
using tradelr.Library.geo;

namespace tradelr.DBML
{
    public partial class organisation
    {
        public string ToFullAddress()
        {
            var sb = new StringBuilder();
            sb.AppendLine(address);
            sb.AppendFormat("{0}\r\n{1} {2}\r\n",
                city.HasValue ? MASTERcity.name : "",
                state.ToStateName(country.HasValue ? country.Value.ToString() : ""),
                postcode);

            if (country.HasValue)
            {
                sb.AppendLine(Country.GetCountry(country.Value).name);
            }

            return sb.ToString();
        }

        
    }
}
