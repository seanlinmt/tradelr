///
/// See COPYING file for licensing information
///

using System;

namespace com.mosso.cloudfiles.exceptions
{
    /// <summary>
    /// This exception is thrown when the length of the meta value in the meta data header is longer than the maximum allowed length
    /// </summary>
    public class MetaValueLengthException : Exception
    {
        /// <summary>
        /// The primary constructor for creating 
        /// </summary>
        public MetaValueLengthException()
        {
        }

        /// <summary>
        /// A constructor for indicating the explicit reason for failure
        /// </summary>
        /// <param name="exception">Used to explicitly indicate the maximum length allowed by cloudfiles</param>
        public MetaValueLengthException(String exception) : base(exception)
        {
        }
    }
}