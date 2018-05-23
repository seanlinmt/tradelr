using System.Collections.Generic;
using System.Web.Mvc;
using tradelr.Models.facebook;
using tradelr.Models.google;

namespace tradelr.Models.networks.viewmodels
{
    public class NetworkViewModel
    {
        public string requestUrl { get; set; }

        public string bloggerSessionKey { get; set; }
        public IEnumerable<GoogleBlogData> blogList { get; set; }
        
        // facebook
        public string FacebookProfileUrl { get; set; }
        public IEnumerable<FacebookToken> FacebookFeeds { get; set; }
        public List<SelectListItem> facebookStreams { get; set; }

        public string myspaceID { get; set; }
        public string hi5ID { get; set; }

        // shipwire
        public string shipwireEmail { get; set; }
        public string shipwirePassword { get; set; }

        public NetworkViewModel()
        {
            FacebookFeeds = new List<FacebookToken>();
        }
    }
}