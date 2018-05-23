using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ebay.Resources;
using eBay.Service.Call;
using eBay.Service.Core.Soap;

namespace Ebay
{
    public class UserService : EbayService
    {
        private const int EntriesPerPage = 200;

        public UserService(string token)
            : base(token)
        {
            
        }

        // http://developer.ebay.com/devzone/xml/docs/reference/ebay/getuser.html
        public UserType GetUser(string userid = null)
        {
            GetUserCall apicall = new GetUserCall(api);
            apicall.UserID = userid;
            return apicall.GetUser();
        }

        public IEnumerable<Listing> GetMyEbaySelling(bool active, bool scheduled, bool unsold)
        {
            var listings = new List<Listing>();
            int activepage = 1;
            int scheduledpage = 1;
            int unsoldpage = 1;
            bool moreActiveEntries = true;
            bool moreScheduledEntries = true;
            bool moreUnsoldEntries = true;
            while (moreActiveEntries || moreScheduledEntries || moreUnsoldEntries)
            {
                var apicall = new GetMyeBaySellingCall(api)
                {
                    ActiveList = new ItemListCustomizationType(),
                    ScheduledList = new ItemListCustomizationType(),
                    UnsoldList = new ItemListCustomizationType()
                };

                if (moreActiveEntries)
                {
                    // active listing
                    // only return fixed price items for now
                    apicall.ActiveList.Include = active;
                    apicall.ActiveList.Pagination = new PaginationType() { EntriesPerPage = EntriesPerPage, PageNumber = activepage++ };
                }

                if (moreScheduledEntries)
                {
                    // scheduled list (things scheduled to sell but not listed yet
                    apicall.ScheduledList.Include = scheduled;
                    apicall.ScheduledList.Pagination = new PaginationType() { EntriesPerPage = EntriesPerPage, PageNumber = scheduledpage++ };
                }

                if (moreUnsoldEntries)
                {
                    // unsold list
                    apicall.UnsoldList.Include = unsold;
                    apicall.UnsoldList.Pagination = new PaginationType() { EntriesPerPage = EntriesPerPage, PageNumber = unsoldpage++ };
                }

                // get data
                apicall.GetMyeBaySelling();

                // now parse results
                if (apicall.ActiveListReturn != null &&
                        apicall.ActiveListReturn.ItemArray != null &&
                        apicall.ActiveListReturn.ItemArray.Count > 0)
                {
                    if (apicall.ActiveListReturn.ItemArray.Count < EntriesPerPage)
                    {
                        moreActiveEntries = false;
                    }
                    foreach (ItemType actitem in apicall.ActiveListReturn.ItemArray)
                    {
                        var actlisting = new Listing();
                        actlisting.Populate(actitem);
                        listings.Add(actlisting);
                    }
                }
                else
                {
                    moreActiveEntries = false;
                }

                if (apicall.ScheduledListReturn != null &&
                    apicall.ScheduledListReturn.ItemArray != null &&
                    apicall.ScheduledListReturn.ItemArray.Count > 0)
                {
                    if (apicall.ScheduledListReturn.ItemArray.Count < EntriesPerPage)
                    {
                        moreScheduledEntries = false;
                    }
                    foreach (ItemType scheItem in apicall.ScheduledListReturn.ItemArray)
                    {
                        var slisting = new Listing();
                        slisting.Populate(scheItem);
                        listings.Add(slisting);
                    }
                }
                else
                {
                    moreScheduledEntries = false;
                }

                if (apicall.UnsoldListReturn != null &&
                    apicall.UnsoldListReturn.ItemArray != null &&
                    apicall.UnsoldListReturn.ItemArray.Count > 0)
                {
                    if (apicall.UnsoldListReturn.ItemArray.Count < EntriesPerPage)
                    {
                        moreUnsoldEntries = false;
                    }

                    foreach (ItemType unsoldItem in apicall.UnsoldListReturn.ItemArray)
                    {
                        var unsoldlisting = new Listing();
                        unsoldlisting.Populate(unsoldItem);
                        listings.Add(unsoldlisting);
                    }
                }
                else
                {
                    moreUnsoldEntries = false;
                }

            }

            return listings;
        }
    }
}
