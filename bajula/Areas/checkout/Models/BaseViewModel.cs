using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace tradelr.Areas.checkout.Models
{
    public class BaseViewModel
    {
        protected BaseViewModel(BaseViewModel baseviewmodel)
        {
            this.store_name = baseviewmodel.store_name;
            this.year = baseviewmodel.year;
            this.user_name = baseviewmodel.user_name;
            this.cartid = baseviewmodel.cartid;
            this.isMobileDevice = baseviewmodel.isMobileDevice;
        }

        public BaseViewModel()
        {
            
        }

        public string user_name { get; set; }
        public string store_name { get; set; }
        public int year { get; set; }
        public string cartid { get; set; }
        public bool isMobileDevice { get; set; }
    }
}