///
/// See COPYING file for licensing information
///

using System;
using System.Collections.Generic;
using System.IO;

namespace com.mosso.cloudfiles.domain
{
    /// <summary>
    /// StorageItem
    /// </summary>
    public class StorageItem : IDisposable
    {
        public event Connection.ProgressCallback Progress;
        private readonly string objectName;
        private readonly Dictionary<string, string> metadata;
        private readonly string objectContentType;
        private readonly Stream objectStream;
        private readonly long contentLength;
        private readonly DateTime lastModified;

      

        /// <summary>
        /// 
        /// </summary>
        /// <param name="objectName"></param>
        /// <param name="metadata"></param>
        /// <param name="objectContentType"></param>
        /// <param name="contentLength"></param>
        /// <param name="lastModified"></param>
        public StorageItem(string objectName, Dictionary<string, string> metadata, string objectContentType, long contentLength, DateTime lastModified)
        {
            this.objectName = objectName;
            this.lastModified = lastModified;
            this.contentLength = contentLength;
            this.objectContentType = objectContentType;
            this.metadata = metadata;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="objectName"></param>
        /// <param name="metadata"></param>
        /// <param name="objectContentType"></param>
        /// <param name="contentStream"></param>
        /// <param name="contentLength"></param>
        /// <param name="lastModified"></param>
        public StorageItem(string objectName, Dictionary<string, string> metadata, string objectContentType, Stream contentStream, long contentLength, DateTime lastModified)
        {
            this.objectName = objectName;
            this.lastModified = lastModified;
            this.contentLength = contentLength;
            this.objectContentType = objectContentType;
            this.metadata = metadata;
            objectStream = contentStream;
        }

        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
            if (objectStream != null)
                objectStream.Close();
        }

        /// <summary>
        /// 
        /// </summary>
        public long FileLength
        {
            get { return contentLength; }
        }

        /// <summary>
        /// 
        /// </summary>
        public string ContentType
        {
            get { return objectContentType; }
        }

        /// <summary>
        /// 
        /// </summary>
        public Dictionary<string, string> Metadata
        {
            get { return metadata; }
        }

        /// <summary>
        /// 
        /// </summary>
        public Stream ObjectStream
        {
            get { return objectStream; }
        }

        /// <summary>
        /// 
        /// </summary>
        public string ObjectName
        {
            get { return objectName; }
        }

        public DateTime LastModified
        {
            get { return lastModified; }
        }

    }
}