using System;

namespace tradelr.Crypto
{
    /// <summary>
    /// For all exceptions thrown by BlobCrypter
    /// </summary>
    /// <remarks>
    /// <para>
    
    /// </para>
    /// </remarks>
    [Serializable]
    public class BlobCrypterException : Exception
    {
        public BlobCrypterException(Exception cause)
            : base(cause.Message, cause)
        {
        }

        public BlobCrypterException(String msg, Exception cause)
            : base(msg, cause)
        {
        }

        protected internal BlobCrypterException(String msg)
            : base(msg)
        {
        }
    }
}