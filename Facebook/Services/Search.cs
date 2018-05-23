using System.Collections.Specialized;
using clearpixels.Facebook.Resources;

namespace clearpixels.Facebook.Services
{
    public class Search : RestBase
    {
        protected internal Search(string token) : base(token)
        {

        }

        public ResponseCollection<Post> SearchPosts(string q, string userid)
        {
            method = "GET";
            var parameters = new NameValueCollection { { "q", q } };
            if (!string.IsNullOrEmpty(userid))
            {
                return SendRequest<ResponseCollection<Post>>(userid + "/posts", parameters);
            }
            parameters.Add("type", "post");
            return SendRequest<ResponseCollection<Post>>("search", parameters);
        }

        public ResponseCollection<IdName> SearchUsers(string q)
        {
            method = "GET";
            var parameters = new NameValueCollection { { "q", q }, { "type", "user" } };
            return SendRequest<ResponseCollection<IdName>>("search", parameters);
        }

        public ResponseCollection<Page> SearchPages(string q, int limit, int offset)
        {
            method = "GET";
            var parameters = new NameValueCollection
                                 {
                                     {"q", q}, 
                                     {"type", "page"},
                                     {"limit",limit.ToString()},
                                     {"offset", offset.ToString()}
                                 };
            return SendRequest<ResponseCollection<Page>>("search", parameters);
        }
    }
}
