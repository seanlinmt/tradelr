using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace tradelr.Areas.checkout.Models
{
    public enum CheckoutStatus
    {
        SHIPPING_FAIL,
        SHIPPING_NONE,
        SHIPPING_OK
    }
}