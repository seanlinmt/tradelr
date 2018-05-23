<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<OrderComment>" %>
<%@ Import Namespace="tradelr.Models.comments"%>
<div class="activity_row" alt="<%= Model.id %>" parent="<%= Model.parentid %>" style="margin-left:<%= Model.leftmargin %>px">
<div class="profile_photo"><a class="userlink" href="#">
    <%= Model.profileLink%></a>
    </div>
<div class="content">
<div class="title">
            <%= Model.comment %>
        </div>
<div class="by"><span class="creator"><a class="userlink" href="<%= Model.creatorLink %>"><%= Model.creatorName %></a></span></div>
        <div class="time">
           <%= Model.dateCreated%>
        </div>
        <% if (Model.hideReply)
           { %>
        <div class="reply">
        reply
    </div>
    <% } %>
</div>
    <div class="clear_right"></div>
</div>
