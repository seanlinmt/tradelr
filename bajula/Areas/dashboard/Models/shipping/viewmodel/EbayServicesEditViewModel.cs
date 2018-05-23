using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace tradelr.Areas.dashboard.Models.shipping.viewmodel
{
    public class EbayServicesEditViewModel
    {
        public IEnumerable<SelectListItem> servicesList { get; set; }
        public IEnumerable<SelectListItem> locationList { get; set; } 
        public bool isInternational { get; set; }
    }
}