///
/// See COPYING file for licensing information
///

using System;
using System.Collections.Generic;
using System.IO;

namespace com.mosso.cloudfiles.domain.response
{
    /// <summary>
    /// An interface representing responses with a content body
    /// </summary>
    public interface IResponseWithContentBody : IResponse, IDisposable
    {
        /// <summary>
        /// A property representing the stream returned from cloudfiles
        /// </summary>
        Stream ContentStream { get; set; }

        /// <summary>
        /// A property representing the parsed content body broken down line by line
        /// </summary>
        List<String> ContentBody { get; }

        /// <summary>
        /// A property representing the length, in bytes, of the content body
        /// </summary>
        string ContentLength { get; }

        /// <summary>
        /// A property representing the MIME type of the response body
        /// </summary>
        string ContentType { get; }
    }
}