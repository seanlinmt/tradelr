using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;

namespace Ebay.Resources
{
    public class ListingVariant
    {
        public string sku { get; set; }
        public int quantity { get; set; }
        public NameValueCollection properties { get; set; }

        public ListingVariant()
        {
            properties = new NameValueCollection();
        }
    }
}
