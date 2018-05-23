using System.Collections.Generic;
using Etsy.Resources;

namespace Etsy.Services
{
    public class Taxonomy : RestBase
    {
        public Taxonomy(SessionInfo session)
        {
            info = session;
        }

        public IEnumerable<Category> findAllTopCategory(string fields = "")
        {
            method = "GET";
            URI = "/taxonomy/categories";
            var parameters = new Parameters();
            parameters.AddParameter("fields", fields);

            var response = SendRequest<Category>(parameters);
            return response == null? null: response.results;
        }

        public IEnumerable<Category> findAllTopCategoryChildren(string tag, string fields = "")
        {
            method = "GET";
            URI = "/taxonomy/categories/:tag";
            id.tag_id = tag;
            var parameters = new Parameters();
            parameters.AddParameter("fields", fields);

            var response = SendRequest<Category>(parameters);
            return response == null ? null : response.results;
        }

        public IEnumerable<Category> findAllSubCategoryChildren(string tag, string subtag, string fields = "")
        {
            method = "GET";
            URI = "/taxonomy/categories/:tag/:subtag";
            id.tag_id = tag;
            id.subtag_id = subtag;
            var parameters = new Parameters();
            parameters.AddParameter("fields", fields);

            var response = SendRequest<Category>(parameters);
            return response == null ? null : response.results;
        }
    }
}
