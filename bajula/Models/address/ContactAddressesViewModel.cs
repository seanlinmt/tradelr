namespace tradelr.Models.address
{
    public class ContactAddressesViewModel
    {
        public Address billing { get; set; }
        public Address shipping { get; set; }
        public bool sameBillingAndShipping { get; set; }
        public bool hideSameShippingCheckBox { get; set; }

        public ContactAddressesViewModel()
        {
            billing = new Address();
            shipping = new Address();
            sameBillingAndShipping = true;
        }
    }
}