using System.Collections.Generic;
using System.Web.Mvc;
using tradelr.Models;

namespace tradelr.Areas.dashboard.Models.shipping.viewmodel
{
    public class ShippingViewModel : BaseViewModel
    {
        public IEnumerable<SelectListItem> shippingProfiles { get; set; }
        public IEnumerable<SelectListItem> ebay_sites { get; set; }

        public ShippingViewModel(BaseViewModel baseviewmodel)
            : base(baseviewmodel)
        {
            
        }
    }
}