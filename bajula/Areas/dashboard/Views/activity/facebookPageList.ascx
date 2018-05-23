<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<IEnumerable<ClearPixels.Facebook.Resources.Page>>" %>
<%@ Import Namespace="tradelr.Library.Constants" %>
   <% foreach (var page in Model)
    {%>
<div id="activity_<%= page.id %>" class="activity_row">
    <div class="profile_photo">
            <a class="userlink" href="#" alt="<%= page.id %>">
            <img class="userphoto" src="<%= GeneralConstants.FACEBOOK_GRAPH_HOST %><%= page.id %>/picture" /></a>
    </div>
    <div class="content">
        <div class="title">
            <%= page.name %>
        </div>
        <div class="body">
            <div class="inline">
                <div class="caption">
                </div>
                <div class="overview"></div>
            </div>
        </div>
        
        <div class="by">
        <div class="clear_right"></div>
            <span class="creator"><%= page.category %></span></div>
        <div class="time">
            <span class="fan_count"></span>
        <a class="pagelink" target="_blank" href="javascript:return false;">open page</a>
        </div>
    </div>
    <div class="clear">
    </div>
</div>
<%    } %>
