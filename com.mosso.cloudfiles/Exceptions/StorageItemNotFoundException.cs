///
/// See COPYING file for licensing information
///

using System;

namespace com.mosso.cloudfiles.exceptions
{
    /// <summary>
    /// This exception is thrown when the requested storage item does not exist on cloudfiles in the container specified 
    /// </summary>
    public class StorageItemNotFoundException : Exception
    {
        /// <summary>
        /// The default constructor
        /// </summary>
        public StorageItemNotFoundException()
        {
        }

        /// <summary>
        /// A constructor for more explicitly describing the reason for failure
        /// </summary>
        /// <param name="msg">The message detailing the failure</param>
        public StorageItemNotFoundException(string msg) : base(msg)
        {
        }
    }
}