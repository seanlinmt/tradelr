using System;
using tradelr.Time;

namespace Etsy.Resources
{
    public class FavoriteUser
    {
        public int user_id { get; set; }
        public int target_user_id { get; set; }
        public DateTime creation
        {
            get { return UnixTime.ToDateTime(creation_tsz); }
            set { creation_tsz = UnixTime.ToDouble(value); }
        }

        public double creation_tsz { get; set; }

        public User TargetUser { get; set; }
        public User User { get; set; }
    }
}
