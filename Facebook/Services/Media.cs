using System.Collections.Specialized;
using clearpixels.Facebook.Resources;

namespace clearpixels.Facebook.Services
{
    public class Media : RestBase
    {
        protected internal Media(string token) : base(token)
        {
        }

        public ResponseCollection<Album> GetAlbums(string id)
        {
            method = "GET";
            return SendRequest<ResponseCollection<Album>>(id + "/albums");
        }

        public ResponseCollection<Photo> GetPhotosInAlbum(string albumid)
        {
            method = "GET";
            return SendRequest<ResponseCollection<Photo>>(albumid + "/photos");
        }

        public Id CreateAlbum(string profileid, string name, string description)
        {
            method = "POST";
            var parameters = new NameValueCollection
                                 {
                                     {"name", name}, 
                                     {"message", description}
                                 };
            return SendRequest<Id>(profileid + "/albums", parameters);
        }

        public Id PostPhotoToAlbum(string albumid, string name, string path)
        {
            method = "POST";
            filePath = path;
            var parameters = new NameValueCollection();
            parameters.Add("message", name);
            return SendRequest<Id>(albumid + "/photos", parameters, true);
        }
    }
}
