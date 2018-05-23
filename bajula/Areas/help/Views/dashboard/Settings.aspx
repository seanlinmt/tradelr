<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/NotLoggedIn.Master"
    Inherits="System.Web.Mvc.ViewPage" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div id="banner">
        <div class="content">
            <h1>
                Help</h1>
        </div>
    </div>
    <div class="banner_main">
        <div class="content pt20">
            <div id="help_nav" class="fl">
                <% Html.RenderPartial("~/Areas/help/Views/help_navigation.ascx"); %>
            </div>
            <div id="help_content_top" class="ml200">
            </div>
            <div id="help_content" class="ml200">
                <h1>
                    Store Settings</h1>
                <img class="img_border" src="/Content/img/help/dashboard/store_settings.jpg" alt="store settings" />
                <h2>
                    Store Visibility</h2>
                <p>
                    Your online store is accessible by anyone if it is made public. A private store
                    will only be accessible to those with a login and password.</p>
                
                <h2>
                    Facebook Store</h2>
                <p>
                    You can add a store to your facebook page using our <a target="_blank" href="http://apps.facebook.com/tradelr">
                        facebook app</a>. With the facebook app, you can choose to specify a discount
                    coupon to be revealed when a facebook user likes your page. This provides an incentive
                    for people to help promote your facebook page. You will need to first create a coupon
                    under the Store Marketing page.
                </p>
                <h2>
                    Store Policies</h2>
                <p>
                    This page allows you to enter default payment policies for invoices and refund policies
                    for your online shop.</p>
                <p>
                    Information entered here will be respectively accessible by the liquid variables
                    <span class="code_span">shop.payment_policy</span> and <span class="code_span">shop.refund_policy</span>.</p>
                <h2>
                    Store Navigation</h2>
                <p>
                    From this tab, you can configure the navigation links for your store. A linklist
                    basically is a group of links that will be accessible by your liquid templates.</p>
                <h2>
                    Pages</h2>
                <p>
                    You can create as many pages as you like to display information like your contact
                    form, company information or store policies page.</p>
                <h2>
                    Blogs</h2>
                <p>
                    With Tradelr, a blog holds a collection of articles. More than one blog can be created.</p>
                <p>
                    An article is a blog post that is published by the shop owner or any administrator
                    appointed by the shop owner. You can attach pictures, files, video clips or files
                    in each article.</p>
                <p>
                    Commenting for each blog can be either disabled, moderated or unmoderated. A moderated
                    blog means that all user comments needs your approval first before it is shown.</p>
                <div class="mt50 ar">
                    Next: <a href="/help/dashboard/marketing">Marketing - Coupons</a>
                </div>
            </div>
            <div id="help_content_bottom" class="ml200">
            </div>
            <div class="clear"></div>
        </div>
    </div>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="head" runat="server">
    Help - Store Settings
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="AdditionalJS" runat="server">
</asp:Content>
