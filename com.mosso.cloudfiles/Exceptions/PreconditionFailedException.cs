///
/// See COPYING file for licensing information
///

using System;

namespace com.mosso.cloudfiles.exceptions
{
    /// <summary>
    /// 
    /// </summary>
    public class PreconditionFailedException : Exception
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="msg"></param>
        public PreconditionFailedException(string msg) : base(msg)
        {
        }
    }
}