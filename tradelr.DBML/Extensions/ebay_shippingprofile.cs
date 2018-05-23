using System;
using System.Linq;
using tradelr.Library.JSON;

namespace tradelr.DBML
{
    public partial class ebay_shippingprofile
    {
        public ErrorData HaveValidDomesticRules()
        {
            if (ebay_shippingrules.All(x => x.ebay_shippingservice.isInternational))
            {
                return new ErrorData()
                {
                    message = "You must specify at least one eBay domestic shipping service",
                    success = false
                };
            }

            return new ErrorData()
            {
                message = "",
                success = true
            };
        }
    }
}
