using Etsy.Services;

namespace Etsy
{
    public class EtsyService
    {
        public User User { get; set;}
        public Shop Shop { get; set; }
        public Listing Listing { get; set; }
        public Misc Misc { get; set; }
        public Taxonomy Taxonomy { get; set; }

        public EtsyService()
            :this("","")
        {
            
        }

        public EtsyService(string tokenKey, string tokenSecret)
        {
            var session = new SessionInfo(tokenKey, tokenSecret);
            User = new User(session);
            Shop = new Shop(session);
            Listing = new Listing(session);
            Misc = new Misc(session);
            Taxonomy = new Taxonomy(session);
        }

        

    }
}
