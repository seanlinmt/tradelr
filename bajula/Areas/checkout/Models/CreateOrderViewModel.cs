namespace tradelr.Areas.checkout.Models
{
    public class CreateOrderViewModel
    {
        public string cartid { get; set; }

        public ShippingViewModel shipping { get; set; }
        public PaymentViewModel payment { get; set; }

        public bool isDigitalOrder { get; set; }
        public bool hasShippingMethods { get; set; }

        public CreateOrderViewModel()
        {
            shipping = new ShippingViewModel();
            payment = new PaymentViewModel();
        }
    }
}