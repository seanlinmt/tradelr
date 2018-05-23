using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace tradelr.Areas.dashboard.Models.product.ebay
{
    public class EbayEndListingViewModel
    {
        public string itemid { get; set; }
        public IEnumerable<SelectListItem> reason { get; set; } 
    }
}