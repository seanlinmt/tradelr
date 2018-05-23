namespace tradelr.Models.address
{
    public class CheckoutAddressViewModel : ContactAddressesViewModel
    {
        public string cartid { get; set; }
        public string buyer_name { get; set; }
    }
}