///
/// See COPYING file for licensing information
///

using System;
using com.mosso.cloudfiles.domain.request.Interfaces;
using com.mosso.cloudfiles.utils;

namespace com.mosso.cloudfiles.domain.request
{
    public class GetPublicContainerInformation : IAddToWebRequest
    {
        private readonly string _cdnManagementUrl;
        private readonly string _containerName;

        public GetPublicContainerInformation(string cdnManagementUrl,  string containerName)
        {
           
            if (String.IsNullOrEmpty(cdnManagementUrl) ||
                String.IsNullOrEmpty(containerName))
                throw new ArgumentNullException();
            _cdnManagementUrl = cdnManagementUrl;
            _containerName = containerName;
        }

        public Uri CreateUri()
        {
            return new Uri(_cdnManagementUrl + "/" + _containerName.Encode() + "?enabled_only=true");
        }

        public void Apply(ICloudFilesRequest request)
        {
            request.Method = "HEAD";
        }
    }
}