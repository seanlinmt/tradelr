using System.Collections.Generic;
using DotLiquid;
using tradelr.Library;

namespace tradelr.Models.liquid.models.LinkList
{
    public class LinkList :Drop
    {
        public string title { get; private set; }
        public string handle { get; private set; }

        public IEnumerable<Link> links { get; set; }

        public LinkList(string name, string handle)
        {
            title = name;
            this.handle = handle;
        }
    }
}