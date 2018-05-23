<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<IEnumerable<Activity>>" %>
<%@ Import Namespace="tradelr.Models.activity" %>
<%
    foreach (var activity in Model)
    {
        var localtime = activity.created.ToString("s");
%>
<div id="activity_<%= activity.id %>" class="activity_row">
    <div class="profile_photo">
        <a class="userlink" href="#" alt="<%= activity.contactLink %>">
            <%= activity.profile_url %></a>
    </div>
    <div class="content">
        <div class="title">
            <%= activity.title %>
        </div>
        <div class="body">
            <div class="fl mr10">
                <%= activity.media %></div>
            <div class="inline">
                <%= activity.description %>
                <div class="caption">
                    <%= activity.caption %>
                </div>
            </div>
        </div>
        
        <div class="by">
        <div class="clear_right"></div>
            by <span class="creator"><a class="userlink" href="#" alt="<%= activity.contactLink %>">
                <%= activity.ownerName %></a></span></div>
        <div class="time">
            <%= activity.sourceIcon %>
            <span title="<%= localtime%>"></span>
             <%if (activity.source == ActivitySource.FACEBOOK)
          {%>
        <a href="#" class="comment">comment</a>
        <%
            }%>
        </div>
     <%if (activity.source == ActivitySource.FACEBOOK)
      {%>
    <a href="#" class="showMoreComments block <%= activity.commentsCount != activity.comments.Count() ? "":" hidden" %>">view all <%= activity.commentsCount %> comments</a>
    <ul class="comments">
        <% foreach (var comment in activity.comments)
           {%>
        <li>
            <div class="comment_photo">
                <a class="userlink" href="#" alt="<%= comment.contactLink %>">
                    <%= comment.profile_url %></a>
            </div>
            <div class="comment_content">
            <div>
                <span class="commenter"><%= comment.commenter %></span> <%= comment.message %>
                </div>
                <div class="time">
                    <span title="<%= comment.created %>"></span>
                </div>
            </div>
            <div class="clear"></div>
        </li>
        <%} %>
    </ul>
    <%}%>      
    </div>
    <div class="clear">
    </div>
    
</div>
<%    } %>
