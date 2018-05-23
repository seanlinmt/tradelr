///
/// See COPYING file for licensing information
///

using System;
using com.mosso.cloudfiles.domain.request.Interfaces;

namespace com.mosso.cloudfiles.domain.request
{
    /// <summary>
    /// GetContainers
    /// </summary>
    public class GetContainers : IAddToWebRequest
    {
        private readonly string _storageUrl;

        /// <summary>
        /// GetContainers constructor
        /// </summary>
        /// <param name="storageUrl">the customer unique url to interact with cloudfiles</param>
        /// <exception cref="ArgumentNullException">Thrown when any of the reference parameters are null</exception>
        public GetContainers(string storageUrl)
        {
            _storageUrl = storageUrl;
            if (string.IsNullOrEmpty(storageUrl))

                throw new ArgumentNullException();
        }

        public Uri CreateUri()
        {
            return new Uri(_storageUrl);
        }

        public void Apply(ICloudFilesRequest request)
        {
            request.Method = "GET";
        }
    }
}