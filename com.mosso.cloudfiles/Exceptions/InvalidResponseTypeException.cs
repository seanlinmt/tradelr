///
/// See COPYING file for licensing information
///

using System;

namespace com.mosso.cloudfiles.exceptions
{
    /// <summary>
    /// This exception is thrown when the wrong IResponse type is passed into an instance of ResponseFactory
    /// </summary>
    public class InvalidResponseTypeException : Exception
    {
        /// <summary>
        /// The default constructor
        /// </summary>
        public InvalidResponseTypeException()
        {
        }

        /// <summary>
        /// Used to construct an InvalidResponseTypeException containing the explicit reason for failure
        /// </summary>
        /// <param name="msg">Used to more explicitly indicate the reason for failure</param>
        public InvalidResponseTypeException(string msg) : base(msg)
        {
        }
    }
}