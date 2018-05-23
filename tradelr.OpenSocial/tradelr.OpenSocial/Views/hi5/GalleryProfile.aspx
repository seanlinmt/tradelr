<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/hi5.Master" Inherits="System.Web.Mvc.ViewPage<tradelr.OpenSocial.Models.OpenSocialViewData>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
<div class="clear"></div>
<div class="mt10">
<% foreach (var item in Model.gallery.products)
   {%>
<div class="galleryBox">
    <div class="productThumbnail">
        <a target="_blank" class="galleryThumbnail" href="<%= item.url %>">
        <img src="<%= item.thumbnailUrl %>" alt="" title="<%= item.title %>" />
        </a>
    </div>
</div>       
<%   } %>
</div>
<div class="clear"></div>
<div class="mt10"><a href="#" onclick="navigate('canvas');">View more products</a></div>
</asp:Content>
