///
/// See COPYING file for licensing information
///

using System;

namespace com.mosso.cloudfiles.exceptions
{
    /// <summary>
    /// This exception is thrown when an invalid MD5 ETag is supplied with a file being stored on cloudfiles
    /// </summary>
    public class InvalidETagException : Exception
    {

        /// <summary>
        /// The default constructor
        /// </summary>
        public InvalidETagException()
            
        {
        }
        /// <summary>
        /// A constructor for describing the correct formatting and acceptable value for the ETag header
        /// </summary>
        /// <param name="msg">Used to more explicitly indicate the reason for failure</param>
        public InvalidETagException(string msg) : base(msg)
        {
        }
    }
}