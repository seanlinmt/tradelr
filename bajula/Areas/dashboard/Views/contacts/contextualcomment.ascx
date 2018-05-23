<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<OrderComment>" %>
<%@ Import Namespace="tradelr.Models.comments" %>
<div class="activity_row" alt="<%= Model.id %>">
    <div class="title">
        <%= Model.comment %>
    </div>
    <div class="smaller">
        <a href="<%= Model.contextLink %>">
            <%= Model.contextName %></a>
    </div>
    <div class="badge">
        ○
        <%= Model.type %></div>

<div class="time">
    <%= Model.dateCreated%>
</div>
<div class="clear_right">
</div>
</div>
