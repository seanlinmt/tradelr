using System;
using api.trademe.co.nz.v1;

namespace TradeMe.services
{
    // http://developer.trademe.co.nz/api-documentation/my-trade-me-methods/
    public class MyTrademeService : RestBase, MyTradeMe
    {
        public MyTrademeService(string key, string secret)
        {
            oauth_key = key;
            oauth_secret = secret;
        }


        public RequestRelistResponse RequestRelist(RequestRelistRequest request)
        {
            throw new NotImplementedException();
        }

        public GetSoldItemsResponse GetSoldItems(GetSoldItemsRequest request)
        {
            throw new NotImplementedException();
        }

        public GetUnsoldItemsResponse GetUnsoldItems(GetUnsoldItemsRequest request)
        {
            throw new NotImplementedException();
        }

        public GetListedItemsResponse GetListedItems(GetListedItemsRequest request)
        {
            throw new NotImplementedException();
        }

        public GetProductMappingResponse GetProductMapping(GetProductMappingRequest request)
        {
            throw new NotImplementedException();
        }

        public GetMemberLedgerResponse GetMemberLedger(GetMemberLedgerRequest request)
        {
            throw new NotImplementedException();
        }

        public GetPayNowLedgerResponse GetPayNowLedger(GetPayNowLedgerRequest request)
        {
            throw new NotImplementedException();
        }

        public GetWonItemsResponse GetWonItems(GetWonItemsRequest request)
        {
            throw new NotImplementedException();
        }

        public GetLostItemsResponse GetLostItems(GetLostItemsRequest request)
        {
            throw new NotImplementedException();
        }

        public GetWatchlistResponse GetWatchlist(GetWatchlistRequest request)
        {
            action = "/{0}/MyTradeMe/Watchlist/All.{1}";
            method = "GET";

            var response = SendRequest<Watchlist>(null);

            return new GetWatchlistResponse();
        }

        public GetSalesSummaryResponse GetSalesSummary(GetSalesSummaryRequest request)
        {
            throw new NotImplementedException();
        }

        public GetMemberSummaryResponse GetMemberSummary(GetMemberSummaryRequest request)
        {
            action = "/{0}/MyTradeMe/Summary.{1}";
            method = "GET";

            var response = SendRequest<MemberSummary>(null);

            return new GetMemberSummaryResponse(response);
        }

        public AddToWatchListResponse AddToWatchList(AddToWatchListRequest request)
        {
            throw new NotImplementedException();
        }

        public AddToWatchListWithEmailOptionResponse AddToWatchListWithEmailOption(AddToWatchListWithEmailOptionRequest request)
        {
            throw new NotImplementedException();
        }

        public RemoveFromWatchListResponse RemoveFromWatchList(RemoveFromWatchListRequest request)
        {
            throw new NotImplementedException();
        }

        public GetJobAgentReportResponse GetJobAgentReport(GetJobAgentReportRequest request)
        {
            throw new NotImplementedException();
        }

        public GetPropertyAgentReportResponse GetPropertyAgentReport(GetPropertyAgentReportRequest request)
        {
            throw new NotImplementedException();
        }

        public GetFixedPriceOffersResponse GetFixedPriceOffers(GetFixedPriceOffersRequest request)
        {
            throw new NotImplementedException();
        }

        public MakeFixedPriceOfferResponse MakeFixedPriceOffer(MakeFixedPriceOfferRequest request)
        {
            throw new NotImplementedException();
        }

        public GetListingFeeResponse GetListingFee(GetListingFeeRequest request)
        {
            throw new NotImplementedException();
        }

        public SaveNoteResponse SaveNote(SaveNoteRequest1 request)
        {
            throw new NotImplementedException();
        }

        public GetNoteResponse GetNote(GetNoteRequest request)
        {
            throw new NotImplementedException();
        }

        public DeleteNoteResponse DeleteNote(DeleteNoteRequest request)
        {
            throw new NotImplementedException();
        }

        public SaveStatusResponse SaveStatus(SaveStatusRequest request)
        {
            throw new NotImplementedException();
        }

        public DeleteStatusResponse DeleteStatus(DeleteStatusRequest request)
        {
            throw new NotImplementedException();
        }

        public GetDeliveryAddressesResponse GetDeliveryAddresses(GetDeliveryAddressesRequest request)
        {
            throw new NotImplementedException();
        }

        public RemoveDeliveryAddressResponse RemoveDeliveryAddress(RemoveDeliveryAddressRequest request)
        {
            throw new NotImplementedException();
        }

        public AddDeliveryAddressResponse AddDeliveryAddress(AddDeliveryAddressRequest request)
        {
            throw new NotImplementedException();
        }

        public UpdateDeliveryAddressResponse UpdateDeliveryAddress(UpdateDeliveryAddressRequest request)
        {
            throw new NotImplementedException();
        }

        public AddFeedbackResponse AddFeedback(AddFeedbackRequest request)
        {
            throw new NotImplementedException();
        }

        public UpdateFeedbackResponse UpdateFeedback(UpdateFeedbackRequest request)
        {
            throw new NotImplementedException();
        }

        public RemoveFeedbackResponse RemoveFeedback(RemoveFeedbackRequest request)
        {
            throw new NotImplementedException();
        }

        public ReplyToFeedbackResponse ReplyToFeedback(ReplyToFeedbackRequest request)
        {
            throw new NotImplementedException();
        }
    }
}
