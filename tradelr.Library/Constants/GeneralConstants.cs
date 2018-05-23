using System;

namespace tradelr.Library.Constants
{
    public static class GeneralConstants
    {
        public const string FREE = "FREE";
#if DEBUG
        public const bool DEBUG = true;
#else
        public const bool DEBUG = false;
#endif
        public const int ACTIVITY_ROWS_PER_PAGE = 30;
        

        // azure
#if AZURE
        public const string AZURE_CONTAINER_IMAGES = "photos";
        public const string AZURE_CONTAINER_FILES = "files";
        public const string AZURE_CONTAINER_LUCENE = "search";
#endif
#if RACKSPACE
#if DEBUG
        public const string RACKSPACE_CONTAINER_IMAGES = "photos_debug";
#else
        public const string RACKSPACE_CONTAINER_IMAGES = "photos";
#endif
#endif

        // bitly
        public const string BITLY_API_KEY = "BITLY_API_KEY";

        // content types
        public const string CONTENT_TYPE_JSON = "application/json";

        // cookie
        public const double COOKIE_LIFETIME = 14; // days


        public const string INVENTORY_LOCATION_DEFAULT = "Main";
        public const string DATEFORMAT_FULL = "ddd, dd MMM yyyy hh:mm tt";
        public const string DATEFORMAT_TIME = "dd MMM hh:mm tt";
        public const string DATEFORMAT_GRID = "dd MMM yy";
        public const string DATEFORMAT_NOYEAR = "dd MMM";
        public const string DATEFORMAT_INVOICE = "dd MMM yyyy";
        public const string DATEFORMAT_JAVASCRIPT = "d MMM yyyy";
        public const string DATEFORMAT_STANDARD = "ddd, d MMM yyyy";

        public const int DURATION_1DAY_SECS = 86400;
        public const int DURATION_1MONTH_SECS = 2592000;

        // facebook
        public const string FACEBOOK_GRAPH_HOST = "https://graph.facebook.com/";
        public const string FACEBOOK_EMAIL_DOMAIN = "proxymail.facebook.com";
#if DEBUG
        public const string FACEBOOK_API_KEY = "FACEBOOK_API_KEY";
        public const string FACEBOOK_API_SECRET = "FACEBOOK_API_SECRET";
        public const string FACEBOOK_APP_URL = "http://apps.facebook.com/tradelrtest";
        public const string FACEBOOK_APP_ID = "FACEBOOK_APP_ID";
        public const string FACEBOOK_HOST = "http://fbtest.tradelr.com";
#else
        public const string FACEBOOK_API_KEY = "FACEBOOK_API_KEY";
        public const string FACEBOOK_API_SECRET = "FACEBOOK_API_SECRET";
        public const string FACEBOOK_APP_URL = "http://apps.facebook.com/tradelr";
        public const string FACEBOOK_APP_ID = "FACEBOOK_APP_ID";
        public const string FACEBOOK_HOST = "http://fb.tradelr.com";
#endif

        // files
        public const string FILE_UPLOAD_PATH = "Uploads/files/";

        

#if DEBUG
        public const bool BUILD_DEBUG = true;
        public const string HTTP_HOST = "http://localhost";
        public const string HTTP_SECURE = "https://secure.localhost";
#if SUPPORT_HTTPS
        public const string HTTP_SSLHOST = "https://localhost";
        
#else
        public const string HTTP_SSLHOST = "http://localhost";
#endif
        public const string HTTP_CACHEURL = "http://localhost/dummy";
#else
        public const bool BUILD_DEBUG = false;
        public const string HTTP_HOST = "http://www.tradelr.com";
        public const string HTTP_SECURE = "https://secure.tradelr.com";
#if SUPPORT_HTTPS
        public const string HTTP_SSLHOST = "https://www.tradelr.com";
        
#else
        public const string HTTP_SSLHOST = "http://www.tradelr.com";
#endif
        public const string HTTP_CACHEURL = "http://98.126.29.28/dummy";
#endif // debug
        public const string LUCENE_INDEX_LOCATION = "LuceneIndex";

        
        // opensocial
        public const string OS_MYSPACE_APP_URL = "http://www.myspace.com/537671473";
        public const string OS_FRIENDSTER_APP_URL = "http://widgets.friendster.com/tradelr";

