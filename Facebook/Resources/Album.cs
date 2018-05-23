using System;

namespace clearpixels.Facebook.Resources
{
    public class Album
    {
        public string id { get; set; }
        public IdName from { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public string location { get; set; }
        public string link { get; set; }
        public string privacy { get; set; }
        public int count { get; set; }
        public DateTime created_time { get; set; }
        public DateTime updated_time { get; set; }
    }
}
