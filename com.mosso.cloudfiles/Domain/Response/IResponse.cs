///
/// See COPYING file for licensing information
///

using System.Net;

namespace com.mosso.cloudfiles.domain.response
{
    /// <summary>
    /// The interface for all responses returned from cloudfiles
    /// </summary>
    public interface IResponse
    {
        /// <summary>
        /// A property representing the status of the request from cloudfiles
        /// </summary>
        HttpStatusCode Status { get;  }

        /// <summary>
        /// A collection of key-value pairs representing the headers returned from each request
        /// </summary>
        WebHeaderCollection Headers { get;  }
    }
}