///
/// See COPYING file for licensing information
///

using System;
using System.Net;
using System.Text;
using com.mosso.cloudfiles.domain.request;
using com.mosso.cloudfiles.domain.request.Interfaces;
using com.mosso.cloudfiles.domain.response;
using com.mosso.cloudfiles.domain.response.Interfaces;
using com.mosso.cloudfiles.exceptions;
using com.mosso.cloudfiles.utils;

namespace com.mosso.cloudfiles.domain
{
    /// <summary>
    /// 
    /// </summary>
    public interface IResponseFactory
    {
        ICloudFilesResponse Create(ICloudFilesRequest request);
    }

    /// <summary>
    /// ResponseFactory
    /// </summary>
    public class ResponseFactory : IResponseFactory 
    {
        public ResponseFactory()
        {
            Log.EnsureInitialized();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public ICloudFilesResponse Create(ICloudFilesRequest request)
        {
            // this may be very very wrong look at tests closely
         //   if (request.ContentLength>0)
          //      throw new InvalidResponseTypeException(
             //       "The request type is of IRequestWithContentBody. Content body is expected with this request. ");

          
            Log.Debug(this, OutputRequestInformation(request));


            var response = request.GetResponse();
            Log.Debug(this, OutputResponseInformation(response));

            
            response.Close();
            return response;
                      
        }

        private string OutputRequestInformation(ICloudFilesRequest request)
        {
            StringBuilder output = new StringBuilder();
            output.Append("\n");
            output.Append("REQUEST");
            output.Append("\n");
            output.Append("Request URL: ");
            output.Append(request.RequestUri);
            output.Append("\n");
            output.Append("method: ");
            output.Append(request.Method);
            output.Append("\n");
            output.Append("Headers: ");
            output.Append("\n");
            foreach (var key in request.Headers.AllKeys)
            {
                output.Append(key);
                output.Append(": ");
                output.Append(request.Headers[key]);
                output.Append("\n");
            }

            return output.ToString();
        }

        private string OutputResponseInformation(ICloudFilesResponse response)
        {
            StringBuilder output = new StringBuilder();
            output.Append("\n");
            output.Append("RESPONSE:");
            output.Append("\n");
            output.Append("method: ");
            output.Append(response.Method);
            output.Append("\n");
            output.Append("Status Code: ");
            output.Append(response.StatusCode.ToString());
            output.Append("\n");
            output.Append("Status Description: ");
            output.Append(response.StatusDescription);
            output.Append("\n");
            output.Append("Headers: ");
            output.Append("\n");
            foreach (var key in response.Headers.AllKeys)
            {
                output.Append(key);
                output.Append(": ");
                output.Append(response.Headers[key]);
                output.Append("\n");
            }

            return output.ToString();
        }
    }
}