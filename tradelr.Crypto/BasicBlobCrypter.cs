

using System.Collections.Generic;
using System.Security.Cryptography;
using System.IO;
using System.Text;
using System;
using System.Web;

namespace tradelr.Crypto
{
    /// <summary>
    /// Simple implementation of BlobCrypter.
    /// </summary>
    public class BasicBlobCrypter : IBlobCrypter
    {
        // Labels for key derivation
        private const byte CIPHER_KEY_LABEL = 0;
        private const byte HMAC_KEY_LABEL = 1;

        /// <summary>
        /// Key used for time stamp (in seconds) of data
        /// </summary>
        ///
        public const String TIMESTAMP_KEY = "t";

        /// <summary>
        /// minimum length of master key
        /// </summary>
        ///
        public const int MASTER_KEY_MIN_LEN = 16;

        private byte[] cipherKey;
        private byte[] hmacKey;

        /// <summary>
        /// Creates a crypter based on a key in a file. The key is the first line in
        /// the file, whitespace trimmed from either end, as UTF-8 bytes.
        /// The following///nix command line will create an excellent key:
        /// dd if=/dev/random bs=32 count=1 | openssl base64 > /tmp/key.txt
        /// </summary>
        ///
        /// @throws IOExceptionif the file can't be read.
        public BasicBlobCrypter(string keyfile)
        {
            using (var reader = new StreamReader(keyfile))
            {
                String line = reader.ReadLine();
                line = line.Trim();
                byte[] keyBytes = Encoding.UTF8.GetBytes(line);
                Init(keyBytes);
            }
        }

        /// <summary>
        /// Builds a BlobCrypter from the specified master key
        /// </summary>
        ///
        /// <param name="masterKey"></param>
        public BasicBlobCrypter(byte[] masterKey)
        {
            Init(masterKey);
        }

        private void Init(byte[] masterKey)
        {
            if (masterKey.Length < MASTER_KEY_MIN_LEN)
            {
                throw new Exception("Key too short, " + masterKey.Length + ": Master key needs at least "
                                    + MASTER_KEY_MIN_LEN + " bytes");
            }
            cipherKey = DeriveKey(CIPHER_KEY_LABEL, masterKey, Crypto.CIPHER_KEY_LEN);
            hmacKey = DeriveKey(HMAC_KEY_LABEL, masterKey, 0);
        }

        /// <summary>
        /// Generates unique keys from a master key.
        /// </summary>
        ///
        /// <param name="label">type of key to derive</param>
        /// <param name="masterKey">master key</param>
        /// <param name="len">length of key needed, less than 20 bytes. 20 bytes are</param>
        /// <returns>a derived key of the specified length</returns>
        private byte[] DeriveKey(byte label, byte[] masterKey, int len)
        {
            byte[] bs = Crypto.Concat(new[] { label }, masterKey);
            SHA1 shash = SHA1.Create();
            byte[] hash = shash.ComputeHash(bs);
            if (len == 0)
            {
                return hash;
            }
            byte[] xout = new byte[len];
            Array.Copy(hash, 0, xout, 0, xout.Length);
            return xout;
        }

        /*
         * (non-Javadoc)
         * 
         * @see org.apache.shindig.util.BlobCrypter#wrap(java.util.Map)
         */
        public virtual String Wrap(Dictionary<string, string> ins0, DateTime expires)
        {
            if (ins0.ContainsKey(TIMESTAMP_KEY))
            {
                throw new ArgumentException(
                    "No 't' keys allowed for BlobCrypter");
            }
            try
            {
                byte[] encoded = serializeAndTimestamp(ins0, expires);
                byte[] cipherText = Crypto.Aes128cbcEncrypt(cipherKey, encoded);
                byte[] hmac = Crypto.HmacSha1(hmacKey, cipherText);
                return Convert.ToBase64String(Crypto.Concat(cipherText, hmac));
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        /// <summary>
        /// Encode the input for transfer. We use something a lot like HTML form
        /// encodings. The time stamp is in seconds since the epoch.
        /// </summary>
        ///
        private byte[] serializeAndTimestamp(Dictionary<string, string> inval, DateTime expires)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var val in inval)
            {
                sb.Append(HttpUtility.UrlEncode(val.Key, Encoding.UTF8));
                sb.Append('=');
                sb.Append(HttpUtility.UrlEncode(val.Value, Encoding.UTF8));
                sb.Append('&');
            }
            sb.Append(TIMESTAMP_KEY);
            sb.Append('=');
            sb.Append(expires.Ticks);
            return Encoding.UTF8.GetBytes(sb.ToString());
        }

        public virtual Dictionary<string, string> Unwrap(String ins0)
        {
            try
            {
                byte[] bin = Convert.FromBase64String(ins0);
                byte[] hmac = new byte[Crypto.HMAC_SHA1_LEN];
                byte[] cipherText = new byte[bin.Length - Crypto.HMAC_SHA1_LEN];
                Array.Copy(bin, 0, cipherText, 0, cipherText.Length);
                Array.Copy(bin, cipherText.Length, hmac, 0, hmac.Length);
                Crypto.HmacSha1Verify(hmacKey, cipherText, hmac);
                byte[] plain = Crypto.Aes128cbcDecrypt(cipherKey, cipherText);
                Dictionary<string, string> xout = Deserialize(plain);
                CheckTimestamp(xout);
                return xout;
            }
            catch (IndexOutOfRangeException e_0)
            {
                throw new BlobCrypterException("Invalid token format", e_0);
            }
            catch (IOException e_2)
            {
                throw new BlobCrypterException(e_2);
            }
            catch (Exception e)
            {
                throw new BlobCrypterException("Invalid token signature", e);
            }
        }

        private static Dictionary<string, string> Deserialize(byte[] plain)
        {
            String bs = Encoding.UTF8.GetString(plain);
            String[] items = bs.Split("[&=]".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            var map = new Dictionary<string, string>();
            for (int i = 0; i < items.Length; )
            {
                String key = HttpUtility.UrlDecode(items[i++]);
                String val = HttpUtility.UrlDecode(items[i++]);
                map.Add(key, val);
            }
            return map;
        }

        /// <summary>
        /// We allow a few minutes on either side of the validity window to account
        /// for clock skew.
        /// </summary>
        ///
        private void CheckTimestamp(IDictionary<string, string> xout)
        {
            long maxTime = long.Parse(xout[TIMESTAMP_KEY]);
            var now = DateTime.UtcNow.Ticks;
            if (now > maxTime)
            {
                throw new BlobExpiredException(now, maxTime);
            }
        }

    }
}