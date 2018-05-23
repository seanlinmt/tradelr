namespace Shipwire.tracking
{
    public class TrackingUpdate
    {
        public string EmailAddress { get; set; }
        public string Password { get; set; }
        public string Server { get; set; }
        public string Bookmark { get; set; }
        public string OrderNo { get; set; }
        public string ShipwireId { get; set; }

        public TrackingUpdate()
        {
            
        }

        public TrackingUpdate(string email, string password)
        {
            EmailAddress = email;
            Password = password;
#if DEBUG
            Server = "Test";
#else
            Server = "Production";
#endif
        }
    }
}
