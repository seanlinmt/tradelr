namespace Etsy.Resources
{
    public class ShippingTemplateEntry
    {
        public int shipping_template_entry_id { get; set; }
        public int shipping_template_id { get; set; }
        public string currency_code { get; set; } 	
        public int origin_country_id  { get; set; } 	
        public int? destination_country_id  { get; set; }
        public int? destination_region_id { get; set; } 	
        public decimal primary_cost { get; set; }
        public decimal secondary_cost { get; set; }

        public Country DestinationCountry { get; set; }
        public Region DestinationRegion { get; set; }
        public Country OriginCountry { get; set; }
        public ShippingTemplate Template { get; set; }
    }
}
