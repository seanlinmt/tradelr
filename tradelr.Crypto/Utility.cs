using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace tradelr.Crypto
{
    public static class Utility
    {
        /// <summary>
        /// calculates MD5 hash
        /// copied from 
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string CalculateMD5Hash(string input)
        {
            // step 1, calculate MD5 hash from input
            var md5 = System.Security.Cryptography.MD5.Create();
            byte[] inputBytes = Encoding.ASCII.GetBytes(input);
            byte[] hash = md5.ComputeHash(inputBytes);

            // step 2, convert byte array to hex string
            var sb = new StringBuilder();
            for (int i = 0; i < hash.Length; i++)
            {
                sb.Append(hash[i].ToString("X2"));
            }
            return sb.ToString();
        }

        public static string ComputePasswordHash(string id)
        {
            return BCrypt.HashPassword(id, BCrypt.GenerateSalt());
        }

        public static string GetRandomString(int length = 6, bool uppercase = false)
        {
            var values = new[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'a', 'b', 'c', 'd', 'e', 'f', 
                'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 
                'z', 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 
                'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z'};

            var sb = new StringBuilder();
            Random rnd = RandomNumberGenerator.Instance;
            for (int j = 0; j < length; j++)
            {
                var idx = rnd.Next(0, 61);
                sb.Append(values[idx]);
            }

            if (uppercase)
            {
                return sb.ToString().ToUpper();
            }

            return sb.ToString();
        }
    }
}
