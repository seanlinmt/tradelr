using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using clearpixels.OAuth;

namespace api.trademe.co.nz.v1
{
    public partial class PhotoUploadRequest
    {
        public void GenerateSignature()
        {
            string key = string.Format("{0}{1}{2}{3}{4}",
                OAuthClient.OAUTH_TRADEME_CONSUMER_KEY, 
                FileName, FileType, IsUsernameAdded, IsWaterMarked);

            var csp = new System.Security.Cryptography.MD5CryptoServiceProvider();
            byte[] bytes = Encoding.UTF8.GetBytes(key);
            byte[] hashedBytes = csp.ComputeHash(bytes, 0, bytes.Length);
            Signature = BitConverter.ToString(hashedBytes).Replace("-","").ToLower(System.Globalization.CultureInfo.InvariantCulture);
        }
    }
}
