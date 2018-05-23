using System;

namespace clearpixels.Facebook.Resources
{
    public class Comment
    {
        public string id { get; set; }
        public IdName from { get; set; }
        public string message { get; set; }
        public DateTime created_time { get; set; }
        public DateTime updated_time { get; set; }
        
    }
}
