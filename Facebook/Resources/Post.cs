using System;
using System.Collections.Generic;

namespace clearpixels.Facebook.Resources
{
    public class Post
    {
        public long likes { get; set; }
        public string id { get; set; }
        public IdName from { get; set; }
        public IdName to { get; set; }
        public string message { get; set; }
        public string picture { get; set; }
        public string link { get; set; }
        public string name { get; set; }
        public string caption { get; set; }
        public string description { get; set; }
        public string source { get; set; }
        public string icon { get; set; }
        public string attribution { get; set; }
        public List<NameLink> actions { get; set; }
        public DateTime created_time { get; set; }
        public DateTime updated_time { get; set; }
        public List<Property> properties { get; set; }
        public string type { get; set; }
        
        // connections
        public ResponseCollection<Comment> comments { get; set; }
    }
}
