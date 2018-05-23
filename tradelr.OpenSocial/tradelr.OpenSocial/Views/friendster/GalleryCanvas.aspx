<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Friendster.Master" Inherits="System.Web.Mvc.ViewPage<tradelr.OpenSocial.Models.OpenSocialViewData>" %>
<%@ Import Namespace="tradelr.Common" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
<p>Showing latest 20 products. <a target="_blank" href="<%= Model.gallery.viewAllUrl %>">View all products</a></p>
<% foreach (var item in Model.gallery.products)
   {%>
<div class="galleryBox">
    <div class="productThumbnail">
        <a target="_blank" class="galleryThumbnail" href="<%= item.url %>">
        <img src="<%= item.thumbnailUrl %>" alt="" />
        </a>
    </div>
    <div class="title">
        <a target="_blank" href="<%= item.url %>">
            <%= item.title %></a></div>
    <div class="priceDetails">
    <a href="#" onclick="sendNotification(this);">
    <img class="sharelink" src="http://os.tradelr.com/Content/img/friendster.png" alt="" /></a>
    <div class="details">
        <span class="currency">
            <%= item.currencySymbol %></span>
        <span class="price">
            <%= item.sellingPrice %></span>
            </div>
    </div>
    <div class="summary hidden"><%= item.summary %></div>
</div>       
<%   } %>
</asp:Content>
