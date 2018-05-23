///
/// See COPYING file for licensing information
///

using System;

namespace com.mosso.cloudfiles.exceptions
{
    /// <summary>
    /// This exception is thrown when the user attempts to delete a container that still has storage objects associated with it
    /// </summary>
    public class ContainerNotEmptyException : Exception
    {

        /// <summary>
        /// The default constructor
        /// </summary>
        public ContainerNotEmptyException()
        {
        }

        /// <summary>
        /// A constructor for more explicitly explaining the reason for failure
        /// </summary>
        /// <param name="msg">A message for describing the exception in detail</param>
        public ContainerNotEmptyException(string msg) : base(msg)
        {
        }
    }
}