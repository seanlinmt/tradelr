using Google.GData.Extensions;

namespace GoogleBase.elements
{
    public class ReviewsUrl : SimpleElement
    {
        /// <summary>
        /// default constructor for sc:image_link 
        /// </summary>
        public ReviewsUrl()
            : base(AccountForShoppingNameTable.ReviewsUrl,
                AccountForShoppingNameTable.scDataPrefix,
                AccountForShoppingNameTable.BaseNamespace)
        {
        }

        /// <summary>
        /// default constructor for sc:image_link with an initial value
        /// </summary>
        public ReviewsUrl(string value)
            : base(AccountForShoppingNameTable.ReviewsUrl,
                AccountForShoppingNameTable.scDataPrefix,
                AccountForShoppingNameTable.BaseNamespace,
                value)
        {
        }
    }
}
