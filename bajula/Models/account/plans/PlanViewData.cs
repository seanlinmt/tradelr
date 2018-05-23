namespace tradelr.Models.account.plans
{
    public class PlanViewData : BaseViewModel
    {
        public PlanViewData(BaseViewModel viewmodel) : base(viewmodel)
        {
        }

        public AccountPlanType accountType { get; set; }
        public bool showPayTrialButton { get; set; }
        public string hostName { get; set; }
        public long subdomainid { get; set; }
    }
}