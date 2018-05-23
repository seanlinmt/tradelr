///
/// See COPYING file for licensing information
///

using System;

namespace com.mosso.cloudfiles.exceptions
{
    /// <summary>
    /// This exception is thrown when the length of the meta key in the meta data header is longer than the maximum allowed length
    /// </summary>
    public class MetaKeyLengthException : Exception
    {
        /// <summary>
        /// The default constructor
        /// </summary>
        public MetaKeyLengthException()
        {
        }

        /// <summary>
        /// A constructor for indicating the explicit reason for failure
        /// </summary>
        /// <param name="exception">Used to indicate the maximum length allowed by cloudfiles</param>
        public MetaKeyLengthException(String exception) : base(exception)
        {
        }
    }
}