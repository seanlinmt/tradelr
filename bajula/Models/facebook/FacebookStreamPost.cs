using System;
using System.Collections.Generic;
using clearpixels.Facebook;
using clearpixels.Facebook.Resources;
using Facebook;
using tradelr.Common.Models.currency;
using tradelr.Common.Library.Imaging;
using tradelr.Common.Models.photos;
using tradelr.DBML;
using tradelr.Library;
using tradelr.Library.Constants;
using clearpixels.Logging;
using tradelr.Models.products;

namespace tradelr.Models.facebook
{
    public class FacebookStreamPost
    {
        private readonly FacebookService api;
        private readonly Post post;
        private readonly Currency currency;

        public FacebookStreamPost(string accesstoken, product p, string hostName)
        {
            currency = p.MASTERsubdomain.currency.ToCurrency();
            api = new FacebookService(accesstoken);
            post = new Post
                       {
                           message = p.title,
                           link = hostName.ToDomainUrl(p.ToProductUrl(), true),
                           name = string.Concat(p.title, " ", p.ToSellingPrice(currency)) ,
                           description = p.details.StripHtmlTags()
                       };

            if (p.thumb.HasValue)
            {
                post.picture = hostName.ToDomainUrl(p.product_image.ToModel(Imgsize.THUMB).url);
            }
        }

        public void PostToStream()
        {
            try
            {
                api.Feed.PostToHomeFeed("me", post.message, post.link, post.name, post.description, post.picture);
            }
            catch (Exception ex)
            {
                Syslog.Write(ex);
            }
            
        }

        // creates a photo album
        public void CreateProductAlbum(string profileid, IEnumerable<string> photos)
        {
            try
            {
                var albumid = api.Media.CreateAlbum(profileid, post.message, post.description);
                foreach (var photo in photos)
                {
                    api.Media.PostPhotoToAlbum(albumid.id, post.name, GeneralConstants.APP_ROOT_DIR + photo);
                }
            }
            catch (Exception ex)
            {
                Syslog.Write(ex);
            }
            
        }
    }

}