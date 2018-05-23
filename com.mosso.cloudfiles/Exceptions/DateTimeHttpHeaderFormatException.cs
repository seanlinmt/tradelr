///
/// See COPYING file for licensing information
///

using System;

namespace com.mosso.cloudfiles.exceptions
{
    /// <summary>
    /// This exception is thrown when the date is in a non-HTTP 1.1 compliant format
    /// </summary>
    public class DateTimeHttpHeaderFormatException : Exception
    {
        /// <summary>
        /// The default constructor
        /// </summary>
        public DateTimeHttpHeaderFormatException()
        {
        }

        /// <summary>
        /// A constructor for describing the correct date formatting
        /// </summary>
        /// <param name="message">Used to be more explicit about the acceptable datetime format</param>
        public DateTimeHttpHeaderFormatException(String message)
            : base(message)
        {
        }
    }
}