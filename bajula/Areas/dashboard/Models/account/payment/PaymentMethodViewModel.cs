using System.Collections.Generic;
using System.Web.Mvc;
using tradelr.DBML;
using tradelr.Libraries.Helpers;
using tradelr.Library;
using tradelr.Library.payment;

namespace tradelr.Areas.dashboard.Models.account.payment
{
    public class PaymentMethodViewModel
    {
        public string id { get; set; }
        public string name { get; set; }
        public string instructions { get; set; }

        public IEnumerable<SelectListItem> methodList { get; set; }
        public string identifier { get; set; }
    }

    public static class PaymentMethodHelper
    {
        public static IEnumerable<PaymentMethodViewModel> ToModel(this IEnumerable<paymentMethod> rows)
        {
            foreach (var row in rows)
            {
                yield return row.ToModel();
            }
        }

        private static PaymentMethodViewModel ToModel(this paymentMethod row)
        {
            return new PaymentMethodViewModel()
            {
                instructions = row.instructions,
                id = row.id.ToString(),
                name = row.method == PaymentMethod.Other.ToString()? row.name: row.method.ToEnum<PaymentMethod>().ToDescriptionString(),
                identifier = row.identifier
            };
        }

        public static PaymentMethodViewModel ToFullModel(this paymentMethod row)
        {
            return new PaymentMethodViewModel
                             {
                                 id = row.id.ToString(),
                                 instructions = row.instructions,
                                 identifier = row.identifier,
                                 name = row.name,
                                 methodList = typeof (PaymentMethod).ToSelectList(true, null, null, false, row.method)
                             };

        }
    }

}