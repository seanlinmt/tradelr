///
/// See COPYING file for licensing information
/// 

namespace com.mosso.cloudfiles.domain
{
    /// <summary>
    /// ProxyCredentials
    /// </summary>
    public class ProxyCredentials
    {
        private readonly string proxyAddress;
        private readonly string proxyUsername;
        private readonly string proxyPassword;
        private readonly string proxyDomain;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="proxyAddress"></param>
        /// <param name="proxyUsername"></param>
        /// <param name="proxyPassword"></param>
        /// <param name="proxyDomain"></param>
        public ProxyCredentials(string proxyAddress, string proxyUsername, string proxyPassword, string proxyDomain)
        {
            this.proxyAddress = proxyAddress;
            this.proxyDomain = proxyDomain;
            this.proxyPassword = proxyPassword;
            this.proxyUsername = proxyUsername;
        }

        /// <summary>
        /// 
        /// </summary>
        public string ProxyDomain
        {
            get { return proxyDomain; }
        }

        /// <summary>
        /// 
        /// </summary>
        public string ProxyPassword
        {
            get { return proxyPassword; }
        }

        /// <summary>
        /// 
        /// </summary>
        public string ProxyUsername
        {
            get { return proxyUsername; }
        }

        /// <summary>
        /// 
        /// </summary>
        public string ProxyAddress
        {
            get { return proxyAddress; }
        }
    }
}