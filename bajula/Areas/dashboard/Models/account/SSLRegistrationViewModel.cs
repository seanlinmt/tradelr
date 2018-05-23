using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using tradelr.Models;

namespace tradelr.Areas.dashboard.Models.account
{
    public class SSLRegistrationViewModel : BaseViewModel
    {
        public string ssl_plan { get; set; }

        public SSLRegistrationViewModel(BaseViewModel baseviewmodel)
            : base(baseviewmodel)
        {
            
        }
    }
}