using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace tradelr.Models.transactions.viewmodel
{
    public class EbayOrderShipViewModel
    {
        public long orderID { get; set; }
        public string shippingService { get; set; }
    }
}