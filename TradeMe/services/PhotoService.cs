using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using api.trademe.co.nz.v1;

namespace TradeMe.services
{
    public class PhotoService : RestBase, Photos
    {
        public PhotoService(string key, string secret)
        {
            oauth_key = key;
            oauth_secret = secret;
        }

        public AddPhotoResponse AddPhoto(AddPhotoRequest request)
        {
            action = "/{0}/Photos.{1}";
            method = "POST";

            var response = SendRequest<PhotoResponse>(request.photo);

            return new AddPhotoResponse(response);
        }

        public RemovePhotoResponse RemovePhoto(RemovePhotoRequest request)
        {
            throw new NotImplementedException();
        }

        public GetPhotosResponse GetPhotos(GetPhotosRequest request)
        {
            throw new NotImplementedException();
        }

        public GetPhotosWithDetailsResponse GetPhotosWithDetails(GetPhotosWithDetailsRequest request)
        {
            throw new NotImplementedException();
        }

        public AddPhotoToListingResponse AddPhotoToListing(AddPhotoToListingRequest request)
        {
            throw new NotImplementedException();
        }
    }
}
