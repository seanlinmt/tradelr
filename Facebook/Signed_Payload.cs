namespace clearpixels.Facebook
{
    public class Signed_Payload
    {
        public string algorithm { get; set; }
        public long expires { get; set; }
        public long issued_at { get; set; }
        public string oauth_token { get; set; }
        public Page page { get; set; }
        public User user { get; set; }
        public string user_id { get; set; }

        public class Page
        {
            public string id { get; set; }
            public bool liked { get; set; }
            public bool admin { get; set; }
        }

        public class User
        {
            public string country { get; set; }
            public string locale { get; set; }
            public Age age { get; set; }
        }

        public class Age
        {
            public int min { get; set; }
        }
    }
}
