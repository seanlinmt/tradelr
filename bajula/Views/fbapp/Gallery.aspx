<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/facebook/FacebookTab.Master"
    Inherits="System.Web.Mvc.ViewPage<tradelr.Models.facebook.app.FacebookGalleryViewModel>" %>

<%@ Import Namespace="tradelr.Library.Constants" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div id="header-coupon" <%= String.IsNullOrEmpty(Model.couponMessage)?"class='hidden'":"" %>>
        <%= Model.couponMessage %>
    </div>
    <% if (Model.isOwner)
       { %>
       <div class="mt10 mb10">
    <a href="#" onclick="window.top.location.href = '<%= Model.viewAllUrl %>/login?token=<%= Model.token %>';return false;">go to dashboard</a>
    </div>
    <% }
       else
       { %>
    <div class="header-ad">
        <h3>
            <a href="#" onclick="window.top.location.href = '<%= GeneralConstants.FACEBOOK_APP_URL %>';return false;">Want your own store on Facebook?</a></h3>
    </div>
    <p class="fl">
        <a href="#" onclick="window.top.location.href = '<%= Model.canvasStoreUrl %>';return false;">view all products</a>
    </p>
    <%} %>
    <div class="clear">
    </div>
    <% foreach (var item in Model.products)
       {%>
    <div class="galleryBox">
        <div class="productThumbnail">
            <a class="galleryThumbnail" href="#" onclick="window.top.location.href = '<%=Model.canvasStoreUrl%>'; return false;">
                <img src="<%= item.thumbnailUrl %>" alt="" />
            </a>
        </div>
        <div class="title">
            <a href="#" onclick="window.top.location.href = '<%=Model.canvasStoreUrl%>'; return false;">
                <%= item.title %></a></div>
        <div class="priceDetails">
            <a name="fb_share" type="button" share_url="<%= item.url %>"></a>
            <% if (!string.IsNullOrEmpty(item.sellingPrice))
               {%>
            <div class="details">
                <span class="currency">
                    <%=item.currencySymbol%></span> <span class="price">
                        <%=item.sellingPrice%></span>
            </div>
            <%
}%>
        </div>
    </div>
    <%   } %>
    <div class="clear">
    </div>
    <% if (Model.products.Count() != 0)
       {%>
    <p>
        <a class="bottom_link" href="#" onclick="window.top.location.href = '<%=Model.canvasStoreUrl%>'; return false;">view all products</a></p>
    <% }
       else
       { %>
    <div id="tab_noproducts">
        <p>
            no products have been added yet</p>
        <% if (Model.isOwner)
           { %>
        <a href="#" onclick="window.top.location.href = '<%= Model.viewAllUrl %>/login?token=<%= Model.token %>';return false;">go to
            dashboard</a>
        <% } %>
    </div>
    <% } %>
    <script src="http://static.ak.fbcdn.net/connect.php/js/FB.Share" type="text/javascript"></script>
</asp:Content>
