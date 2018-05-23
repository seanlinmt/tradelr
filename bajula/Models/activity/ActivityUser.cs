using System;
using clearpixels.Facebook.Resources;
using tradelr.Library.Constants;

namespace tradelr.Models.activity
{
    public class ActivityUser
    {
        // basic
        public string id { get; set; }
        public string profileImage { get; set; }
        public string profileLink { get; set; }
        public string about { get; set; }
        public string location { get; set; }
        public string fullName { get; set; }

        // fb stuff
        public string screenName { get; set; }
        
    }

    public static class ActivityUserHelper
    {
        public static ActivityUser ToModel(this User val)
        {
            var profileUrl = val.link;
            var u = new ActivityUser
            {
                about = val.about,
                fullName = val.name,
                id = val.id,
                profileImage = string.Concat(GeneralConstants.FACEBOOK_GRAPH_HOST, val.id, "/picture?type=large"),
                profileLink = profileUrl
            };
            return u;
        }
    }
}