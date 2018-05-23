<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<IEnumerable<tradelr.Models.message.Message>>" %>
<%@ Import Namespace="tradelr.DBML.Helper" %>
<%@ Import Namespace="tradelr.Library" %>
<%
    if (Model.Count() != 0)
    {
        foreach (var message in Model)
        {
            var created = String.Format("{0:f}", message.created);

            var title = message.title;
            title = title.Length > 20 ? title.Substring(0, 20) : title;
            var preview = message.body.StripHtmlTags();
            preview = preview.Length > 80 ? preview.Substring(0, 80) + ".." : preview;
            var readStyle = !message.read ? "bold" : "";
%>
<div class="message" id="message<%= message.id %>">
    <div style="float: right; margin: 6px;" class="ui-state-default ui-corner-all">
        <a href="javascript: void(0);" id="removeButton<%=message.id %>"><span id="removeIcon<%=message.id %>"
            class="ui-icon ui-icon-closethick"></span></a>
    </div>
    <div class="who">
        <%= message.sender.ToFullName() %><br />
        <span>
            <%=created%></span></div>
    <div class="preview <%=readStyle%>">
        <%=title%><br />
        <%= preview%></div>
</div>
<div id="dialog<%= message.id%>" title="Delete message?" style="display: none">
    <p>
        <span id="dialogSpan<%=message.id %>" class="ui-icon ui-icon-alert" style="float: left;
            margin: 0 7px 20px 0;"></span>Are you sure you want to delete this message?</p>
</div>
<%}
    }
    else
    {%>
<%= "No messages.."%>
<% }%>
