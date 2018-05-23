using System;
using System.Collections.Generic;
using System.Text;

namespace tradelr.Crypto.token
{
    public class TradelrSecurityToken
    {
        /** serialized form of the token */
        private readonly String token;

        /** data from the token */
        private readonly Dictionary<String, String> tokenData;

        /** tool to use for signing and encrypting the token */
        private readonly BasicBlobCrypter crypter = new BasicBlobCrypter(Encoding.UTF8.GetBytes(AESCrypt.key));

        private const string USER_ID_KEY = "u";
        private const string USER_ROLE_KEY = "r";
        private const string PERMISSION_KEY = "p";
        private const string DOMAIN_KEY = "d";

        public string Serialize()
        {
            return token;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="token"></param>
        /// <param name="maxAge">age of token in seconds</param>
        public TradelrSecurityToken(String token)
        {
            this.token = token;
            tokenData = crypter.Unwrap(token);
        }

        public TradelrSecurityToken(string userid, string role, string perms, DateTime expires)
        {
            tokenData = new Dictionary<String, String>();
            PutNullSafe(USER_ID_KEY, userid);
            PutNullSafe(USER_ROLE_KEY, role);
            PutNullSafe(PERMISSION_KEY, perms);
            token = crypter.Wrap(tokenData, expires);
        }

        public static TradelrSecurityToken Decode(string token) 
        {
            return new TradelrSecurityToken(token);
        }

        public static TradelrSecurityToken Encode(string userid, string role, string perms, DateTime expires) 
        {
            return new TradelrSecurityToken(userid, role, perms, expires);
        }

        private void PutNullSafe(String key, String value)
        {
            if (value != null)
            {
                tokenData.Add(key, value);
            }
        }

        public string Domain
        {
            get { return tokenData[DOMAIN_KEY]; }
        }
        public long UserID
        {
            get { return long.Parse(tokenData[USER_ID_KEY]); }
        }
        public int UserRole
        {
            get { return int.Parse(tokenData[USER_ROLE_KEY]); }
        }
        public int Permission
        {
            get { return int.Parse(tokenData[PERMISSION_KEY]); }
        }
    }
}