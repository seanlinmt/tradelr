using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using tradelr.Common.Library;
using tradelr.Libraries.Helpers;
using tradelr.Library;
using tradelr.Models.account;

namespace tradelr.Libraries.Extensions
{
    public static class ImportAccountDataExtension
    {
        public static string ImportAccountDataTable(this HtmlHelper htmlHelper, string name, Dictionary<AccountDataType, long> accountInfo)
        {
            if (String.IsNullOrEmpty(name))
                throw new ArgumentException("The argument must have a value", "name");
            if (accountInfo == null)
                throw new ArgumentNullException("accountInfo");

            StringBuilder sb = new StringBuilder();
            sb.Append("<table id='importData_" + name + "'><tbody>");
            foreach (var info in accountInfo)
            {
                sb.Append("<tr><td>");
                sb.Append("<input type='checkbox' name='" + info.Key + "' checked='checked' />");
                sb.Append("</td><td>");
                sb.Append(info.Key.ToDescriptionString());
                sb.Append("</td><td>");
                sb.Append(info.Value);
                sb.Append("</td></tr>");
            }
            sb.Append("</tbody></table>");
            return sb.ToString();
        }
    }
}
