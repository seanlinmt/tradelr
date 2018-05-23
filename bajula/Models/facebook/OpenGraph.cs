using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using tradelr.Common.Library.Imaging;
using tradelr.DBML;
using tradelr.Library;
using tradelr.Library.Constants;
using tradelr.Library.geo;
using tradelr.Models.liquid.models.Blog;
using tradelr.Models.liquid.models.Product;
using tradelr.Models.subdomain;

namespace tradelr.Models.facebook
{
    public class OpenGraph
    {
        public string fbid { get; set; }
        public string sitename { get; set; }
        public string description { get; set; }

        public string title { get; set; }
        public string type { get; set; }
        public string url { get; set; }
        public string image { get; set; }
        public string latitude { get; set; }
        public string longtitude { get; set; }
        public string address { get; set; }
        public string locality { get; set; }
        public string region { get; set; }
        public string postcode { get; set; }
        public string countryname { get; set; }
        public string email { get; set; }
        public string phone { get; set; }
        public string fax { get; set; }
    }

    public static class OpenGraphHelper
    {
        public static OpenGraph ToOpenGraph(this organisation org, product p, article a)
        {
            var graph = new OpenGraph
            {
                fbid = org.users.First().FBID,
                sitename = org.MASTERsubdomain.storeName,
                description = org.motd,
                address = org.address,
                countryname = org.country.HasValue ? Country.GetCountry(org.country.Value).name : "",
                email = "",
                fax = org.fax,
                latitude = org.latitude.HasValue ? org.latitude.ToString() : "",
                longtitude = org.longtitude.HasValue ? org.longtitude.ToString() : "",
                locality = org.city.HasValue ? org.MASTERcity.name : "",
                region = "",
                postcode = org.postcode,
                phone = org.phone,
            };

            if (p != null)
            {
                graph.type = "product";
                graph.title = p.title;
                graph.url = org.MASTERsubdomain.ToHostName().ToDomainUrl(p.ToLiquidProductUrl());
                graph.image = p.thumb.HasValue
                                  ? org.MASTERsubdomain.ToHostName().ToDomainUrl(Img.by_size(p.product_image.url,
                                                                                        Imgsize.SMALL))
                                  : org.MASTERsubdomain.ToHostName().ToDomainUrl(GeneralConstants.PHOTO_NO_THUMBNAIL);
            }
            else if (a != null)
            {
                graph.type = "article";
                graph.title = a.title;
                graph.url = org.MASTERsubdomain.ToHostName().ToDomainUrl(a.ToLiquidUrl());
            }
            else
            {
                graph.type = "company";
                graph.title = org.MASTERsubdomain.storeName;
                graph.url = org.MASTERsubdomain.ToHostName().ToDomainUrl();
            }

            return graph;
        }
    }
}