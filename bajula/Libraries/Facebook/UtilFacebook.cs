using System;
using System.Security.Cryptography;
using System.Web;
using FacebookToolkit.Session;
using tradelr.Common.Constants;
using tradelr.Library.Constants;

namespace tradelr.Libraries.Facebook
{
    public static class UtilFacebook
    {
        public static void ClearFacebookCookies(HttpResponseBase response, string apikey)
        {
            string[] cookies = new[] { "user", "session_key", "expires", "ss" };
            foreach (var c in cookies)
            {
                string fullCookie = apikey + "_" + c;
                response.Cookies[fullCookie].Expires = DateTime.Now.AddMonths(-1);
            }

            // clear new oauth cookie
            string oauthCookie = "fbs_" + apikey;
            response.Cookies[oauthCookie].Expires = DateTime.Now.AddMonths(-1);
        }

        public static ConnectSession GetConnectSession()
        {
            return new ConnectSession(GeneralConstants.FACEBOOK_API_KEY, GeneralConstants.FACEBOOK_API_SECRET);
        }

        /**
        * Returns the "public" hash of the email address, i.e., the one we give out
        * to select partners via our API.
        *
        * @param  string _email An email address to hash
        * @return string        A public hash of the form crc32(_email)_md5(_email)
        */
        public static string email_get_public_hash(string email)
        {
            if (!string.IsNullOrEmpty(email))
            {
                email = email.ToLower().Trim();
                byte[] rawBytes = System.Text.UTF8Encoding.UTF8.GetBytes(email);

                CRC32 crc = new CRC32();
                UInt32 crcResult = crc.GetCrc32(new System.IO.MemoryStream(rawBytes));

                MD5 md5 = new MD5CryptoServiceProvider();
                byte[] md5Result = md5.ComputeHash(rawBytes);
                string md5Data = ToHexString(md5Result).ToLower();

                return crcResult.ToString() + "_" + md5Data;
            }
            return "";
        }

        public static ConnectSession GetGenericConnectionSession()
        {
            var connectSession = new ConnectSession(GeneralConstants.FACEBOOK_API_KEY, GeneralConstants.FACEBOOK_API_SECRET);
            connectSession.SessionKey = tradelrSettings.Fb_api_session_key;
            connectSession.UserId = tradelrSettings.Fb_api_session_userid;
            return connectSession;
        }

        private static string ToHexString(byte[] bytes)
        {
            string hexString = "";
            for (int i = 0; i < bytes.Length; i++)
                hexString += bytes[i].ToString("X2");
            return hexString;
        }
    }
}
