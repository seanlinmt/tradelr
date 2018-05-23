///
/// See COPYING file for licensing information
///

using System;
using System.Collections.Generic;

namespace com.mosso.cloudfiles.domain
{
    public interface IObject
    {
        string Name { get; }
        Uri PublicUrl { get; set; }
        Dictionary<string, string> Metadata { get; set; }
        long ContentLength { get; }
        string ETag { get; }
        string ContentType { get; }
        string ContainerName { get; set; }
    }

    public class CF_Object : IObject
    {
        private readonly IConnection connection;
        protected Uri publicUrl;
        protected long contentLength;
        protected string etag;
        protected string contentType;
        protected Dictionary<string, string> metadata;

        public CF_Object(IConnection connection, string objectName) : this(connection, objectName, new Dictionary<string, string>()){}

        public CF_Object(IConnection connection, string objectName, Dictionary<string, string> metadata)
        {
            this.metadata = metadata;
            this.connection = connection;
            Name = objectName;
        }

        public string ContainerName { get; set; }

        public string Name { get; private set; }

        public long ContentLength
        {
            get
            {
                CloudFilesHeadObject();
                return contentLength;
            }
        }

        public string ETag
        {
            get
            {
                CloudFilesHeadObject(); 
                return etag;
            }
        }

        public string ContentType
        {
            get
            {
                CloudFilesHeadObject(); 
                return contentType;
            }
        }

        public Dictionary<string, string> Metadata
        {
            get { return metadata; }
            set
            {
                metadata = value;
                CloudFilesPostObject();
            }
        }

        public Uri PublicUrl
        {
            get { return new Uri(publicUrl + Name); }
            set { publicUrl = value; }
        }

        protected virtual void CloudFilesHeadObject()
        {
            var @objectInformation = connection.GetStorageItemInformation(ContainerName, Name);
            contentLength =String.IsNullOrEmpty(@objectInformation.ContentLength) ? 0 : long.Parse(@objectInformation.ContentLength);
            contentType = @objectInformation.ContentType;
            etag = @objectInformation.ETag;
            metadata = @objectInformation.Metadata;

        }

        protected virtual void CloudFilesPostObject()
        {
            connection.PutStorageItem(ContainerName, Name, Metadata);
        }
    }
}