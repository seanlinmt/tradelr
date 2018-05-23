using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DotLiquid;
using tradelr.DBML;
using tradelr.Models.subdomain;

namespace tradelr.Models.liquid.models
{
    public class Collections : Drop, IEnumerable<Collection>
    {
        public IEnumerable<Collection> all { get; set; }
        public int size
        {
            get { return all.Count(); }
        }

        public override object BeforeMethod(string method)
        {
            method = method.Trim();
            return all.SingleOrDefault(x => string.Compare(x.handle, method,true) == 0); 
        }

        public Collections(MASTERsubdomain sd, long? viewerid)
        {
            all =
                sd.product_collections.Select(
                    x => new Collection(x, null, viewerid));
        }

        public IEnumerator<Collection> GetEnumerator()
        {
            return all.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}