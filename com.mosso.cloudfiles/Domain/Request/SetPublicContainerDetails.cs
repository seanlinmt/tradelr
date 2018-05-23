///
/// See COPYING file for licensing information
///

using System;
using com.mosso.cloudfiles.domain.request.Interfaces;
using com.mosso.cloudfiles.utils;

namespace com.mosso.cloudfiles.domain.request
{
    public class SetPublicContainerDetails : IAddToWebRequest
    {
        private readonly string _cdnManagementUrl;
        private readonly string _containerName;
        private readonly bool _isCdnEnabled;
        private readonly bool _isLoggingEnabled;
        private readonly int _timeToLiveInSeconds;
        private readonly string _agentacl;
        private readonly string _refacl;

        /// <summary>
        /// Assigns various details to containers already publicly available on the CDN
        /// </summary>
        /// <param name="cdnManagementUrl">The CDN URL</param>
        /// <param name="containerName">The name of the container to update the details for</param>
        /// <param name="isCdnEnabled">Sets whether or not specified container is available on the CDN</param>
        /// <param name="timeToLiveInSeconds"></param>
        public SetPublicContainerDetails(string cdnManagementUrl, string containerName,  bool isCdnEnabled,bool isLoggingEnabled ,int timeToLiveInSeconds, string agentacl, string refacl)
        {
  
            if (String.IsNullOrEmpty(cdnManagementUrl) ||
                String.IsNullOrEmpty(containerName))
                throw new ArgumentNullException();        
            _cdnManagementUrl = cdnManagementUrl;
            _containerName = containerName;
            _isCdnEnabled = isCdnEnabled;
            _isLoggingEnabled = isLoggingEnabled;
            _timeToLiveInSeconds = timeToLiveInSeconds;
            _agentacl = agentacl;
            _refacl = refacl;
        }

        public Uri CreateUri()
        {
            return  new Uri(_cdnManagementUrl + "/" + _containerName.Encode());
        }

        public void Apply(ICloudFilesRequest request)
        {
            request.Method = "POST";
            request.Headers.Add(Constants.X_CDN_ENABLED, _isCdnEnabled.Capitalize());
            request.Headers.Add(Constants.X_LOG_RETENTION, _isLoggingEnabled.Capitalize());
            if(_timeToLiveInSeconds > -1) request.Headers.Add(Constants.X_CDN_TTL, _timeToLiveInSeconds.ToString());
            if(!String.IsNullOrEmpty(_agentacl)) request.Headers.Add(Constants.X_USER_AGENT_ACL, _agentacl);
            if (!String.IsNullOrEmpty(_refacl)) request.Headers.Add(Constants.X_REFERRER_ACL, _refacl);
        
        }
    }
}