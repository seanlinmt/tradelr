///
/// See COPYING file for licensing information
///

using System.Collections.Generic;
using System.IO;
using System.Net;
using com.mosso.cloudfiles.utils;

namespace com.mosso.cloudfiles.domain.response
{
    /// <summary>
    /// This class wraps the response from the cloud files request when expecting a content body
    /// </summary>
    public class CloudFilesResponseWithContentBody : IResponseWithContentBody
    {
        private readonly List<string> contentBody;
        private Stream contentStream;

        /// <summary>
        /// A property containing the HTTP status code from the transaction
        /// </summary>
        public HttpStatusCode Status { get; set; }

        /// <summary>
        /// A collection of key-value pairs representing the headers returned from the get container item list request
        /// </summary>
        public WebHeaderCollection Headers { get; set; }

        /// <summary>
        /// The default constructor for creating this response
        /// </summary>
        public CloudFilesResponseWithContentBody()
        {
            contentBody = new List<string>();
        }

        /// <summary>
        /// A property containing the list of lines from the content body
        /// </summary>
        public List<string> ContentBody
        {
            get { return contentBody; }
        }

        /// <summary>
        /// This method must be called once the stream has been processed to release the resources associated with the request
        /// </summary>
        public void Dispose()
        {
            if (contentStream != null)
                contentStream.Close();
        }

        /// <summary>
        /// A property representing the MIME type of the content in the body of the response
        /// </summary>
        public string ContentType
        {
            get { return Headers[Constants.CONTENT_TYPE_HEADER]; }
        }

        /// <summary>
        /// A property representing the length of the content in the body of the response
        /// </summary>
        public string ContentLength
        {
            get { return Headers[Constants.CONTENT_LENGTH_HEADER]; }
        }

        /// <summary>
        /// A property representing the stream returned from the response
        /// </summary>
        public virtual Stream ContentStream
        {
            get { return contentStream; }
            set
            {
                contentStream = value;
                ReadStream();
            }
        }

        private void ReadStream()
        {
            var streamLines = new StreamReader(contentStream).ReadToEnd().Split('\n');
            if (Status != HttpStatusCode.OK) return;
            //Because all HTTP requests end with \n\n the split at the end was appending an additional empty string container to the list
            //which of course doesn't exist
            foreach (var s in streamLines)
            {
                if (s.Length > 0)
                    contentBody.Add(s);
            }
            contentStream.Close();
        }
    }
}