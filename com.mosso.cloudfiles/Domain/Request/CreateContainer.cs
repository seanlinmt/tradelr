///
/// See COPYING file for licensing information
///

using System;
using com.mosso.cloudfiles.domain.request.Interfaces;
using com.mosso.cloudfiles.exceptions;
using com.mosso.cloudfiles.utils;

namespace com.mosso.cloudfiles.domain.request
{
    /// <summary>
    /// CreateContainer
    /// </summary>
    public class CreateContainer : IAddToWebRequest
    {
        private readonly string _storageUrl;
        private readonly string _containerName;

        /// <summary>
        /// CreateContainer constructor
        /// </summary>
        /// <param name="storageUrl">the customer unique url to interact with cloudfiles</param>
        /// <param name="containerName">the name of the container where the storage item is located</param>
        /// <exception cref="ArgumentNullException">Thrown when any of the reference arguments are null</exception>
        /// <exception cref="ContainerNameException">Thrown when the container name is invalid</exception>
        /// <exception cref="StorageItemNameException">Thrown when the object name is invalid</exception>
        public CreateContainer(string storageUrl, string containerName)
        {
       
            if (string.IsNullOrEmpty(storageUrl)
                || string.IsNullOrEmpty(containerName))
                throw new ArgumentNullException();

            if (!ContainerNameValidator.Validate(containerName)) throw new ContainerNameException();

                _storageUrl = storageUrl;
            _containerName = containerName;
        }

        public Uri CreateUri()
        {
            return  new Uri(_storageUrl + "/" + _containerName.Encode());
        }

        public void Apply(ICloudFilesRequest request)
        {
            request.Method = "PUT";
        }
    }
}