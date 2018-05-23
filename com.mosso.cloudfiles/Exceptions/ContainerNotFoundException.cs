///
/// See COPYING file for licensing information
///

using System;

namespace com.mosso.cloudfiles.exceptions
{
    /// <summary>
    /// This exception is thrown when the requested container does not exist on cloudfiles
    /// </summary>
    public class ContainerNotFoundException : Exception
    {

        /// <summary>
        /// Default constructor
        /// </summary>
      
        public ContainerNotFoundException()
        {
        }

        /// <summary>
        /// A constructor for describing the missing containing more explicitly
        /// </summary>
        /// <param name="msg">A message used to explicitly describe the failure when requesting the non-existence container</param>
        public ContainerNotFoundException(string msg) : base(msg)
        {
        }
    }
}