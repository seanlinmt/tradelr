using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using clearpixels.OAuth;
using tradelr.DBML.Lucene;
using tradelr.DBML.Models;
using tradelr.Library;
using tradelr.Models.activity;
using tradelr.Models.contacts;
using tradelr.Models.counter;
using tradelr.Models.history;
using tradelr.Models.message;
using tradelr.Models.payment;
using tradelr.Models.photos;
using tradelr.Models.products;
using tradelr.Models.time;
using tradelr.Models.transactions;
using tradelr.Models.users;

namespace tradelr.DBML
{
    public interface ITradelrRepository
    {
        // misc
        void Save(string method = "");
        void CopyDataMembers(object sourceEntity, object targetEntity);

        // accounts
        user VerifyAccount(int confirm);
        bool IsDomainAvailable(string name);

        // activities
        void AddActivity(long owner, ActivityMessage message, long subdomainid);
        void DeleteActivity(long activityID, long owner);
        IQueryable<activity> GetActivities(long subdomainid);

        // cities
        MASTERcity AddCity(string city);
        IQueryable<MASTERcity> FindMASTERCity(string s);
        MASTERcity GetCity(long id);

        // comments
        void AddOrderComment(comment note);
        void AddComment(product_comment comment);
        product_comment GetComment(long parentid, long subdomainid);

        // contacts
        void AddContactGroupMember(contactGroupMember member);
        contactGroup GetContactGroup(long groupid, long subdomainid);
        IQueryable<contactGroup> GetContactGroups(long subdomainid);
        bool IsContactInUse(long contactid);
        void UpdateContactGroupMembers(long subdomainid, long groupid, string[] userids);
        void DeleteContactGroup(long ownerid, long groupid);
        user GetContact(long subdomain, long contactid);
        organisation GetProductSupplier(string supplier);
        IQueryable<user> GetContacts(long subdomain, long requester, string filterList, string sidx, string sord, ContactType? type, string letter);
        IQueryable<user> GetContacts(long subdomain, long requester, List<string> ids, string sidx, string sord);
        IQueryable<user> GetPrivateContacts(long subdomainid);
        IQueryable<user> GetPublicContacts(long requesterSubdomain);
       
        // counters
        void UpdateCounters(long subdomainid, long count, CounterType type);

        // coupons
        void AddCoupon(coupon coupon);
        void DeleteCoupon(coupon coupon);
        IQueryable<coupon> GetCoupons(long subdomainid, string sidx = "", string sord = "");

        // facebook
        void AddUpdateFacebookToken(facebook_token ftoken);
        void DeleteFacebookTokens(long subdomainid);
        void DeleteFacebookPage(facebookPage fbpage);
        void AddFacebookPage(facebookPage facebookPage);
        IQueryable<facebookPage> GetFacebookPage(string pageid);

        // favourites
        void DeleteFavourite(long owner, long productid);
        IQueryable<product> GetFavourites(long subdomainid, string categoryid, string sidx, string sord, long? owner);
        void AddFavourite(favourite fav);
        bool IsFavourite(long productid, long sessionid);
        
        // friend
        void AddFriend(long subdomainid, long friendsubdomainid);
        bool IsFriend(long subdomainid, long friendsubdomainid);

        // google
        void DeleteGoogleBlogsProductPosts(long id);
        void DeleteGoogleBaseProduct(gbase_product gbase);
        void DeleteGoogleBaseSync(MASTERsubdomain sd);

        // images
        long AddImage(image img);
        void AddProductImage(product_image img);
        void DeleteImage(string filename);
        string[] DeleteImage(long imageid, long subdomainid, PhotoType imageType);
        image GetImage(long id);
        IQueryable<image> GetImagesAll();
        product_image GetProductImage(long imageid);
        void UpdateProductImages(long subdomainid, long productid, IEnumerable<string> imageIDs);

        // indexing
        void AddActionToIndexingQueue(LuceneIndexType type, XElement data, long subdomainid, bool deleteOnly, long itemKey);

        // inventory
        void AddInventoryLocationItem(inventoryLocationItem list, long subdomainid);
        long AddInventoryLocation(inventoryLocation location, long subdomainid);
        void DeleteInventoryLocation(long id, long subdomainid);
        void DeleteInventoryLocation(string locationname, long subdomainid);
        void DeleteInventoryLocationItem(long id, long subdomainid);
        void DeleteInventoryLocationItems(long productid);
        IQueryable<inventoryLocation> GetLocations(long subdomainid);
        void UpdateProductsOutOfStock(long subdomain);

        // linkrequest
        void AddLinkRequest(linkRequest request);
        void DeleteLinkRequest(long userid, long friendid);
        void DeleteLinkRequest(linkRequest request);
        linkRequest GetLinkRequest(long id);
        linkRequest GetLinkRequest(long userid, long friendid);

