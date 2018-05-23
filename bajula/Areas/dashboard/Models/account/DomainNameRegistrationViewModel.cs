using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using tradelr.Models;

namespace tradelr.Areas.dashboard.Models.account
{
    public class DomainNameRegistrationViewModel : BaseViewModel
    {
        public string domain_name { get; set; }

        public DomainNameRegistrationViewModel(BaseViewModel baseviewmodel) : base(baseviewmodel)
        {
            
        }
    }
}