using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Etsy.Resources;

namespace Etsy.Services
{
    public class Shop : RestBase
    {
        public Shop(SessionInfo session)
        {
            info = session;
        }

        public ShopSection createShopSection(string title, string user_id)
        {
            method = "POST";
            URI = "sections";
            var parameters = new Parameters();
            parameters.AddParameter("title", title);
            parameters.AddParameter("user_id", user_id);

            var response = SendRequest<ShopSection>(parameters);
            return response == null ? null: response.results[0];
        }

        public IEnumerable<Resources.Listing> findAllShopListingsActive(long shopid, int offset = 0, string includes = "", int limit = Constants.LIMIT_ENTRIES)
        {
            method = "GET";
            URI = "/shops/:shop_id/listings/active";
            id.shop_id = shopid.ToString();
            var parameters = new Parameters();
            parameters.AddParameter("includes", includes);
            parameters.AddParameter("offset", offset.ToString());
            parameters.AddParameter("limit", limit.ToString());

            var response = SendRequest<Resources.Listing>(parameters);
            return response == null ? null : response.results;
        }

        public IEnumerable<Resources.Listing> findAllShopListingsExpired(long shopid, int offset = 0, string includes = "", int limit = Constants.LIMIT_ENTRIES)
        {
            method = "GET";
            URI = "/shops/:shop_id/listings/expired";
            id.shop_id = shopid.ToString();
            var parameters = new Parameters();
            parameters.AddParameter("includes", includes);
            parameters.AddParameter("offset", offset.ToString());
            parameters.AddParameter("limit", limit.ToString());

            var response = SendRequest<Resources.Listing>(parameters);
            return response == null ? null : response.results;
        }

        public Shop getShop()
        {
            method = "GET";
            URI = "/shops/:shop_id";
            var parameters = new Parameters();
            parameters.AddParameter("includes", "Listings,Sections");

            var response = SendRequest<Shop>(parameters);
            return response == null ? null : response.results[0];
        }
    }
}
