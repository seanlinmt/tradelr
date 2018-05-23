using clearpixels.Facebook.Services;

namespace clearpixels.Facebook
{
    public class FacebookService 
    {
        public Media Media { get; set; }
        public People People { get; set; }
        public Feed Feed { get; set; }
        public Search Search { get; set; }
        public Services.Account Account { get; set; }

        public FacebookService(string token)
        {
            Media = new Media(token);
            People = new People(token);
            Feed = new Feed(token);
            Search = new Search(token);
            Account = new Services.Account(token);
        }

        

        

        
        

        

        
    }
}
