<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<IEnumerable<ClearPixels.Facebook.Resources.IdName>>" %>
<%@ Import Namespace="tradelr.Library.Constants" %>
<%
    foreach (var user in Model)
    {
%>
<div id="activity_<%= user.id %>" class="activity_row">
    <div class="profile_photo">
        <a class="userlink" href="#" alt="<%= user.id %>">
            <img class="userphoto" src="<%= GeneralConstants.FACEBOOK_GRAPH_HOST %><%= user.id %>/picture" /></a>
    </div>
    <div class="content">
        <div class="body">
            <div class="inline">
                <div class="caption">
                    <%= user.name %> 
                </div>
                <span class="userlocation"></span> <a class="userwebsite" href=""></a>
            </div>
        </div>
    </div>
    <div class="clear">
    </div>
</div>
<%    } %>
