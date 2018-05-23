using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using tradelr.Models.payment;

namespace tradelr.Areas.checkout.Models
{
    public class PaymentViewModel
    {
        public PaymentMethodList paymentMethods { get; set; }

        public PaymentViewModel()
        {
            paymentMethods = new PaymentMethodList();
        }
    }
}