using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using api.trademe.co.nz.v1;

namespace TradeMe.models
{
    public class EditListingRequest : ListingRequest
    {
        public int ListingID;
    }
}
