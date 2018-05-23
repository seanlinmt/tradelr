using System;

namespace tradelr.Crypto
{
    /// <summary>
    /// Thrown when a blob has expired.
    /// </summary>
    [Serializable]
    public class BlobExpiredException : BlobCrypterException
    {
        public BlobExpiredException(long now, long maxTime)
            : base("Blob expired, was valid till " + new DateTime(maxTime) + ", attempted use at " + new DateTime(now))
        {
        }

    }
}