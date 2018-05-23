using Etsy.Resources;

namespace Etsy.Services
{
    public class Listing : RestBase
    {
        public Listing(SessionInfo session)
        {
            base.info = session;
        }

        // listings
        public Resources.Listing createListing(int quantity, string title, string description, decimal price, 
            string tags, long shipping_template_id, string materials = "", long? shop_section_id = null)
        {
            method = "POST";
            URI = "/listings";
            var parameters = new Parameters();
            parameters.AddParameter("quantity", quantity.ToString());
            parameters.AddParameter("title", title);
            parameters.AddParameter("description", description);
            parameters.AddParameter("price", price.ToString());
            parameters.AddParameter("tags", tags);
            parameters.AddParameter("shipping_template_id", shipping_template_id.ToString());
            if (!string.IsNullOrEmpty(materials))
            {
                parameters.AddParameter("materials", materials);
            }
            if (shop_section_id.HasValue)
            {
                parameters.AddParameter("shop_section_id", shop_section_id.Value.ToString());
            }
            var response = SendRequest<Resources.Listing>(parameters);
            return response == null ? null : response.results[0];
        }

        public void deleteListing(long listing_id)
        {
            method = "DELETE";
            URI = "/listings/:listing_id";
            id.listing_id = listing_id.ToString();
            var parameters = new Parameters();

            SendRequest<Resources.Listing>(parameters);
        }

        public Resources.Listing getListing(long listing_id, string fields = null, string includes = null)
        {
            method = "GET";
            URI = "/listings/:listing_id";
            id.listing_id = listing_id.ToString();
            var parameters = new Parameters();
            parameters.AddParameter("fields", fields);
            parameters.AddParameter("includes", includes);

            var response = SendRequest<Resources.Listing>(parameters);
            return response == null ? null : response.results[0];
        }

        public Resources.Listing updateListing(long listing_id, bool renew, int quantity, string title, string description, decimal price,
            string tags, long? shipping_template_id = null, string materials = "", long? shop_section_id = null, string state = "active")
        {
            method = "PUT";
            URI = "/listings/:listing_id";
            id.listing_id = listing_id.ToString();
            var parameters = new Parameters();
            parameters.AddParameter("renew", renew?"true":"false");
            parameters.AddParameter("state", state);
            parameters.AddParameter("quantity", quantity.ToString());
            parameters.AddParameter("title", title);
            parameters.AddParameter("description", description);
            parameters.AddParameter("price", price.ToString());
            parameters.AddParameter("tags", tags);
            parameters.AddParameter("shipping_template_id", shipping_template_id.ToString());
            parameters.AddParameter("materials", materials);
            parameters.AddParameter("shop_section_id", shop_section_id.ToString());
            var response = SendRequest<Resources.Listing>(parameters);
            return response == null ? null : response.results[0];
        }

        public ListingImage uploadListingImage(string listing_id, string path)
        {
            method = "POST";
            URI = "/listings/:listing_id/images";
            id.listing_id = listing_id;
            filePath = path;
            var parameters = new Parameters();

            var response = SendRequest<ListingImage>(parameters, true);
            return response == null ? null : response.results[0];
        }

        // shipping templates
        public ShippingTemplate createShippingTemplate(string title, int origin_country_id, int? destination_country_id,
            decimal primary_cost, decimal secondary_cost, int? destination_region_id)
        {
            method = "POST";
            URI = "shipping/templates";
            var parameters = new Parameters();
            parameters.AddParameter("title", title);
            parameters.AddParameter("origin_country_id", origin_country_id.ToString());
            parameters.AddParameter("destination_country_id", destination_country_id.ToString());
            parameters.AddParameter("primary_cost", primary_cost.ToString());
            parameters.AddParameter("secondary_cost", secondary_cost.ToString());
            parameters.AddParameter("destination_region_id", destination_region_id.ToString());

            var data = SendRequest<ShippingTemplate>(parameters);
            return data == null? null: data.results[0];
        }

        public ShippingTemplateEntry createShippingTemplateEntry(long shipping_template_id, int? destination_country_id,
            decimal primary_cost, decimal secondary_cost, int? destination_region_id)
        {
            method = "POST";
            URI = "shipping/templates/entries";
            var parameters = new Parameters();
            parameters.AddParameter("shipping_template_id", shipping_template_id.ToString());
            parameters.AddParameter("destination_country_id", destination_country_id.ToString());
            parameters.AddParameter("primary_cost", primary_cost.ToString());
            parameters.AddParameter("secondary_cost", secondary_cost.ToString());
            parameters.AddParameter("destination_region_id", destination_region_id.ToString());

            var data = SendRequest<ShippingTemplateEntry>(parameters);
            return data == null ? null : data.results[0];
        }
    }
}
