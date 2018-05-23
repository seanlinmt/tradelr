///
/// See COPYING file for licensing information
///

using System;
using com.mosso.cloudfiles.utils;

namespace com.mosso.cloudfiles.exceptions
{
    /// <summary>
    /// Thrown when the name of the object is invalid
    /// </summary>
    public class StorageItemNameException : Exception
    {
        /// <summary>
        /// The default constructor
        /// </summary>
        public StorageItemNameException()
            : this("Either the object name exceeds the maximum length of " + ObjectNameValidator.MAX_OBJECT_NAME_LENGTH + " or it has invalid characters (?)")
        {
        }

        /// <summary>
        /// A constructor for describing the explicit issue with the object name
        /// </summary>
        /// <param name="message">A message indicating the explicit issue with the container name.</param>
        public StorageItemNameException(String message) : base(message)
        {
        }
    }
}