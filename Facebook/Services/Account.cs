namespace clearpixels.Facebook.Services
{
    public class Account : RestBase
    {
        protected internal Account(string token) : base(token)
        {

        }

        public ResponseCollection<Resources.Account> GetAccountTokens(string id)
        {
            method = "GET";
            return SendRequest<ResponseCollection<Resources.Account>>(id + "/accounts");
        }
    }
}