        // orders, invoices
        public const int ORDER_DESCRIPTION_STRING_LENGTH = 80;

        // photos
        public const string PHOTO_NO_THUMBNAIL = "/Content/img/thumbs_none.png";
        public const string PHOTO_NO_THUMBNAIL_MEDIUM = "/Content/img/thumbs_none_medium.png";
        public const string PHOTO_NO_PROFILE = "/Content/img/profile_nophoto.png";
        public const string PHOTO_NO_PROFILE64 = "/Content/img/profile_nophoto_64.png";
        
        public const string PHOTO_UPLOAD_ERROR_PATH = "/Content/img/swf/error.gif";
#if DEBUG && !FBCANVAS
        public const string APP_ROOT_DIR = @"C:\code\tradelr\bajula\bajula\";
#else
        public const string APP_ROOT_DIR = @"C:\inetpub\wwwroot\tradelr\";
#endif

        // rackspace
        public const string RACKSPACE_CLOUD_USERNAME = "RACKSPACE_CLOUD_USERNAME";
        public const string RACKSPACE_CLOUD_APIKEY = "RACKSPACE_CLOUD_APIKEY";


#if DEBUG
        public const string SERVER_IP = "127.0.0.1";
#else
        public const string SERVER_IP = "98.126.29.28";
#endif
        // twitter
        public const int TWITTER_MAX_LENGTH = 140;

        // url links for various stuff
        //view contact
        public const string URL_SINGLE_CONTACT = "/dashboard/contacts/";
        //view product
        public const string URL_SINGLE_PRODUCT_SHOW = "/products/";
        // view purcharse order
        public const string URL_SINGLE_ORDER = "/dashboard/orders/";
        // view invoice
        public const string URL_SINGLE_INVOICE = "/dashboard/invoices/";

        // gbase
        public const string GBASE_MASTERACCOUNTID = "12020081";

        // subdomain names
#if DEBUG
        public const string SUBDOMAIN_HOST_NOPORT = "localhost";
#if AZURE
        public const string SUBDOMAIN_HOST = "localhost";
#else
        public const string SUBDOMAIN_HOST = "localhost";
#endif // azure
#else
        public const string SUBDOMAIN_HOST_NOPORT = "tradelr.com";
        public const string SUBDOMAIN_HOST = "tradelr.com";
#endif // debug
        public static readonly string TIMESTAMP = DateTime.UtcNow.Ticks.ToString();

        public static readonly string[] SUBDOMAIN_RESTRICTED = new[]
                                                          {
                                                              "a", "admin","administrator", "amazon", "api",
                                                              "b","blog","blogs","buzz",
                                                              "c","checkout","checkouts","css",
                                                              "d","developer",
                                                              "e", "etsy", "ebay",
                                                              "f","facebook","fb", "fbtest","forum","forums",
                                                              "g", "gadget","gadgets",
                                                              "h","help",
                                                              "i","image","images",
                                                              "j","js",
                                                              "k",
                                                              "l","login",
                                                              "m","mail", "mailer", "mails", "multiply",
                                                              "n",
                                                              "o","opensocial","os",
                                                              "p","photo","photos", "pinterest", "platform",
                                                              "q",
                                                              "r","register","ruf",
                                                              "s","search","secure","static","store","stores", "shops", "shop",
                                                              "t","twitter",
                                                              "u",
                                                              "v",
                                                              "w","wiki","www",
                                                              "x","xxx",
                                                              "y",
                                                              "z"
                                                          };

        public static readonly string[] KEYWORD_RESTRICTED = new[]
                                                                 {
                                                                     "alcohol","ammo","ammunition",
                                                                     "beer","bet","blade","bonds","bullet",
                                                                     "casino","cigar",
                                                                     "drug",
                                                                     "forex",
                                                                     "gun",
                                                                     "knife",
                                                                     "pistol",
                                                                     "rifle","rich",
                                                                     "sex",
                                                                     "tobacco"
                                                                };
        
    }
}