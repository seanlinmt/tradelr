using System.Collections.Generic;
using System.Web.Mvc;
using tradelr.DBML;
using tradelr.Library;
using tradelr.Library.payment;

namespace tradelr.Models.payment
{
    public class PaymentMethodList
    {
        public List<SelectListItem> items { get; private set; }
        public int count { get { return items.Count; }  }

        public void Initialise(MASTERsubdomain msd, bool excludePaypal)
        {
            foreach (var row in msd.paymentMethods)
            {
                if (row.method == PaymentMethod.Paypal.ToString())
                {
                    if (excludePaypal)
                    {
                        continue;
                    }
                    if (!string.IsNullOrEmpty(msd.GetPaypalID()))
                    {
                        Add(PaymentMethod.Paypal.ToString(), "paypal");
                    }
                }
                else
                {
                    Add(row.method == PaymentMethod.Other.ToString() ? row.name : row.method.ToEnum<PaymentMethod>().ToDescriptionString(), 
                        row.id.ToString());
                }
            }
        }

        private void Add(string name, string value)
        {
            items.Add(new SelectListItem(){ Text = name, Value = value});
        }

        public PaymentMethodList()
        {
            items = new List<SelectListItem>();
        }


    }
}