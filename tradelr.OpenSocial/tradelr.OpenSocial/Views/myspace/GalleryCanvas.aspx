<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Myspace.Master" Inherits="System.Web.Mvc.ViewPage<tradelr.OpenSocial.Models.OpenSocialViewData>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
<style type="text/css">
.galleryBox{
    float:left;
    position:relative;
    margin:2px;
    background-color:#fff;
    width:154px;
    border:solid 1px #f2f2f2;
    padding:3px;
    overflow: hidden;
    height:235px;
}

.galleryBox:hover .title a{
    color:#009DFF;
}

.galleryBox .productThumbnail{
    height:150px;
    width:150px;
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

.galleryBox .title{
    font-size:11px;
    padding:5px;
    text-align:center;
    line-height:12px;
}

.galleryBox .title a{
    text-decoration:none;
    color:#444;
}

.galleryBox .title a:hover{
    text-decoration:underline;
}

.galleryBox .currency{
    color:#aaa;
}

.galleryBox .priceDetails {
    position: absolute;
    bottom:10px;
    width:154px;
}

.galleryBox .details {
    padding-top:4px;
    float:right;
}

.galleryBox .price{
    font-size:12px;
    font-weight:bold;
    color:#78C042;
    padding-right:5px;
}

</style>
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
    <a href="javascript:GetThis('<%= item.title %>','<%= item.summary %>', '<%= item.url %>')">
    <img class="sharelink" src="http://os.tradelr.com/Content/img/myspace.png" alt="" /></a>
    <div class="details">
        <span class="currency">
            <%= item.currencySymbol %></span>
        <span class="price">
            <%= item.sellingPrice %></span>
            </div>
    </div>
</div>       
<%   } %>
</asp:Content>
