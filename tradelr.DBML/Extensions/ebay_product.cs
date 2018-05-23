using System;
using System.Linq;

namespace tradelr.DBML
{
    public partial class ebay_product
    {
        public string ToExternalLink()
        {
#if DEBUG
            return string.Format("http://cgi.sandbox.ebay.com/ws/eBayISAPI.dll?ViewItem&item={0}", ebayid);
#else
            return string.Format("http://cgi.ebay.com/ws/eBayISAPI.dll?ViewItem&item={0}", ebayid);
#endif
        }
    }
}
