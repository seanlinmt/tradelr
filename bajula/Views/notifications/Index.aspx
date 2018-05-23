<%@ Page Title="" Language="C#" Inherits="System.Web.Mvc.ViewPage<tradelr.Models.notifications.NotificationViewData>" MasterPageFile="~/Views/Shared/Site.Master" %>
<%@ Import Namespace="tradelr.Libraries.Extensions" %>
<asp:Content runat="server" ID="Title" ContentPlaceHolderID="TitleContent">Notifications</asp:Content>
<asp:Content runat="server" ID="Main" ContentPlaceHolderID="MainContent">
<div class="content_area_2col">
        <div id="content_main">
        <div class="section_header">
                Notifications</div>
            <div class="form_group">
            <% foreach (var notification in Model.notificationList){%>
  <div class="notification_row" alt="<%= notification.id %>">
    <div class="profile_photo"><a target="_blank" href="<%= notification.senderProfileUrl %>">
    <img src="<%= notification.senderProfilePhoto %>" /></a>
    </div>
    <div class="content">
        <div class="title">
            <%= notification.title %>
        </div>
        <div class="body">
        By linking inventories, both of you will be able to view each other's products and inventory levels.</div>
        <div class="pt5 mb10"><button class="accept small green" type="button">accept request</button> 
        <button class="ignore small" type="button">silently ignore request</button>
       </div>
</div>
    <div class="clear_right"></div>
</div>
<%} %>
            </div>
        </div>
        <div id="content_side">
                </div>
        <div class="clear">
        </div>
    </div>
</asp:Content>
<asp:Content runat="server" ID="AdditionalJs" ContentPlaceHolderID="AdditionalJS">
<%= Html.RegisterViewJS() %>
</asp:Content>
