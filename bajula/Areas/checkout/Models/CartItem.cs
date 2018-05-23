using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace tradelr.Areas.checkout.Models
{
    public class CartItem
    {
        public string thumbnail_url { get; set; }
        public string name { get; set; }
        public int quantity { get; set; }

    }
}