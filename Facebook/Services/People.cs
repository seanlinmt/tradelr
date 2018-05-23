using System.Collections.Generic;
using clearpixels.Facebook.Resources;

namespace clearpixels.Facebook.Services
{
    public class People : RestBase
    {
        protected internal People(string token)
            : base(token)
        {
        }

        public User GetUser(string id)
        {
            method = "GET";
            return SendRequest<User>(id);
        }

        public ResponseCollection<IdName> GetFriends(string id)
        {
            method = "GET";
            return SendRequest<ResponseCollection<IdName>>(id + "/friends");
        }

        public ResponseCollection<Permissions> GetPermissions(string id)
        {
            method = "GET";
            return SendRequest<ResponseCollection<Permissions>>(id + "/permissions");
        }
    }
}
