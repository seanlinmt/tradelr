///
/// See COPYING file for licensing information
///

using System;
using System.Collections.Generic;
using System.Text;
using com.mosso.cloudfiles.domain.request.Interfaces;
using com.mosso.cloudfiles.exceptions;
using com.mosso.cloudfiles.utils;

namespace com.mosso.cloudfiles.domain.request
{
    /// <summary>
    /// GetContainerItemList
    /// </summary>
    public class GetContainerItemList : IAddToWebRequest
    {
        private readonly string _storageUrl;
        private readonly string _containerName;
        private readonly StringBuilder _stringBuilder;

        /// <summary>
        /// GetContainerItemList constructor
        /// </summary>
        /// <param name="storageUrl">the customer unique url to interact with cloudfiles</param>
        /// <param name="containerName">the name of the container where the storage item is located</param>
        /// <param name="requestParameters">dictionary of parameter filters to place on the request url</param>
        /// <exception cref="ArgumentNullException">Thrown when any of the reference parameters are null</exception>
        /// <exception cref="ContainerNameException">Thrown when the container name is invalid</exception>
        public GetContainerItemList(string storageUrl,  string containerName, 
            Dictionary<GetItemListParameters, string> requestParameters)
        {
            _storageUrl = storageUrl;
            _containerName = containerName;
            if (string.IsNullOrEmpty(storageUrl)
             
                || string.IsNullOrEmpty(containerName))
                throw new ArgumentNullException();

            if (!ContainerNameValidator.Validate(containerName)) throw new ContainerNameException();

            _stringBuilder = new StringBuilder();


            if (requestParameters == null || requestParameters.Count <= 0) return;
            foreach (GetItemListParameters param in requestParameters.Keys)
            {
                var paramName = param.ToString().ToLower();
                if (param == GetItemListParameters.Limit)
                    int.Parse(requestParameters[param]);

                if (_stringBuilder.Length > 0)
                    _stringBuilder.Append("&");
                else
                    _stringBuilder.AppendFormat("?");
                _stringBuilder.Append(paramName + "=" + requestParameters[param].Encode());
            }
        }

        /// <summary>
        /// GetContainerItemList constructor
        /// </summary>
        /// <param name="storageUrl">the customer unique url to interact with cloudfiles</param>
        /// <param name="containerName">the name of the container where the storage item is located</param>
        public GetContainerItemList(string storageUrl, string containerName)
            : this(storageUrl, containerName, null)
        {
        }

        public Uri CreateUri()
        {
           return  new Uri(_storageUrl + "/" + _containerName.Encode() + _stringBuilder);
        }

        public void Apply(ICloudFilesRequest request)
        {
            request.Method = "GET";
        }
    }
}