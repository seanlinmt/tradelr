<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/facebook/FacebookCanvas.Master" Inherits="System.Web.Mvc.ViewPage" %>
<%@ Import Namespace="tradelr.Library.Constants" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
<div class="app_install_container">
<div id="app_install">
<div id="app_install_intro">
<ul class="list_circle mt10">
<li>Let your customers shop without leaving Facebook</li>
<li>Promote your page by revealing discount codes when your page is liked</li>
<li>Allow friends and fans to share your products</li>
</ul>
<p id="app_install_button">
<a id="button_install" target="_top" href="https://graph.facebook.com/oauth/authorize?scope=email&client_id=<%= GeneralConstants.FACEBOOK_APP_ID %>&redirect_uri=<%= GeneralConstants.HTTP_SECURE %>/fbapp/tab">Install</a>
</p>
</div>
<img src="/Content/img/facebook/example.jpg"/>
</div>
<div id="app_features">
<h2>Don't have a store yet?</h2>
<p><a target="_blank" href="http://www.tradelr.com">Start one now »</a></p>
<img src="/Content/img/facebook/features.jpg" alt="Some of our features" />
</div>
</div>
</asp:Content>