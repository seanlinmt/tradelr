///
/// See COPYING file for licensing information
///

using System;
using System.ComponentModel;
using com.mosso.cloudfiles.domain.request.Interfaces;
using com.mosso.cloudfiles.utils;

namespace com.mosso.cloudfiles.domain.request
{
    /// <summary>
    /// GetAccountInformation
    /// </summary>
    public class GetAccountInformation : IAddToWebRequest
    {
        private readonly string _storageUrl;
      

        /// <summary>
        /// GetAccountInformation constructor
        /// </summary>
        /// <exception cref="ArgumentNullException">Thrown when any of the parameters are null</exception>
        public GetAccountInformation(string storageUrl)
        {
            _storageUrl = storageUrl;
         
            if (string.IsNullOrEmpty(storageUrl))
                throw new ArgumentNullException();
        }

        public Uri CreateUri()
        {
            return new Uri(_storageUrl + "/");
   
        }

        public void Apply(ICloudFilesRequest request)
        {
            request.Method = "HEAD";
        }
    }

    public enum Format
    {
        [Description("json")]
        JSON,
        [Description("xml")]
        XML
    }

    public class GetAccountInformationSerialized : IAddToWebRequest
    {
        private readonly string _storageUrl;
        private readonly Format _format;

        /// <summary>
        /// GetAccountInformationSerialized constructor
        /// </summary>
        /// <exception cref="ArgumentNullException">Thrown when any of the parameters are null</exception>
        public GetAccountInformationSerialized(string storageUrl, Format format)
        {
            
            if (string.IsNullOrEmpty(storageUrl))
                throw new ArgumentNullException();
            _storageUrl = storageUrl;
            _format = format;
        }

        public Uri CreateUri()
        {
           return  new Uri(_storageUrl + "?format=" + EnumHelper.GetDescription(_format));
        }

        public void Apply(ICloudFilesRequest request)
        {
            request.  Method = "GET";
        }
    }
}