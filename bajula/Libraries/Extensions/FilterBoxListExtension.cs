using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace tradelr.Libraries.Extensions
{
    public static class FilterBoxListExtension
    {
        public static IEnumerable<FilterBoxListInfo> ToFilterList(this IEnumerable<SelectListItem> values)
        {
            foreach (var value in values)
            {
                yield return new FilterBoxListInfo()
                {
                    //details = value.details,
                    id = value.Value,
                    title = value.Text
                };
            }
        }

        public static string FilterBoxList(this HtmlHelper htmlHelper, string name, IEnumerable<FilterBoxListInfo> listInfo)
        {
            return FilterBoxList(htmlHelper, name, listInfo, null);
        }

        public static string FilterBoxList(this HtmlHelper htmlHelper, string name, IEnumerable<FilterBoxListInfo> listInfo,
                                          object htmlAttributes)
        {
            return FilterBoxList(htmlHelper, name, listInfo, new RouteValueDictionary(htmlAttributes));
        }

        public static string FilterBoxList(this HtmlHelper htmlHelper, string name, IEnumerable<FilterBoxListInfo> listInfo,
                                          IDictionary<string, object> htmlAttributes)
        {
            if (String.IsNullOrEmpty(name))
                throw new ArgumentException("The argument must have a value", "name");
            if (listInfo == null)
            {
                return "";
            }

            return listInfo.ToFilterBoxList();
        }

        public static string ToFilterBoxList(this IEnumerable<FilterBoxListInfo> values)
        {
            StringBuilder sb = new StringBuilder();

            foreach (FilterBoxListInfo info in values)
            {
                if (!info.isSub)
                {
                    sb.Append("<div class='sideboxEntry' fid='");
                }
                else
                {
                    sb.Append("<div class='sideboxSubEntry' fid='");
                }
                sb.Append(info.id);
                sb.Append("'><div class='title'>");
                sb.Append(info.title);
                sb.Append("</div>");
                if (!string.IsNullOrEmpty(info.details))
                {
                    sb.Append(info.details);
                }
                if (info.allowDelete)
                {
                    sb.Append("<div class='del'></div>");
                }
                sb.Append("</div>");
            }

            return sb.ToString();
        }
    }
}
