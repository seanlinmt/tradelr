///
/// See COPYING file for licensing information
///

using System;

namespace com.mosso.cloudfiles.exceptions
{
    /// <summary>
    /// Thrown when the user has incorrect authentication credentials.
    /// </summary>
    public class AuthenticationFailedException : Exception
    {
        /// <summary>
        /// The default constructor
        /// </summary>
        public AuthenticationFailedException()
        {
        }

        /// <summary>
        /// A constructor for describing the reason for failure
        /// </summary>
        /// <param name="exception">A description of why the authentication failed</param>
        public AuthenticationFailedException(String exception) : base(exception)
        {
        }
    }
}