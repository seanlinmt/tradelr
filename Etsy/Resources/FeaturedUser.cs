using System;
using tradelr.Time;

namespace Etsy.Resources
{
    public class FeaturedUser
    {
        public int featured_user_id { get; set; }
        public string article_url { get; set; }
        public string excerpt { get; set; }
        public DateTime activeTime
        {
            get { return UnixTime.ToDateTime(active_time); }
            set { active_time = UnixTime.ToDouble(value); }
        }

        public double active_time { get; set; }
        public string image_url_160x160 { get; set; }
        public string image_url_155x90 { get; set; }
        public string image_url_fullxfull { get; set; }
        public Shop Shop { get; set; }
        public User User { get; set; }

    }
}
