///
/// See COPYING file for licensing information
///

using System.Collections.Generic;
using System.Net;
using com.mosso.cloudfiles.utils;

namespace com.mosso.cloudfiles.domain
{
    public class StorageItemInformation
    {
        private readonly WebHeaderCollection headers;

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="headers">collection of headers assigned to this storage item</param>
        public StorageItemInformation(WebHeaderCollection headers)
        {
            this.headers = headers;
        }

        /// <summary>
        /// entity tag used to determine if any content changed in transfer - http://en.wikipedia.org/wiki/HTTP_ETag
        /// </summary>
        public string ETag
        {
            get { return headers[Constants.ETAG]; }
        }

        /// <summary>
        /// http content type of the storage item
        /// </summary>
        public string ContentType
        {
            get { return headers[Constants.CONTENT_TYPE_HEADER]; }
        }

        /// <summary>
        /// http content length of the storage item
        /// </summary>
        public string ContentLength
        {
            get { return headers[Constants.CONTENT_LENGTH_HEADER]; }
        }

        /// <summary>
        /// dictionary of meta tags assigned to this storage item
        /// </summary>
        public Dictionary<string, string> Metadata
        {
            get
            {
                Dictionary<string, string> tags = new Dictionary<string, string>();
                foreach (string s in headers.Keys)
                {
                    if (s.IndexOf(Constants.META_DATA_HEADER) != -1)
                    {
                        int metaKeyStart = s.LastIndexOf("-");
                        tags.Add(s.Substring(metaKeyStart + 1), headers[s]);
                    }
                }
                return tags;
            }
        }
    }
}