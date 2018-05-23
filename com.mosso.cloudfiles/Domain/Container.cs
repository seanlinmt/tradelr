///
/// See COPYING file for licensing information
/// 

namespace com.mosso.cloudfiles.domain
{
    /// <summary>
    /// Container
    /// </summary>
    public class Container
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="containerName">Name of the container</param>
        public Container(string containerName)
        {
            Name = containerName;
            ObjectCount = 0;
            ByteCount = 0;
            TTL = -1;
        }

        /// <summary>
        /// Size of the container
        /// </summary>
        public long ByteCount { get; set; }

        /// <summary>
        /// Number of items in the container
        /// </summary>
        public long ObjectCount { get; set; }

        /// <summary>
        /// Name of the container
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// The maximum time (in seconds) content should be kept alive on the CDN before it checks for freshness.
        /// </summary>
        public int TTL { get; set; }

        /// <summary>
        /// The URI one can use to access objects in this container via the CDN. No time based URL stuff will be included with this URI
        /// </summary>
        public string CdnUri { get;  set; }

        /// <summary>
        /// Referrer ACL 
        /// </summary>
        public string ReferrerACL { get; set; }

        /// <summary>
        /// User Agent ACL
        /// </summary>
        public string UserAgentACL { get; set; }
          
    }
}