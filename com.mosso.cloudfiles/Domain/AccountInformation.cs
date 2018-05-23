///
/// See COPYING file for licensing information
///

using System;

namespace com.mosso.cloudfiles.domain
{
    /// <summary>
    /// encapsulates the container count and bytes used by a customer
    /// </summary>
    public class AccountInformation
    {
        /// <summary>
        /// the account information of a customer
        /// </summary>
        /// <param name="containerCount">the number of containers a customer owns</param>
        /// <param name="bytesUsed">the bytes used by a customer</param>
        /// <exception cref="System.ArgumentNullException">Thrown when any of the reference arguments are null</exception>
        public AccountInformation(string containerCount, string bytesUsed)
        {
            if (string.IsNullOrEmpty(containerCount) ||
                string.IsNullOrEmpty(bytesUsed))
                throw new ArgumentNullException();

            ContainerCount = int.Parse(containerCount);
            BytesUsed = long.Parse(bytesUsed);
        }

        /// <summary>
        /// the number of the containers a customer owns
        /// </summary>
        /// <return>number of containers</return>
        public int ContainerCount { get; set; }

        /// <summary>
        /// the bytes used by a customer
        /// </summary>
        /// <return>number of bytes</return>
        public long BytesUsed { get; set; }
    }
}