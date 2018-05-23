///
/// See COPYING file for licensing information
///

using System;

namespace com.mosso.cloudfiles.exceptions
{
    /// <summary>
    /// Thrown when attempting to create a container that already exists in cloudfiles
    /// </summary>
    public class ContainerAlreadyExistsException : Exception
    {

        /// <summary>
        /// The default constructor
        /// </summary>
        public ContainerAlreadyExistsException()
        {
        }
        /// <summary>
        /// A constructor for explaining the nature of the exception
        /// </summary>
        /// <param name="msg">A message describing the failure</param>
        public ContainerAlreadyExistsException(string msg) : base(msg)
        {
        }
    }
}