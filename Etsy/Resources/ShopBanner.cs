using System;
using tradelr.Time;

namespace Etsy.Resources
{
    public class ShopBanner
    {
        public int shop_banner_id { get; set; }
        public string hex_code { get; set; }
        public int red { get; set; }
        public int green { get; set; }
        public int blue { get; set; }
        public int hue { get; set; }
        public int saturation { get; set; }
        public int brightness { get; set; }
        public bool is_black_and_white { get; set; }

        public DateTime creation
        {
            get { return UnixTime.ToDateTime(creation_tsz); }
            set { creation_tsz = UnixTime.ToDouble(value); }
        }

        public double creation_tsz { get; set; }
        public int user_id { get; set; }

        public Type Field { get; set; }
        public Shop Shop { get; set; }
        public User User { get; set; }

    }
}