        // mails
        IQueryable<mail> GetMails();
        void DeleteMail(mail entry);

        // messages
        void AddMessage(message message);
        void DeleteMessage(long sender, long recipient, MessageType msgtype);
        IQueryable<message> GetMessages(long owner, bool isInbox);
        IQueryable<message> GetLinkRequestNotifications(long owner);

        // oauth
        oauth_token GetOAuthToken(long subdomainid, OAuthTokenType type, bool? authorised = null);
        oauth_token GetOAuthToken(string oauth_token, OAuthTokenType type);
        oauth_token GetOAuthToken(long subdomainid, string appid, OAuthTokenType type);

        // oganisations
        long AddOrganisation(organisation o);
        IQueryable<organisation> GetAllOrganisationExceptOwn(long subdomainid, long ownOrgID);
        organisation GetOrganisation(long orgid);
        organisation GetOrganisation(long orgID, long subdomain);

        // orders
        void AddOrder(order order);
        void DeleteOrder(long id);
        void DeleteOrder(order o);
        IQueryable<order> GetAllPurchaseOrders(long subdomain);
        IQueryable<order> GetAllInvoices(long subdomain);
        inventoryLocation GetInventoryLocation(string locationName, long subdomainid);
        inventoryLocation GetInventoryLocation(long locationID, long subdomainid);
        inventoryLocationItem GetInventoryLocationItem(long variantid, long subdomainid);
        inventoryLocationItem GetInventoryLocationItem(long variantid, long inventoryLocationId, long subdomainid);
        IQueryable<inventoryLocationItem> GetInventoryLocationItems(long locationid, long subdomainid);
        IQueryable<inventoryLocation> GetInventoryLocationsExceptSyncNetworks(long subdomainid);
        int GetMonthlyInvoiceCount(long sessionid);
        long GetNewOrderNumber(long subdomain, TransactionType type);
        order GetOrder(long orderid);
        order GetOrder(long subdomainid, long id);
        order GetOrder(long orgid, TransactionType type, long orderNumber);
        order GetOrderByOrderNumber(long subdomain, TransactionType type, long ordernumber);
        order GetOrderByViewID(string viewid);
        IQueryable<order> GetOrders();
        IQueryable<order> GetOrders(long subdomainid, TransactionType type, long viewerid, TimeLine? interval, string sidx, string sord, bool meIsReceiver);
        void UpdateOrderStatus(long orderid,TransactionType type, long? userId, long? receiverId, OrderStatus status);
        void UpdateOrderStatus(long id, long owner, OrderStatus status);
        void UpdateOrderViewID(order order);

        // payment
        void DeletePayment(payment payment);
        payment GetPayment(long id);
        IQueryable<payment> GetPayments(PaymentMethodType method, PaymentStatus status);
        IQueryable<payment> GetPayments(long owner, TimeLine? interval, string sidx, string sord);
        payment GetPaymentByReference(string reference);

        // products
        void AddProduct(ProductInfo productInfo, long subdomainid);
        void AddProducts(IEnumerable<ProductInfo> products, long subdomainid);
        void DeleteProduct(long id, long subdomainid);
        void DeleteProductVariant(product_variant variant);
        product GetProduct(long id);
        product GetProduct(long id, long subdomain);
        product_variant GetProductVariant(string sku, long subdomainid, ProductFlag? excludeFlag);
        product_variant GetProductVariant(long variantid, long subdomainid);
        IQueryable<product_variant> GetProductVariants(long subdomain, string query);
        IQueryable<product> GetProducts(long subdomain);
        IQueryable<product> GetProducts(long subdomain, string categoryID,
                                                string sidx, string sord, string alarm, ProductFlag flag, long? collection);
        IQueryable<product> GetProducts(long subdomain, IEnumerable<string> ids, string sidx, string sord);
        IQueryable<product> GetSupplierProducts(long subdomainid);
        IQueryable<product> GetSupplierProducts(long subdomainid, long orgid);
        IQueryable<product> GetSupplierProducts(long subdomainid, string categoryID, string sidx, string sord, string supplierID);
        void UpdateProductMainThumbnail(long productid, long owner, string photoid);

        // product variants
        IQueryable<product_variant> GetProductVariants(long subdomainid);
        bool IsProductVariantInUse(long variantid, long subdomainid);

        // product categories
        long AddProductCategory(productCategory category, long subdomain);
        MASTERproductCategory AddMasterProductCategory(string name);
        void DeleteProductCategories(long subdomain, string[] strings);
        productCategory GetProductCategory(long id);
        IQueryable<productCategory> GetProductCategories(long subdomain);
        IQueryable<productCategory> GetProductCategories(long? maincategoryID, long subdomain);
        IQueryable<productCategory> GetProductCategoriesOfContactsButMine(long subdomainid);
        IQueryable<MASTERproductCategory> FindMASTERProductCategories(string query, long owner);
        void UpdateProductCategories(long subdomain, long catid, string[] allPIDs, string[] updatePIDs);

