using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace tradelr.Areas.checkout.Models
{
    public class ShippingViewModel
    {
        public string shippingAddress { get; set; }
        public IEnumerable<SelectListItem> shippingMethods { get; set; }
    }
}