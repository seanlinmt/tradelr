using Google.GData.Client;
using GoogleBase.elements;

namespace GoogleBase
{
    public class AccountEntry : AbstractEntry
    {
        public bool AdultContent
        {
            get
            {
                bool value;
                if (!bool.TryParse(GetStringValue<AdultContent>(AccountForShoppingNameTable.AdultContent,
                    AccountForShoppingNameTable.BaseNamespace), out value))
                {
                    value = false;
                }

                return value;
            }
            set
            {
                SetStringValue<AdultContent>(value.ToString(),
                    AccountForShoppingNameTable.AdultContent,
                    AccountForShoppingNameTable.BaseNamespace);
            }
        }

        public string InternalId
        {
            get
            {
                return GetStringValue<InternalId>(AccountForShoppingNameTable.InternalId,
                    AccountForShoppingNameTable.BaseNamespace);
            }
            set
            {
                SetStringValue<InternalId>(value,
                    AccountForShoppingNameTable.InternalId,
                    AccountForShoppingNameTable.BaseNamespace);
            }
        }

        public string ReviewsUrl
        {
            get
            {
                return GetStringValue<ReviewsUrl>(AccountForShoppingNameTable.ReviewsUrl,
                    AccountForShoppingNameTable.BaseNamespace);
            }
            set
            {
                SetStringValue<ReviewsUrl>(value,
                    AccountForShoppingNameTable.ReviewsUrl,
                    AccountForShoppingNameTable.BaseNamespace);
            }
        }
    }
}
