using tradelr.Areas.dashboard.Models.store.general;
using tradelr.Areas.dashboard.Models.store.policies;
using tradelr.Models;

namespace tradelr.Areas.dashboard.Models.store
{
    public class StoreSettingsViewModel : BaseViewModel
    {
        public PolicySettings policies { get; set; }
        public GeneralSettings general { get; set; }
        
        public StoreSettingsViewModel(BaseViewModel viewmodel)
            : base(viewmodel)
        {
            
        }
    }
}