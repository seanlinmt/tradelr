using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace tradelr.Libraries.Extensions
{
    public static class CheckBoxListExtension
    {
        public static string CheckBoxList(this HtmlHelper htmlHelper, string name, List<CheckBoxListInfo> listInfo)
        {
            return CheckBoxList(htmlHelper, name, (List<CheckBoxListInfo>)listInfo, null);
        }

        public static string CheckBoxList(this HtmlHelper htmlHelper, string name, List<CheckBoxListInfo> listInfo,
                                          object htmlAttributes)
        {
            return CheckBoxList(htmlHelper, name, (List<CheckBoxListInfo>)listInfo, new RouteValueDictionary(htmlAttributes));
        }

        public static string CheckBoxList(this HtmlHelper htmlHelper, string name, List<CheckBoxListInfo> listInfo,
                                          IDictionary<string, object> htmlAttributes)
        {
            if (String.IsNullOrEmpty(name))
                throw new ArgumentException("The argument must have a value", "name");
            if (listInfo == null)
                throw new ArgumentNullException("listInfo");
            if (listInfo.Count < 1)
                throw new ArgumentException("The list must contain at least one value", "listInfo");

            var sb = new StringBuilder();
            sb.Append("<ul>");
            foreach (CheckBoxListInfo info in listInfo)
            {
                sb.Append("<li>");
                var builder = new TagBuilder("input");
                if (info.IsChecked) builder.MergeAttribute("checked", "checked");
                builder.MergeAttributes(htmlAttributes);
                builder.MergeAttribute("type", "checkbox");
                builder.MergeAttribute("value", info.Value);
                builder.MergeAttribute("name", name);
                var id = info.Value + "_" + name;
                builder.MergeAttribute("id", id);
                var label = new TagBuilder("label");
                label.MergeAttribute("for", id);
                label.InnerHtml = info.DisplayText;
                builder.InnerHtml = label.ToString(TagRenderMode.Normal);
                sb.Append(builder.ToString(TagRenderMode.Normal));
                sb.Append("</li>");
            }
            sb.Append("</ul>");

            return sb.ToString();
        } 
    }
}
