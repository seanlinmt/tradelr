///
/// See COPYING file for licensing information
///

using System;
using com.mosso.cloudfiles.domain.request.Interfaces;

namespace com.mosso.cloudfiles.domain.request
{
    public class GetPublicContainers : IAddToWebRequest
    {
        private readonly string _cdnManagementUrl;

        public GetPublicContainers(string cdnManagementUrl)
        {
            _cdnManagementUrl = cdnManagementUrl;
            if (string.IsNullOrEmpty(cdnManagementUrl))
                throw new ArgumentNullException();
        }

        public Uri CreateUri()
        {
            return  new Uri(_cdnManagementUrl + "?enabled_only=true");
        }

        public void Apply(ICloudFilesRequest request)
        {
            request.Method = "GET";
            
        }
    }
}