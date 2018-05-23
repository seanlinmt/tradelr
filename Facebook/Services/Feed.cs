using System.Collections.Specialized;
using clearpixels.Facebook.Resources;

namespace clearpixels.Facebook.Services
{
    public class Feed : RestBase
    {
        protected internal Feed(string token)
            : base(token)
        {
        }

        public ResponseCollection<Post> GetFeed(string id, long? since, long? until)
        {
            method = "GET";
            var parameters = new NameValueCollection();
            if (since.HasValue)
            {
                parameters.Add("since", since.ToString());
            }
            if (until.HasValue)
            {
                parameters.Add("until", until.ToString());
            }
            // include connections
            parameters.Add("metadata", "1");
            return SendRequest<ResponseCollection<Post>>(id + "/feed", parameters);
        }

        public ResponseCollection<Post> GetHomeFeed(long? since, long? until)
        {
            method = "GET";
            var parameters = new NameValueCollection();
            if (since.HasValue)
            {
                parameters.Add("since", since.ToString());
            }
            if (until.HasValue)
            {
                parameters.Add("until", until.ToString());
            }
            // include connections
            parameters.Add("metadata", "1");
            return SendRequest<ResponseCollection<Post>>("me/home", parameters);
        }

        public Id PostToHomeFeed(string id, string message, string url = null, string name = null, 
            string description = null, string picture = null)
        {
            method = "POST";
            var parameters = new NameValueCollection();
            if (url != null)
            {
                parameters.Add("link", url);
            }
            if (name != null)
            {
                parameters.Add("name", name);
            }
            if (description != null)
            {
                parameters.Add("description", description);
            }
            if (picture != null)
            {
                parameters.Add("picture", picture);
            }
            parameters.Add("message", message);
            return SendRequest<Id>(id + "/feed", parameters);
        }

        public ResponseCollection<Comment> GetComments(string postid)
        {
            method = "GET";
            return SendRequest<ResponseCollection<Comment>>(postid + "/comments");
        }

        public Comment GetSingleComment(string id)
        {
            method = "GET";
            return SendRequest<Comment>(id);
        }

        public Id PostComment(string postid, string message)
        {
            method = "POST";
            var parameters = new NameValueCollection();
            parameters.Add("message", message);
            return SendRequest<Id>(postid + "/comments", parameters);
        }
    }
}
