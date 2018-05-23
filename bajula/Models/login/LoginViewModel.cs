using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace tradelr.Models.login
{
    public class LoginViewModel : BaseViewModel
    {
        public string loginPageName { get; set; }
        public bool showRegistrationHelp { get; set; }

        public LoginViewModel(BaseViewModel baseviewdata): base(baseviewdata)
        {
            
        }
    }
}