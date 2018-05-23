using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using tradelr.Areas.dashboard.Models;
using tradelr.Areas.dashboard.Models.account.payment;
using tradelr.Models.payment;

namespace tradelr.Areas.checkout.Models
{
    public class OrderCompletedViewModel : BaseViewModel
    {
        public string store_url { get; set; }
        public string order_number { get; set; }
        public string order_url { get; set; }
        public PaymentMethodType paymentType { get; set; }
        public PaymentMethodViewModel paymentmethod { get; set; }
        public List<CartItem> cart_items { get; set; }

        public OrderCompletedViewModel(BaseViewModel viewmodel) : base(viewmodel)
        {
            paymentmethod = new PaymentMethodViewModel();
            cart_items = new List<CartItem>();
        }
    }
}