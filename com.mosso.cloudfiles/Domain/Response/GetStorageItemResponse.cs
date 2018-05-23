
/// See COPYING file for licensing information
///

using System.Collections.Generic;
using System.IO;
using System.Net;
using com.mosso.cloudfiles.utils;

namespace com.mosso.cloudfiles.domain.response
{
    /// <summary>
    /// This class wraps the response from the get storage item request
    /// </summary>
    public class GetStorageItemResponse : IResponseWithContentBody
    {
        private readonly List<string> contentBody;
        public event Connection.ProgressCallback Progress;
        private Stream contentStream;

        /// <summary>
        /// The default constructor for creating instances of GetStorageItemResponse
        /// </summary>
        public GetStorageItemResponse()
        {
            contentBody = new List<string>();
        }

        /// <summary>
        /// A property containing the HTTP status code from the transaction
        /// </summary>
        public HttpStatusCode Status { get; set; }

        /// <summary>
        /// A property representing the binary information making up the storage item
        /// </summary>
        public List<string> ContentBody
        {
            get { return contentBody; }
        }

        /// <summary>
        /// A collection of key-value pairs representing the headers returned from the get storage item request
        /// </summary>
        public WebHeaderCollection Headers { get; set; }

        /// <summary>
        /// A property representing the size, in bytes, of the stream representing the storage item
        /// </summary>
        public string ContentLength
        {
            get { return Headers[Constants.CONTENT_LENGTH_HEADER]; }
        }

        /// <summary>
        /// A property representing the MIME type of the storage item
        /// </summary>
        public string ContentType
        {
            get { return Headers[Constants.CONTENT_TYPE_HEADER]; }
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
        /// A property representing the stream returned from the response
        /// </summary>
        public Stream ContentStream
        {
            get { return contentStream; }
            set { contentStream = value; }
        }

        /// <summary>
        /// This method saves the stream from the response directly to a named file on disk
        /// </summary>
        /// <param name="filename">The file name to save the stream to locally</param>
        public void SaveStreamToDisk(string filename)
        {
            StoreFile(filename);
        }

        private void StoreFile(string filename)
        {
            FileStream fs = new FileStream(filename, FileMode.Create);

            byte[] buffer = new byte[4096];

            int amt = 0;
            while ((amt = contentStream.Read(buffer, 0, buffer.Length)) != 0)
            {
                fs.Write(buffer, 0, amt);
                if (Progress != null)
                    Progress(amt);
            }
            fs.Close();
            contentStream.Close();
        }
    }
}