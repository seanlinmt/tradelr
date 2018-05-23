using System.Collections.Generic;

namespace tradelr.Areas.dashboard.Models.store.navigation
{
    public class LinksViewModel
    {
        public string selectablesString { get; set; }
        public IEnumerable<LinkList> linklists { get; set; } 
    }
}