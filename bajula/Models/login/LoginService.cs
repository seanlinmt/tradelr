using System;
using tradelr.DataAccess;
using tradelr.DBML;

namespace tradelr.Models.login
{
    public class LoginService
    {
        private LoginProvider provider;
        public enum ServiceType
        {
            OPENID,
            FACEBOOK
        }

        public LoginService(ServiceType type)
        {
            switch (type)
            {
                case ServiceType.OPENID:
                    provider = Activator.CreateInstance<OpenIDProvider>();
                    break;
                case ServiceType.FACEBOOK:
                    provider = Activator.CreateInstance<FacebookProvider>();
                    break;
            }
        }

        public bool DoesIDExist(string id)
        {
            return provider.DoesIDExist(id);
        }

        public user GetUserByProviderID(string id)
        {
            return provider.GetUserByProviderID(id);
        }
    }
}
