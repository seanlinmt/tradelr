<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<tradelr.Models.activity.ActivityUser>" %>
<div class="activity_userpopup">
<div class="userimage">
<a target="_blank" href="<%= Model.profileLink %>">
<img class="w150px h150px overflow_none" src="<%= Model.profileImage %>" alt="view profile on facebook.com" />
</a>
</div>
<div class="userstats">
</div>
<div class="userinfo">
<p><%= Model.about %></p>
</div>
<div class="userlinks">
<p><b>Profile:</b> <a target="_blank" href="<%= Model.profileLink %>"><%= Model.profileLink %></a></p>
</div>
<span class="hidden userid"><%= Model.id %></span>
</div>