using Google.GData.Client;

namespace GoogleBase
{
    public class AccountQuery : FeedQuery
    {
        private const string accountFeedBaseUri = "https://content.googleapis.com/content/v1/";
        private const string accountDataType = "managedaccounts";

        private string parentAccountId { get; set; }
        private string clientAccountId { get; set; }

         /// <summary>
        /// Constructor
        /// </summary>
        public AccountQuery()
            : base(accountFeedBaseUri)
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public AccountQuery(string parentId, string accountId)
            :base(accountFeedBaseUri)
        {
            parentAccountId = parentId;
            clientAccountId = accountId;
        }

        public AccountQuery(string parentId)
            : base(accountFeedBaseUri)
        {
            parentAccountId = parentId;
        }

        protected override string GetBaseUri()
        {
            return string.Format("{0}{1}/managedaccounts", accountFeedBaseUri, parentAccountId);
        }
    }
}