        // product collections
        long AddProductCollection(product_collection collection, long subdomainid);
        void DeleteProductCollection(long collectionid, long subdomainid);
        product_collection GetProductCollection(long collectionid, long subdomainid);
        IQueryable<product_collection> GetProductCollections(long subdomainid);
        void UpdateProductCollection(long subdomain, long collectionid, IEnumerable<long> oldSelectedPIDs,
                                     IEnumerable<long> selectedPIDs);

        // review
        review GetReview(long id);
        void AddReview(review fb);
        
        // settings
        void SetActivityPanel(long owner, UserSettings settings);
        void SetMetric(long owner, bool ismetric);
        UserSettings GetActivityPanel(long owner);
        void ToggleOfflineAccess(long subdomainid);
        
        // stockunit
        MASTERstockUnit AddMasterStockUnit(string unitName);
        void DeleteStockUnit(long id, long subdomainid);
        IQueryable<MASTERstockUnit> FindMASTERStockUnit(string query, long owner);
        long AddStockUnit(stockUnit sunit);
        IQueryable<stockUnit> GetAllStockUnits(long ownerid);

        // support
        void AddSupportMessage(adminSupportMessage message);

        // twitter search
        void AddTwitterSearch(twitterSearch search);

        // user
        void DeleteUser(long userid, long subdomainid);
        user GetPrimaryUser(long orgid);
        user GetUserByEmailAndPassword(string emailpassword, long subdomain);
        user GetUserByFBID(string fbid, long subdomainid);
        IQueryable<user> GetUserByFBID(string fbid);
        user GetUserByTwitterID(string screenName, long subdomainid);
        user GetUserByEbayID(string ebayuserid);
        user GetUserById(long id, long subdomain);
        user GetUserById(long id);
        IQueryable<user> GetUsersByEmail(string email);
        IQueryable<user> GetUsersByEmail(string email, long subdomainid);
        user GetUserByViewId(string viewid);
        long AddUser(user u);
        void UpdatePasswordHash(long userid, string hash);
        void UpdateOpenID(string openid, long ownerid);

        bool IsEmailInUse(string email, long subdomain);
        IQueryable<image> GetImages(PhotoType type, long contextID);

        // change history
        IQueryable<changeHistoryItem> GetChangeHistory(ChangeHistoryType type, long contextid, long subdomain);
        void AddChangeHistory(long changer, long contextID, ChangeHistoryType changeType, Dictionary<string, Pair<object, object>> difference);

        //shipping
        long AddShippingRule(shippingRule method);
        void AddShippingProfile(shippingProfile profile);
        void DeleteShippingRule(long id, long profileid);
        void DeleteShippingRules(long profileid, long subdomainid);
        void DeleteShippingProfile(long id, long subdomainid);
        ebay_shippingprofile GetEbayShippingProfile(long id);
        shippingProfile GetShippingProfile(long id);
        IQueryable<shippingProfile> GetShippingProfiles(long subdomainid);
        IQueryable<shippingRule> GetShippingRule(long id);
        IQueryable<shippingRule> GetShippingRules(long profiled, long subdomainid);
        bool ExistShippingRule(shippingRule rule);
        bool ExistDifferentRuleType(shippingRule rule);

        // shipwire
        void AddShipwireTransaction(shipwireTransaction t);

        // subdomains
        IQueryable<MASTERsubdomain> GetSubDomains();
        MASTERsubdomain GetSubDomain(long subdomainid);
        void AddMasterSubdomain(MASTERsubdomain subdomain);
        void DeleteSubdomain(long id);

        // store
        IQueryable<product> GetStoreProducts(long subdomainid);

        // transactions
        transaction GetTransaction(long id);

        // tumblr
        void AddTumblr(tumblrSite tumblr);
        void DeleteTumblr(tumblrSite tumblr);
        void AddTumblrPost(tumblrPost tumblr);

        // twitter
        void AddOAuthToken(oauth_token oauthToken);
        void DeleteOAuthToken(long subdomainid, OAuthTokenType type);
        IQueryable<twitterSearch> GetTwitterSearches(long subdomainid);

        // video
        void AddVideo(string youtubeid, long thumbnailid, long subdomainid);

        // wordpress
        void AddWordpress(wordpressSite wordpress);
        void DeleteWordpress(wordpressSite wordpress);

        IQueryable<contactGroupPricing> GetGroupPricings(long? groupid, long subdomainid);
        void DeleteGroupPricings(IEnumerable<contactGroupPricing> contactGroupPricings);
    }
}
