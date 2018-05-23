using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace tradelr.Models.payment
{
    public interface IPaymentWorker
    {
        string GetPaymentUrl();
    }
}