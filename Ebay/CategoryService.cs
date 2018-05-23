using eBay.Service.Call;
using eBay.Service.Core.Soap;

namespace Ebay
{
    public class CategoryService : EbayService
    {
        public CategoryService(string token)
            : base(token)
        {
            
        }

        public CategoryTypeCollection GetCategories(SiteCodeType siteid)
        {
            var apicall = new GetCategoriesCall(api);

            // Enable HTTP compression to reduce the download size.
            apicall.EnableCompression = true;

            apicall.DetailLevelList.Add(DetailLevelCodeType.ReturnAll);
            apicall.ViewAllNodes = true;

            apicall.Site = siteid;

            return apicall.GetCategories();
        }

        // http://developer.ebay.com/DevZone/xml/docs/Reference/ebay/GetCategoryFeatures.html#Response.Category.ReturnPolicyEnabled
        public GetCategoryFeaturesCall GetCategoryFeatures(int categoryid, SiteCodeType siteid)
        {
            //create the call object for GetCategoryFeatures
            var call = new GetCategoryFeaturesCall(api);

            call.Site = siteid;

            //set the CategoryID
            call.CategoryID = categoryid.ToString();

            //view all the nodes and get all the details
            call.ViewAllNodes = true;
            call.DetailLevelList.Add(DetailLevelCodeType.ReturnAll);

            //execute the request
            call.GetCategoryFeatures();
            return call;
        }

        public GeteBayDetailsResponseType GetEbayDetails(SiteCodeType siteid)
        {
            var call = new GeteBayDetailsCall(api);
            call.Site = siteid;
            call.GeteBayDetails(null);

            return call.ApiResponse;
        }
    }
}
