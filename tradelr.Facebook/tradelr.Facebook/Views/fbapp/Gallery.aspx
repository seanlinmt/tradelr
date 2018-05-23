<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Facebook.Master" Inherits="System.Web.Mvc.ViewPage<tradelr.Facebook.Models.facebook.FacebookViewData>" %>
<%@ Import Namespace="tradelr.Common.Constants" %>
<%@ Import Namespace="tradelr.Library.Constants" %>

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

#header-coupon {
    background-color:#EEEEEE;
    border:1px dashed #CCCCCC;
    margin-top:10px;
    padding:20px;
    color:#3B5998;
}

</style>
<div id="header-coupon" <%= String.IsNullOrEmpty(Model.couponMessage)?"class='hidden'":"" %>>
<%= Model.couponMessage %>
</div>
<div class="header-ad">
<h3><a href="<%= GeneralConstants.FACEBOOK_APP_URL %>">Want your store on Facebook?</a></h3>
</div>
<p class="fl">Showing newest 21 products. <a href="<%= Model.gallery.viewAllUrl %>">View all products</a></p>
<div class="clear"></div>
<% foreach (var item in Model.gallery.products)
   {%>
<div class="galleryBox">
    <div class="productThumbnail">
        <a class="galleryThumbnail" href="<%= item.url %>">
        <img src="<%= item.thumbnailUrl %>" alt="" />
        </a>
    </div>
    <div class="title">
        <a href="<%= item.url %>">
            <%= item.title %></a></div>
    <div class="priceDetails">
    <fb:share-button class="url" href="<%= item.url %>" />
    <% if (!string.IsNullOrEmpty(item.sellingPrice))
{%>
    <div class="details">
        <span class="currency">
            <%=item.currencySymbol%></span>
        <span class="price">
            <%=item.sellingPrice%></span>
            </div>
            <%
}%>
    </div>
</div>       
<%   } %>
<div class="clear"></div>
<div class="pt20">
<fb:comments xid="<%= HttpUtility.UrlEncode(Model.gallery.viewAllUrl) %>"></fb:comments>
</div>
</asp:Content>
