namespace tradelr.Models.contacts
{
    public class ContactView
    {
        // user details
        public string name { get; set; }
        public string email { get; set; }
        public string lastLogin { get; set; }

        // organisation details
        public string orgname { get; set; }
        public string phone { get; set; }
        public string fax { get; set; }
        public string address { get; set; }
    }
}
