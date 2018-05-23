///
/// See COPYING file for licensing information
///

using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using com.mosso.cloudfiles.domain.request;
using com.mosso.cloudfiles.domain.request.Interfaces;
using com.mosso.cloudfiles.domain.response;
using com.mosso.cloudfiles.domain.response.Interfaces;

namespace com.mosso.cloudfiles.domain
{
    public interface IResponseFactoryWithContentBody
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        IResponseWithContentBody Create(ICloudFilesRequest request);

        GetStorageItemResponse CreateStorageItem(ICloudFilesRequest request);
    }

    /// <summary>
    /// ResponseFactoryWithContentBody
    /// </summary>
    public class ResponseFactoryWithContentBody : IResponseFactoryWithContentBody
    {
        private ICloudFilesResponse httpResponse;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public IResponseWithContentBody Create(ICloudFilesRequest request)
        {
            HttpStatusCode statusCode;
            Stream responseStream;
            WebHeaderCollection headerCollection = GetHeaderCollection(request, out statusCode, out responseStream);

            var response = new CloudFilesResponseWithContentBody()
                             {
                                 Headers = headerCollection,
                                 Status = statusCode,
                                 ContentStream = responseStream
                             };

            return response;
        }

        private WebHeaderCollection GetHeaderCollection(ICloudFilesRequest request, out HttpStatusCode statusCode, out Stream responseStream)
        {
             


            httpResponse = request.GetResponse();

            var headerCollection = httpResponse.Headers;
            statusCode = httpResponse.StatusCode;
            responseStream = httpResponse.GetResponseStream();
            return headerCollection;
        }


        public GetStorageItemResponse CreateStorageItem(ICloudFilesRequest request)
        {
            HttpStatusCode statusCode;
            Stream responseStream;
            WebHeaderCollection headerCollection = GetHeaderCollection(request, out statusCode, out responseStream);

            var response = new GetStorageItemResponse()
            {
                Headers = headerCollection,
                Status = statusCode,
                ContentStream = responseStream
            };

            return response;
        }
    }
}