using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace tradelr.Crypto
{
    public class AESCrypt
    {
        internal const string key = "LLinMengTeck1976LLinMefgT88k1976";
        private readonly byte[] keybytes;
        public string Encrypt(string plaintext, string iv)
        {
            var ivbytes = Encoding.ASCII.GetBytes(iv);
            var cipherbytes = encryptStringToBytes_AES(plaintext, keybytes, NormalizeArrayTo16(ivbytes));
            return Convert.ToBase64String(cipherbytes);
        }

        public string Decrypt(string ciphertext, string iv)
        {
            var ivbytes = Encoding.ASCII.GetBytes(iv);
            var cipherbytes = Convert.FromBase64String(ciphertext);
            return decryptStringFromBytes_AES(cipherbytes, keybytes, NormalizeArrayTo16(ivbytes));
        }

        public AESCrypt()
        {
            keybytes = Encoding.ASCII.GetBytes(key);
        }

        static byte[] NormalizeArrayTo16(byte[] inputarray)
        {
            var newarray = new byte[16]
                               {
                                   0x53, 0x6f, 0x64, 0x69, 0x75, 0x6d, 0x20, 0x10,
                                   0x43, 0x68, 0x6c, 0x6f, 0x72, 0x69, 0x64, 0x65
                               };
            Buffer.BlockCopy(inputarray, 0, newarray, 0, inputarray.Length);
            return newarray;
        }


        static byte[] encryptStringToBytes_AES(string plainText, byte[] Key, byte[] IV)
        {
            // Check arguments.
            if (plainText == null || plainText.Length <= 0)
                throw new ArgumentNullException("plainText");
            if (Key == null || Key.Length <= 0)
                throw new ArgumentNullException("Key");
            if (IV == null || IV.Length <= 0)
                throw new ArgumentNullException("IV");

            // Declare the stream used to encrypt to an in memory
            // array of bytes.
            MemoryStream msEncrypt = null;

            // Declare the RijndaelManaged object
            // used to encrypt the data.
            RijndaelManaged aesAlg = null;

            try
            {
                // Create a RijndaelManaged object
                // with the specified key and IV.
                aesAlg = new RijndaelManaged();
                aesAlg.Key = Key;
                aesAlg.IV = IV;

                // Create an encryptor to perform the stream transform.
                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                // Create the streams used for encryption.
                msEncrypt = new MemoryStream();
                using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                {
                    using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                    {

                        //Write all data to the stream.
                        swEncrypt.Write(plainText);
                    }
                }
            }
            finally
            {
                // Clear the RijndaelManaged object.
                if (aesAlg != null)
                    aesAlg.Clear();
            }

            // Return the encrypted bytes from the memory stream.
            return msEncrypt.ToArray();
        }

        static string decryptStringFromBytes_AES(byte[] cipherText, byte[] Key, byte[] IV)
        {
            // Check arguments.
            if (cipherText == null || cipherText.Length <= 0)
                throw new ArgumentNullException("cipherText");
            if (Key == null || Key.Length <= 0)
                throw new ArgumentNullException("Key");
            if (IV == null || IV.Length <= 0)
                throw new ArgumentNullException("IV");

            // Declare the RijndaelManaged object
            // used to decrypt the data.
            RijndaelManaged aesAlg = null;

            // Declare the string used to hold
            // the decrypted text.
            string plaintext = null;

            try
            {
                // Create a RijndaelManaged object
                // with the specified key and IV.
                aesAlg = new RijndaelManaged();
                aesAlg.Key = Key;
                aesAlg.IV = IV;

                // Create a decrytor to perform the stream transform.
                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);
                // Create the streams used for decryption.
                using (MemoryStream msDecrypt = new MemoryStream(cipherText))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {
                            // Read the decrypted bytes from the decrypting stream
                            // and place them in a string.
                            plaintext = srDecrypt.ReadToEnd();
                        }
                    }
                }
            }
            finally
            {
                // Clear the RijndaelManaged object.
                if (aesAlg != null)
                    aesAlg.Clear();
            }

            return plaintext;
        }

    }
}
