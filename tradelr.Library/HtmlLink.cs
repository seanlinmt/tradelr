using System;
using System.Text;
using tradelr.Library.Constants;
using tradelr.Models.transactions;

namespace tradelr.Library
{
    public class HtmlLink
    {
        private readonly string displayText;
        private readonly string htmllink;
        private readonly string className;
        private readonly long? id;

        public HtmlLink(string text, string link)
        {
            displayText = text;
            htmllink = link;
        }

        public HtmlLink(string text, long id)
        {
            displayText = text;
            this.id = id;
        }
        public HtmlLink(string text, long id, string className)
        {
            displayText = text;
            this.id = id;
            this.className = className;
        }

        public override string ToString()
        {
            return "<a href='" + htmllink + "'>" + displayText + "</a>";
        }

        public string ToContactString()
        {
            if (id == null)
            {
                throw new ArgumentException("ToContactString id NULL");
            }
            StringBuilder sb = new StringBuilder();
            sb.Append("<a ");
            if (!string.IsNullOrEmpty(className))
            {
                sb.Append("class=\"");
                sb.Append(className);
                sb.Append("\"");
            }
            sb.Append(" href='");
            sb.Append(GeneralConstants.URL_SINGLE_CONTACT);
            sb.Append(id);
            sb.Append("'>");
            sb.Append(displayText);
            sb.Append("</a>");
            return sb.ToString();
        }


        /// <summary>
        /// returns url to SHOW product
        /// </summary>
        /// <returns></returns>
        public string ToProductString()
        {
            if (id == null)
            {
                throw new ArgumentException("ToProductString id NULL");
            }
            var sb = new StringBuilder();
            sb.Append("<a ");
            if (!string.IsNullOrEmpty(className))
            {
                sb.Append("class=\"");
                sb.Append(className);
                sb.Append("\"");
            }
            sb.Append(" href='");
            sb.Append(GeneralConstants.URL_SINGLE_PRODUCT_SHOW);
            sb.Append(id);
            sb.Append("'>");
            sb.Append(displayText);
            sb.Append("</a>");
            return sb.ToString();
        }

        public string ToTransactionString(TransactionType type)
        {
            return string.Format("<a class=\"{0}\" href=\"{1}/{2}\">{3}</a>", 
                className,
                type == TransactionType.ORDER ? GeneralConstants.URL_SINGLE_ORDER : GeneralConstants.URL_SINGLE_INVOICE,
                id,
                displayText);
        }
    }
}
