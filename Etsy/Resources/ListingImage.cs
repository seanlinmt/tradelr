using System;
using tradelr.Time;

namespace Etsy.Resources
{
    public class ListingImage
    {
        public int listing_image_id { get; set; }
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
        public int listing_id { get; set; }
        public int? rank { get; set; }
        public string url_75x75 { get; set; }
        public string url_170x135 { get; set; }
        public string url_570xN { get; set; }
        public string url_fullxfull { get; set; }

        public Listing Listing { get; set; }
    }
}
