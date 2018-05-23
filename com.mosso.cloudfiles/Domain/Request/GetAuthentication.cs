///
/// See COPYING file for licensing information
///

using System;
using System.Net;
using com.mosso.cloudfiles.domain.request.Interfaces;
using com.mosso.cloudfiles.utils;

namespace com.mosso.cloudfiles.domain.request
{
    /// <summary>
    /// GetAuthentication
    /// </summary>
    public class GetAuthentication : IAddToWebRequest
    {
        private readonly UserCredentials _userCredentials;


        /// <summary>
        /// GetAuthentication constructor
        /// </summary>
        /// <param name="userCreds">the UserCredentials instace to use when attempting authentication</param>
        /// <exception cref="System.ArgumentNullException">Thrown when any of the reference arguments are null</exception>
        public GetAuthentication(UserCredentials userCreds)
        {
            if(userCreds == null)
            throw new ArgumentNullException();
            _userCredentials = userCreds;
        }

        public Uri CreateUri()
        {
             if (_userCredentials == null) throw new ArgumentNullException();
            var uri = string.IsNullOrEmpty(_userCredentials.AccountName)
                ? _userCredentials.AuthUrl
                : new Uri(_userCredentials.AuthUrl + "/"
                    + _userCredentials.Cloudversion.Encode() + "/"
                    + _userCredentials.AccountName.Encode() + "/auth");
            return uri;
        }

        public void Apply(ICloudFilesRequest request)
        {
            request.Method = "GET";
            request.Headers.Add(Constants.X_AUTH_USER, _userCredentials.Username.Encode());
            request.Headers.Add(Constants.X_AUTH_KEY, _userCredentials.Api_access_key.Encode());
        }
    }
}