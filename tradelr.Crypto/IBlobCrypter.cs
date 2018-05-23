using System.Collections.Generic;
using System;

namespace tradelr.Crypto
{
    /// <summary>
    /// Utility interface for managing signed, encrypted, and time stamped blobs.
    /// Blobs are made up of name/value pairs. Time stamps are automatically included
    /// and checked.
    /// Thread safe.
    /// </summary>
    /// <remarks>
    /// <para>
    
    /// </para>
    /// </remarks>
    public interface IBlobCrypter
    {
        String Wrap(Dictionary<string, string> ins0, DateTime expires);
        Dictionary<string, string> Unwrap(String ins0);
    }
}