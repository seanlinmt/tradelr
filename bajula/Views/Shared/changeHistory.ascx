<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<IEnumerable<ChangeHistoryItem>>" %>
<%@ Import Namespace="tradelr.Models.history" %>
<%
    foreach (var item in Model)
    {%>
<div class="changeHistory">
    <div class="content">
        <span class="modified">
            <%= item.timeModified %>: </span><span class="fieldName">
                <%= item.field %></span>
        <%
            if (string.IsNullOrEmpty(item.oldValue) && string.IsNullOrEmpty(item.newValue))
            {%>
        created
        <%}
            else if (string.IsNullOrEmpty(item.oldValue))
            {%>
        changed to <span class="fieldValue">
            <%= item.newValue%></span>
        <%}
            else
            { %>
        changed from <span class="fieldValue">
            <%= item.oldValue%></span> to <span class="fieldValue">
                <%= item.newValue%></span>
        <%} %>
        <span class="creator">by
            <%= item.creator %></span>
    </div>
</div>
<%} %>
