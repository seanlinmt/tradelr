<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Facebook.Master" Inherits="System.Web.Mvc.ViewPage<tradelr.Facebook.Models.facebook.FacebookViewData>" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
<fb:redirect url="<%= Model.pageUrl %>" />
</asp:Content>