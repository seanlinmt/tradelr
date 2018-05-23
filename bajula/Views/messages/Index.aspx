<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Message>" %>
<%@ Import Namespace="tradelr.Models.message"%>
<%@ Import Namespace="tradelr.Libraries.Extensions"%>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
Message
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
<div class="content_area">
<div id="messageTabs" style="position:relative">
<div class="buttonRow">
  <button id="messageCompose" type="button">Compose</button></div>
    <ul>
    	<li><a href="#inbox">Inbox</a></li>
    	<li><a href="#sent">Sent</a></li>
    </ul>
    <div id="inbox"></div>
    <div id="sent"></div>
  </div>
</div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="AdditionalJS" runat="server">
    <%= Html.RegisterViewJS() %>
</asp:Content>
