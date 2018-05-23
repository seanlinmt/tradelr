
using System.Collections.Generic;

namespace clearpixels.Facebook.Resources
{
    /// <summary>
    /// this isn't part of facebook, we need it because we can't get album photo without access token
    /// </summary>
    public class AlbumsWithToken
    {
        public IEnumerable<Album> albums { get; set; }
        public string access_token { get; set; }
    }
}