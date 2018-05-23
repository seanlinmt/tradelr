using System.Collections.Generic;
using System.Linq;
using DotLiquid;
using tradelr.Areas.dashboard.Models.store.navigation;
using tradelr.DBML;

namespace tradelr.Models.liquid.models.LinkList
{
    public class LinkLists : Drop
    {
        public IEnumerable<LinkList> all { get; set; }

        public override object BeforeMethod(string method)
        {
            method = method.Trim();
            return all.Where(x => string.Compare(x.handle, method, true) == 0).SingleOrDefault(); 
        }

        public LinkLists(MASTERsubdomain sd)
        {
            all = sd.linklists.Select(x => new LinkList(x.title, x.permalink)
                                               {
                                                   links = x.links.Select(y => new Link()
                                                                                   {
                                                                                       title = y.title,
                                                                                       url = y.url,
                                                                                       type = ((LinkType)y.type).ToLinkType()
                                                                                   })
                                               });
        }
    }
}