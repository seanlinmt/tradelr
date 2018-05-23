///
/// See COPYING file for licensing information
///

using System;
using com.mosso.cloudfiles.utils;

namespace com.mosso.cloudfiles.domain
{
    /// <summary>
    /// UserCredentials
    /// </summary>
    public class UserCredentials
    {
        private readonly Uri authUrl;
        private readonly string username;
        private readonly string api_access_key;
        private readonly string cloudversion;
        private readonly string accountName;
        private readonly ProxyCredentials proxyCredentials;

        /// <summary>
        /// Constructor - defaults Auth Url to https://api.mosso.com/auth without proxy credentials
        /// </summary>
        /// <param name="username">client username to use during authentication</param>
        /// <param name="api_access_key">client api access key to use during authentication</param>
        public UserCredentials(string username, string api_access_key) :
            this(new Uri(Constants.MOSSO_AUTH_URL), username, api_access_key, null, null)
        {
        }

        /// <summary>
        /// Constructor - defaults Auth Url to https://api.mosso.com/auth with proxy credentials
        /// </summary>
        /// <param name="username">client username to use during authentication</param>
        /// <param name="api_access_key">client api access key to use during authentication</param>
        /// <param name="proxyCredentials">credentials to use to obtain access via proxy</param>
        public UserCredentials(string username, string api_access_key, ProxyCredentials proxyCredentials) :
            this(new Uri(Constants.MOSSO_AUTH_URL), username, api_access_key, null, null, proxyCredentials)
        {
        }

        /// <summary>
        /// UserCredential constructor
        /// </summary>
        /// <param name="authUrl">url to authenticate against</param>
        /// <param name="username">client username to use during authentication</param>
        /// <param name="api_access_key">client api access key to use during authentication</param>
        /// <param name="cloudversion">version of the cloudfiles system to access</param>
        /// <param name="accountName">client account name</param>
        /// <param name="proxyCredentials">credentials to use to obtain access via proxy</param>
        /// <returns>An instance of UserCredentials</returns>
        public UserCredentials(Uri authUrl, string username, string api_access_key, string cloudversion, string accountName, ProxyCredentials proxyCredentials)
        {
            this.authUrl = authUrl;
            this.username = username;
            this.api_access_key = api_access_key;
            this.accountName = accountName;
            this.cloudversion = cloudversion;
            this.proxyCredentials = proxyCredentials;
        }

        /// <summary>
        /// UserCredential constructor
        /// </summary>
        /// <param name="authUrl">url to authenticate against</param>
        /// <param name="username">client username to use during authentication</param>
        /// <param name="api_access_key">client api access key to use during authentication</param>
        /// <param name="cloudversion">version of the cloudfiles system to access</param>
        /// <param name="accountname">client account name</param>
        /// <returns>An instance of UserCredentials</returns>
        public UserCredentials(Uri authUrl, string username, string api_access_key, string cloudversion, string accountname)
            : this(authUrl, username, api_access_key, cloudversion, accountname, null)
        {
        }

        /// <summary>
        /// Proxy Credentials
        /// </summary>
        /// <returns>An instance of the local proxy credentials</returns>
        public ProxyCredentials ProxyCredentials
        {
            get { return proxyCredentials; }
        }


        /// <summary>
        /// api access key to use for authentication
        /// </summary>
        /// <returns>a string representation of the api access key used to authenticate against the user's account</returns>
        public string Api_access_key
        {
            get { return api_access_key; }
        }


        /// <summary>
        /// username to use for authentication
        /// </summary>
        /// <returns>a string representation of the account name used to authenticate against the user's account</returns>
        public string AccountName
        {
            get { return accountName; }
        }

        /// <summary>
        /// the url to authenticate against
        /// </summary>
        /// <returns>a Uri instance having the url for authentication</returns>
        public Uri AuthUrl
        {
            get { return authUrl; }
        }

        /// <summary>
        /// version of the cloudfiles system
        /// </summary>
        /// <returns>a string representation of the cloudfiles system version</returns>
        public string Cloudversion
        {
            get { return cloudversion; }
        }

        /// <summary>
        /// username to use for authentication
        /// </summary>
        /// <returns>a string representation of the username used to authenticate against the user's account</returns>
        public string Username
        {
            get { return username; }
        }
    }
}