///
/// See COPYING file for licensing information
///

using System;

namespace com.mosso.cloudfiles.exceptions
{
    /// <summary>
    /// 
    /// </summary>
    public class NoContainersFoundException : Exception
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="msg"></param>
        public NoContainersFoundException(string msg) : base(msg)
        {
        }
    }
}