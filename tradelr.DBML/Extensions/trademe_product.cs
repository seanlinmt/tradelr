using System;
using System.Linq;

namespace tradelr.DBML
{
    public partial class trademe_product
    {
        public string ToExternalLink()
        {
#if DEBUG
            return string.Format("http://www.tmsandbox.co.nz/Browse/Listing.aspx?id={0}", listingid);
#else
            return string.Format("http://www.trademe.co.nz/Browse/Listing.aspx?id={0}", listingid);
#endif
        }
    }
}
