///
/// See COPYING file for licensing information
///

using System;

namespace com.mosso.cloudfiles.exceptions
{
    /// <summary>
    /// This exception is thrown when the requested container has not been made public
    /// </summary>
    public class PublicContainerNotFoundException : Exception
    {

        /// <summary>
        /// Default constructor
        /// </summary>

        public PublicContainerNotFoundException()
        {
        }

        /// <summary>
        /// A constructor for describing the missing containing more explicitly
        /// </summary>
        /// <param name="msg">A message used to explicitly describe the failure when requesting the non-public container</param>
        public PublicContainerNotFoundException(string msg)
            : base(msg)
        {
        }
    }
}