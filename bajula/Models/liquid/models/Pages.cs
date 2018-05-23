using System.Collections.Generic;
using System.Linq;
using DotLiquid;
using tradelr.DBML;

namespace tradelr.Models.liquid.models
{
    public class Pages : Drop
    {
        public IEnumerable<Page> all { get; private set; }
        
        public Pages(MASTERsubdomain sd)
        {
            all = sd.pages.Select(x => new Page(x));
        }

        public override object BeforeMethod(string method)
        {
            method = method.Trim();
           return all.Where(x => string.Compare(x.handle, method, true) == 0).SingleOrDefault(); 
        }
    }
}