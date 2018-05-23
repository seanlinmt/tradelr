using System;
using Google.GData.Client;

namespace GoogleBase {
    /// <summary>
    /// Feed API customization class for defining Product feed.
    /// </summary>
    public class AccountFeed : AbstractFeed {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="uriBase">The uri for the product feed.</param>
        /// <param name="iService">The AccountForShopping service.</param>
        public AccountFeed(Uri uriBase, IService iService)
            : base(uriBase, iService) {
        }

        /// <summary>
        /// returns a new entry for this feed
        /// </summary>
        /// <returns>AtomEntry</returns>
        public override AtomEntry CreateFeedEntry() {
            return new AccountEntry();
        }
    }
}
