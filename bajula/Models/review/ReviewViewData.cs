namespace tradelr.Models.review
{
    public class ReviewViewData : BaseViewModel
    {
        public ReviewViewData(BaseViewModel viewmodel)
            : base(viewmodel)
        {
            
        }

        public long transactionID { get; set; }
        public string transactionLink { get; set; }
        public string transactionName { get; set; }
        public long reviewID { get; set; }
    }
}