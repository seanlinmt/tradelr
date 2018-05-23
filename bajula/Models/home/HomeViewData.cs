using System;
using System.Collections.Generic;
using tradelr.Models.activity;
using tradelr.Models.subdomain;
using tradelr.Models.users;

namespace tradelr.Models.home
{
    public class HomeViewData : BaseViewModel
    {
        public HomeViewData(BaseViewModel viewmodel) : base(viewmodel)
        {
        }

        public Statistics stats { get; set; }
        public int panelIndex { get; set;}
        public ActivityGroup activities { get; set; }
        public bool isAdmin { get; set; }
    }
}
