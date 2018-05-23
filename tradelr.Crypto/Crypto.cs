using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;

namespace tradelr.Crypto
{
    /// <summary>
    /// Cryptographic utility functions.
    /// </summary>
    /// <remarks>
    /// <para>
    
    /// </para>
    /// </remarks>
    public class Crypto
    {
        /// <summary>
        /// HMAC algorithm to use
        /// </summary>
        ///
        private const String HMAC_TYPE = "HMACSHA1";

        /// <summary>
        /// minimum safe length for hmac keys (this is good practice, but not
        /// actually a requirement of the algorithm
        /// </summary>
        ///
        private const int MIN_HMAC_KEY_LEN = 8;

        /// <summary>
        /// Encryption algorithm to use
        /// </summary>
        ///
        private const String CIPHER_TYPE = "AES/CBC/PKCS5Padding";

        private const String CIPHER_KEY_TYPE = "AES";

        /// <summary>
        /// Use keys of this length for encryption operations
        /// </summary>
        ///
        public const int CIPHER_KEY_LEN = 16;

        private const int CIPHER_BLOCK_SIZE = 16;

        /// <summary>
        /// Length of HMAC SHA1 output
        /// </summary>
        ///
        public const int HMAC_SHA1_LEN = 20;

        public static Random rand = new Random();

        // everything is static, no instantiating this class
        private Crypto()
        {
        }

        /// <summary>
        /// Gets a hex encoded random string.
        /// </summary>
        ///
        /// <param name="numBytes">number of bytes of randomness.</param>
        public static String getRandomString(int numBytes)
        {
            return BitConverter.ToString(getRandomBytes(numBytes)).Replace("-", "").ToLower();
        }

        /// <summary>
        /// Returns strong random bytes.
        /// </summary>
        ///
        /// <param name="numBytes">number of bytes of randomness</param>
        public static byte[] getRandomBytes(int numBytes)
        {
            byte[] xout = new byte[numBytes];
            rand.NextBytes(xout);
            return xout;
        }

        /// <summary>
        /// HMAC sha1
        /// </summary>
        ///
        /// <param name="key">the key must be at least 8 bytes in length.</param>
        /// <param name="ins0">byte array to HMAC.</param>
        /// <returns>the hash</returns>
        /// @throws GeneralSecurityException
        public static byte[] HmacSha1(byte[] key, byte[] ins0)
        {
            if (key.Length < MIN_HMAC_KEY_LEN)
            {
                throw new Exception("HMAC key should be at least "
                        + MIN_HMAC_KEY_LEN + " bytes.");
            }
            HMACSHA1 hmac = new HMACSHA1(key);
            hmac.Initialize();
            return hmac.ComputeHash(ins0);
        }

        /// <summary>
        /// Verifies an HMAC SHA1 hash. Throws if the verification fails.
        /// </summary>
        ///
        /// <param name="key"></param>
        /// <param name="ins0"></param>
        /// <param name="expected"></param>
        /// @throws GeneralSecurityException
        public static void HmacSha1Verify(byte[] key, byte[] ins0, byte[] expected)
        {

            HMACSHA1 hmac = new HMACSHA1(key);
            byte[] actual = hmac.ComputeHash(ins0);
            if (actual.Length != expected.Length)
            {
                throw new Exception("HMAC verification failure");
            }
            if (!actual.SequenceEqual(expected))
            {
                throw new Exception("HMAC verification failure");
            }
        }

        /// <summary>
        /// AES-128-CBC encryption. The IV is returned as the first 16 bytes of the
        /// cipher text.
        /// </summary>
        ///
        /// <param name="key"></param>
        /// <param name="plain"></param>
        /// <returns>the IV and cipher text</returns>
        /// @throws GeneralSecurityException
        public static byte[] Aes128cbcEncrypt(byte[] key, byte[] plain)
        {
            RijndaelManaged symmetricKey = new RijndaelManaged {Mode = CipherMode.CBC, KeySize = 128};
            byte[] iv = symmetricKey.IV;
            ICryptoTransform encryptor = symmetricKey.CreateEncryptor(key, iv);
            MemoryStream memoryStream = new MemoryStream();
            CryptoStream cryptoStream = new CryptoStream(memoryStream,
                                                         encryptor,
                                                         CryptoStreamMode.Write);
            cryptoStream.Write(plain, 0, plain.Length);
            cryptoStream.FlushFinalBlock();
            byte[] cipherText = memoryStream.ToArray();
            memoryStream.Close();
            cryptoStream.Close();
            return Concat(iv, cipherText);
        }

        /// <summary>
        /// AES-128-CBC decryption. The IV is assumed to be the first 16 bytes of the
        /// cipher text.
        /// </summary>
        ///
        /// <param name="key"></param>
        /// <param name="cipherText"></param>
        /// <returns>the plain text</returns>
        /// @throws GeneralSecurityException
        public static byte[] Aes128cbcDecrypt(byte[] key, byte[] cipherText)
        {
            byte[] iv = new byte[CIPHER_BLOCK_SIZE];
            Array.Copy(cipherText, 0, iv, 0, iv.Length);
            return Aes128cbcDecryptWithIv(key, iv, cipherText, iv.Length);
        }

        /// <summary>
        /// AES-128-CBC decryption with a particular IV.
        /// </summary>
        ///
        /// <param name="key">decryption key</param>
        /// <param name="iv">initial vector for decryption</param>
        /// <param name="cipherText">cipher text to decrypt</param>
        /// <param name="offset">offset into cipher text to begin decryption</param>
        /// <returns>the plain text</returns>
        /// @throws GeneralSecurityException
        public static byte[] Aes128cbcDecryptWithIv(byte[] key, byte[] iv, byte[] cipherText, int offset)
        {
            RijndaelManaged symmetricKey = new RijndaelManaged {Mode = CipherMode.CBC};
            int cipherLength = cipherText.Length - offset;
            ICryptoTransform decryptor = symmetricKey.CreateDecryptor(key, iv);
            MemoryStream memoryStream = new MemoryStream(cipherText, offset, cipherLength);
            CryptoStream cryptoStream = new CryptoStream(memoryStream,
                                                         decryptor,
                                                         CryptoStreamMode.Read);
            byte[] plain = new byte[cipherLength];
            int decryptedLength = cryptoStream.Read(plain, 0, plain.Length);
            memoryStream.Close();
            cryptoStream.Close();
            byte[] decrypted = new byte[decryptedLength];
            Array.Copy(plain, decrypted, decryptedLength);
            return decrypted;
        }

        /// <summary>
        /// Concatenate two byte arrays.
        /// </summary>
        ///
        public static byte[] Concat(byte[] a, byte[] b)
        {
            byte[] xout = new byte[a.Length + b.Length];
            int cursor = 0;
            Array.Copy(a, 0, xout, cursor, a.Length);
            cursor += a.Length;
            Array.Copy(b, 0, xout, cursor, b.Length);
            return xout;
        }
    } 
}
