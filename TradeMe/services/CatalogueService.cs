using System;
using api.trademe.co.nz.v1;

namespace TradeMe.services
{
    // http://developer.trademe.co.nz/api-documentation/catalogue-methods/
    public class CatalogueService : RestBase, Catalogue
    {
        public CatalogueService(string key, string secret)
        {
            oauth_key = key;
            oauth_secret = secret;
        }

        public GetCategoriesResponse GetCategories(GetCategoriesRequest request)
        {
            throw new NotImplementedException();
        }

        public GetUsedCarMakesResponse GetUsedCarMakes(GetUsedCarMakesRequest request)
        {
            throw new NotImplementedException();
        }

        public GetMotorBikeTypesResponse GetMotorBikeTypes(GetMotorBikeTypesRequest request)
        {
            throw new NotImplementedException();
        }

        public GetJobCategoriesResponse GetJobCategories(GetJobCategoriesRequest request)
        {
            throw new NotImplementedException();
        }

        public GetCategoryLastUpdatedResponse GetCategoryLastUpdated(GetCategoryLastUpdatedRequest request)
        {
            throw new NotImplementedException();
        }

        public GetCategoryAttributesResponse GetCategoryAttributes(GetCategoryAttributesRequest request)
        {
            throw new NotImplementedException();
        }

        public GetCategoryLegalNoticeResponse GetCategoryLegalNotice(GetCategoryLegalNoticeRequest request)
        {
            throw new NotImplementedException();
        }

        public GetCategoryDurationResponse GetCategoryDuration(GetCategoryDurationRequest request)
        {
            action = "/{0}/Categories/" + request.category + "/Durations.{1}";
            method = "GET";

            var response = SendRequest<ListingDurations>(null);

            return new GetCategoryDurationResponse(response);
        }

        public GetCategoryFeesResponse GetCategoryFees(GetCategoryFeesRequest request)
        {
            throw new NotImplementedException();
        }

        public GetLocalitesResponse GetLocalites(GetLocalitesRequest request)
        {
            throw new NotImplementedException();
        }

        public GetRegionsResponse GetRegions(GetRegionsRequest request)
        {
            throw new NotImplementedException();
        }

        public GetDistrictsResponse GetDistricts(GetDistrictsRequest request)
        {
            throw new NotImplementedException();
        }

        public GetSuburbsResponse GetSuburbs(GetSuburbsRequest request)
        {
            throw new NotImplementedException();
        }

        public GetSearchAreasResponse GetSearchAreas(GetSearchAreasRequest request)
        {
            throw new NotImplementedException();
        }

        public GetTravelAreasResponse GetTravelAreas(GetTravelAreasRequest request)
        {
            throw new NotImplementedException();
        }

        public GetDvdValidationsResponse GetDvdValidations(GetDvdValidationsRequest request)
        {
            throw new NotImplementedException();
        }

        public GetBluRayValidationsResponse GetBluRayValidations(GetBluRayValidationsRequest request)
        {
            throw new NotImplementedException();
        }
    }
}
