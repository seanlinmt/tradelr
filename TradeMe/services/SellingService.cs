using System;
using TradeMe.models;
using api.trademe.co.nz.v1;

namespace TradeMe.services
{
    // http://developer.trademe.co.nz/api-documentation/selling-methods/
    public class SellingService : RestBase, Selling
    {
        public SellingService(string key, string secret)
        {
            oauth_key = key;
            oauth_secret = secret;
        }

        public CreateListingResponse CreateListing(CreateListingRequest request)
        {
            action = "/{0}/Selling.{1}";
            method = "POST";

            var response = SendRequest<ListingResponse>(request.request);

            return new CreateListingResponse(response);
        }

        public FeesResponse Fees(FeesRequest request)
        {
            action = "/{0}/Selling/Fees.{1}";
            method = "POST";

            var response = SendRequest<FeeResponse>(request.request);

            return new FeesResponse(response);
        }
    }
}
