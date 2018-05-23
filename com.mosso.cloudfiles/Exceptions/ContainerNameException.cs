///
/// See COPYING file for licensing information
///

using System;
using com.mosso.cloudfiles.utils;

namespace com.mosso.cloudfiles.exceptions
{
    /// <summary>
    /// Thrown when the name of the container is invalid
    /// </summary>
    public class ContainerNameException : Exception
    {
        /// <summary>
        /// The default constructor
        /// </summary>
        public ContainerNameException() : this("Either the container name exceeds the maximum length of " + ContainerNameValidator.MAX_CONTAINER_NAME_LENGTH + " or it has invalid characters (/ or ?)")
        {
        }

        /// <summary>
        /// A constructor for describing the explicit issue with the container name
        /// </summary>
        /// <param name="message">A message indicating the explicit issue with the container name.</param>
        public ContainerNameException(String message) : base(message)
        {
        }
    }
}