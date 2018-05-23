<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Myspace.Master" Inherits="System.Web.Mvc.ViewPage<tradelr.OpenSocial.Models.OpenSocialViewData>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
<style type="text/css">
.galleryBox{
    float:left;
    position:relative;
    margin:2px;
    background-color:#fff;
    width:135px;
    padding:3px;
    overflow: hidden;
    height:135px;
}

.galleryBox .productThumbnail{
    position:relative;
}

.galleryBox .productThumbnail img{
    top:0;
    bottom:0;
    left:0;
    right:0;
    margin:auto;
    position:absolute;
}

</style>
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
<div class="mt10"><a href="#" onclick="navigate('CANVAS');">View more products</a></div>
</asp:Content>
