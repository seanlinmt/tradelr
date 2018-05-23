using System;
using System.Collections.Generic;
using System.Text;

namespace tradelr.Crypto.token
{
    public class BasicSecurityToken
    {

        /** serialized form of the token */
        private readonly String token;

        /** data from the token */
        private readonly Dictionary<string, string> tokenData;

        /** tool to use for signing and encrypting the token */
        private readonly BasicBlobCrypter crypter = new BasicBlobCrypter(Encoding.UTF8.GetBytes(AESCrypt.key));

        private const string USER_ID_KEY = "u";
        private const string USER_NAME_KEY = "n";
        private const string USER_GROUP_KEY = "g";
        private const string USER_PERM_KEY = "p";
        private const string DOMAIN_KEY = "d";
        private const string MODULE_KEY = "m";

        public string Serialize()
        {
            return token;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="token"></param>
        /// <param name="maxAge">age of token in seconds</param>
        public BasicSecurityToken(String token)
        {
            this.token = token;
            tokenData = crypter.Unwrap(token);
        }

        public BasicSecurityToken(long userid, string username, int usergroup, long permission, int domain, string moduleId, DateTime expires)
        {
            tokenData = new Dictionary<string, string>();
            PutNullSafe(USER_ID_KEY, userid.ToString());
            PutNullSafe(USER_NAME_KEY, username);
            PutNullSafe(USER_GROUP_KEY, usergroup.ToString());
            PutNullSafe(USER_PERM_KEY, permission.ToString());
            PutNullSafe(DOMAIN_KEY, domain.ToString());
            PutNullSafe(MODULE_KEY, moduleId);

            token = crypter.Wrap(tokenData, expires);
        }

        public static BasicSecurityToken Decode(string token) 
        {
            return new BasicSecurityToken(token);
        }

        /**
        * Generates a token from an input array of values
        * @param owner owner of this gadget
        * @param viewer viewer of this gadget
        * @param app application id
        * @param domain domain of the container
        * @param appUrl url where the application lives
        * @param moduleId module id of this gadget 
        * @throws BlobCrypterException 
        */
        public static BasicSecurityToken Encode(long userid, string username, int usergroup, long permission, int domainid, string moduleId, DateTime expires) 
        {
            return new BasicSecurityToken(userid, username, usergroup, permission, domainid, moduleId, expires);
        }

        private void PutNullSafe(String key, String value)
        {
            if (!String.IsNullOrEmpty(value))
            {
                tokenData.Add(key, value);
            }
        }

        public string Domain
        {
            get { return tokenData[DOMAIN_KEY]; }
        }
        public string UserID
        {
            get { return tokenData[USER_ID_KEY]; }
        }
        public string UserName
        {
            get
            {
                if (tokenData.ContainsKey(USER_NAME_KEY))
                {
                    return tokenData[USER_NAME_KEY];
                }
                return null;
            }
        }
        public string Group
        {
            get { return tokenData[USER_GROUP_KEY]; }
        }
        public string Permission
        {
            get
            {
                if (tokenData.ContainsKey(USER_PERM_KEY))
                {
                    return tokenData[USER_PERM_KEY];
                }
                return null;
            }
        }
        public string ModuleID
        {
            get { return tokenData[MODULE_KEY]; }
        }
    }
}