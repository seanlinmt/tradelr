<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<tradelr.Models.store.LoginPanel>" %>
<div style="position:absolute; top:4px; right:4px; white-space:nowrap; height:20px;">
<% if (Model.isLoggedIn) { %>
    <span style="display: inline;" id="logged_in_controls">
        <a id="logout_link" target="_top" href="<%= Model.hostName %>/logout">
        <img alt="logout" id="logout_image" src="/Content/img/store/logout_link.png"/></a>
        <a id="dashboard_link" target="_top" href="<%= Model.hostName %>/dashboard">
        <img alt="dashboard" src="/Content/img/store/dashboard_link.png"/></a>
    </span>
    <% } else { %>
    <span style="display: inline;" id="logged_out_controls">
        <a id="login_link" target="_top" href="<%= Model.hostName %>/login">
        <img alt="login" src="/Content/img/store/login_link.png"/></a>
    </span>
    <% } %>
</div>
