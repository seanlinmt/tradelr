namespace Etsy.Resources
{
    public class ShippingInfo
    {
        public int shipping_info_id { get; set; }
        public int origin_country_id { get; set; }
        public int? destination_country_id { get; set; }
        public string currency_code { get; set; }
        public decimal primary_cost { get; set; }
        public decimal secondary_cost { get; set; }
        public int listing_id { get; set; }
        public int? region_id { get; set; }
        public string origin_country_name { get; set; }
        public string destination_country_name { get; set; }

        public Country DestinationCountry { get; set; }
        public Country OriginCountry { get; set; }
        public Region Region { get; set; }

    }
}
