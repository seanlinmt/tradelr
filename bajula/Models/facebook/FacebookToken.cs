using System.Collections.Generic;
using tradelr.DBML;

namespace tradelr.Models.facebook
{
    public class FacebookToken
    {
        public bool inFeed { get; set; }
        public string name { get; set; }
        public string category { get; set; }
        public string id { get; set; }
    }

    public static class FacebookTokenHelper
    {
        public static IEnumerable<FacebookToken> ToModel(this IEnumerable<facebook_token> tokens)
        {
            foreach (var token in tokens)
            {
                yield return new FacebookToken()
                                 {
                                     id = token.pageid,
                                     name = token.name
                                 };
            }
        }
    }
}