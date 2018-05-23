using System.Collections.Generic;

namespace Etsy.Resources
{
    public class ShippingTemplate
    {
        public int shipping_template_id { get; set; }
        public string title { get; set; }
        public string user_id { get; set; }

        public List<ShippingTemplateEntry> Entries { get; set; }
    }
}
