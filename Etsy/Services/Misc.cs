using System.Collections.Generic;
using Etsy.Resources;

namespace Etsy.Services
{
    public class Misc : RestBase
    {
        public Misc(SessionInfo session)
        {
            info = session;
        }

        public IEnumerable<Country> findAllCountries()
        {
            method = "GET";
            URI = "/countries";

            var response = SendRequest<Country>();
            return response == null? null: response.results;
        }

        public IEnumerable<Region> findAllRegion()
        {
            method = "GET";
            URI = "/regions";

            var response = SendRequest<Region>();
            return response == null ? null : response.results;
        }
    }
}
