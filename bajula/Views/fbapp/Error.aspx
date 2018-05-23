<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/facebook/FacebookTab.Master" Inherits="System.Web.Mvc.ViewPage<tradelr.Models.facebook.app.FacebookPageViewModel>" %>
<%@ Import Namespace="tradelr.Library.Constants" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
<div class="error">
<div class="icon">
<img src="/Content/img/headings/heading_error.png" alt="" />
</div>
<div class="errorMsg">
<%= Model.errorMessage %>. 
</div>
<p>
<a target="_top" href="http://www.facebook.com/pages/n/<%= Model.pageID %>?v=app_<%= GeneralConstants.FACEBOOK_APP_ID %>">Click here to try again</a>.
</p>
</div>

</asp:Content>
