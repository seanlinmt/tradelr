using tradelr.Models.users;

namespace tradelr.Models
{
    public class BaseViewModel
    {
        public string orgName { get; set; }
        public string manifestFile { get; set; }
        public string notifications { get; set; }
        public UserPermission permission { get; set; }
        public UserRole role { get; set; }
        public string shopUrl { get; set; }
        public bool storeEnabled { get; set; }

        public BaseViewModel()
        {
            
        }

        protected BaseViewModel(BaseViewModel viewmodel)
        {
            if (viewmodel != null)
            {
                this.orgName = viewmodel.orgName;
                this.manifestFile = viewmodel.manifestFile;
                this.notifications = viewmodel.notifications;
                this.role = viewmodel.role;
                this.permission = viewmodel.permission;
                this.shopUrl = viewmodel.shopUrl;
                this.storeEnabled = viewmodel.storeEnabled;
            }
        }
    }
}