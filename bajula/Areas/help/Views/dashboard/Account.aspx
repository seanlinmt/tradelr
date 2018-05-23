<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/NotLoggedIn.Master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

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
                    Account Settings</h1>
                <h2>
                    Profile</h2>
                    <p>Organizational information and logo are used in invoices and purchase orders;</p>
                    <p></p>
                    <h2>
                    Payment &amp; Checkout</h2>
                    <p>Currently, payment from your clients and customers can be accepted via Paypal.</p>
                    <p>Custom payment methods like bank deposit, cash on delivery or cheque can be specified manually here as well.
                    For each method, you can choose to include payment instructions which will be displayed to a customer.
                    </p>
                    <h2>
                    Custom Domain Names</h2>
                    <img class="img_border mt10" src="/Content/img/help/dashboard/custom_domain.jpg" alt="creating discount coupons" />
                    <p>You can continue to maintain your branding while using tradelr.</p>
                <p>
                    If you currently have registered a domain name you would like to use for your online
                    store, you can do so by entering the registered domain name in the <strong>Custom Domain
                        Name</strong> box.</p>
                <p>
                    You will need to ask your domain name registrar to create an <strong>A record</strong>
                    to point your domain name to the address <strong>98.126.29.28</strong></p>
                <p>Alternatively, you can purchase your own domain using tradelr and we will automatically setup your account to use your new domain name.</p>
                <div class="mt50 ar">
                    Next: <a href="/help/storefront">Storefront</a>
                </div>
            </div>
            <div id="help_content_bottom" class="ml200">
            </div>
            <div class="clear"></div>
        </div>
    </div>

</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="head" runat="server">
    Help - Account Settings
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="AdditionalJS" runat="server">
</asp:Content>
