using DotLiquid;
using tradelr.DBML;
using tradelr.DBML.Helper;
using tradelr.Models.users;

namespace tradelr.Models.liquid.models
{
    public class Page : Drop
    {
        public Page(page page)
        {
            title = page.name;
            handle = page.permalink;
            url = "/pages/" + handle.ToLower();
            content = page.content;
            author = page.user.ToFullName();
        }

        public string title { get; set; }
        public string handle { get; set; }
        public string url { get; set; }
        public string content { get; set; }
        public string author { get; set; }

    }
}