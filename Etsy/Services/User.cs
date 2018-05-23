using System.Collections.Generic;
using Etsy.Resources;

namespace Etsy.Services
{
    public class User : RestBase
    {
        public User(SessionInfo session)
        {
            info = session;
        }

        public IEnumerable<ShippingTemplate> findAllUserShippingTemplates(string user_id = "", string fields = null, string includes = null)
        {
            method = "GET";
            URI = "/users/:user_id/shipping/templates";
            id.user_id = user_id;
            var parameters = new Parameters();
            parameters.AddParameter("fields", fields);
            parameters.AddParameter("includes", includes);

            var response = SendRequest<ShippingTemplate>(parameters);
            return response == null ? null : response.results;
        }

        public Resources.User getUser(string user_id = "", string fields = null, string includes = null)
        {
            method = "GET";
            URI = "/users/:user_id";
            id.user_id = user_id;
            var parameters = new Parameters();
            parameters.AddParameter("fields", fields);
            parameters.AddParameter("includes", includes);

            var response = SendRequest<Resources.User>(parameters);
            return response == null? null: response.results[0];
        }
    }
}
