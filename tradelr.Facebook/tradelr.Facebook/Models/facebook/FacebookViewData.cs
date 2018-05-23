 using tradelr.Common;
using tradelr.Common.Constants;
using tradelr.Library.Constants;

namespace tradelr.Facebook.Models.facebook
{
    public class FacebookViewData
    {
        public bool isTab { get; set; }
        public bool isConfigured { get; set; }
        public string pageID { get; set; }
        public string pageUrl { get; set; }
        public string storeName { get; set; }
        public string logoUrl { get; set; }
        public string pageType { get; set; }
        public string profileID { get; set; }

        public string errorMessage { get;set; }

        public string couponMessage { get; set; }

        public Gallery gallery { get; set; }

        public FacebookViewData()
        {
            logoUrl = GeneralConstants.FACEBOOK_HOST + "/Content/img/logo2.png";
        }
    }

    public static class FacebookViewHelper
    {
        public static string ToReturnUrl(this PageType pageType, string id)
        {
            string url = "";
            switch (pageType)
            {
                case PageType.APPLICATION:
                    url = string.Concat("http://www.facebook.com/apps/application.php?id=", id, "&v=app_",
                                      GeneralConstants.FACEBOOK_APP_ID);
                    break;
                case PageType.PROFILE:
                    url = string.Concat("http://www.facebook.com/profile.php?id=", id, "&v=app_",
                                      GeneralConstants.FACEBOOK_APP_ID);
                    break;
                case PageType.FANPAGE:
                default:
                    url = string.Concat("http://www.facebook.com/pages/n/", id, "?v=app_",
                                      GeneralConstants.FACEBOOK_APP_ID);
                    break;
            }
            return url;
        }
    }
}