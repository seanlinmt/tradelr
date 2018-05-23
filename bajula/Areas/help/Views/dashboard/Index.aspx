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
                    Dashboard</h1>
                    <img class="img_border" src="/Content/img/help/dashboard/dashboard.jpg" alt="dashboard screenshot" />
                    <p>Your dashboard provides a common interface for you, your customers and staff to view various status and edit settings on your site. Permissions are used to limit access to various sections.</p>
                    <p>As a seller, the dashboard allows you to</p>
                    <ol>
                        <li>Manage products, contacts and transactions</li>
                        <li>View account and online storefront statistics</li>
                        <li>Add payment, shipping and discount options</li>
                        <li>Monitor social media accounts for leads</li>
                        <li>Customise your online storefront</li>
                    </ol>
                    <div class="mt50 ar">
                Next: <a href="/help/dashboard/inventory">Inventory</a>
                </div>
            </div>
            <div id="help_content_bottom" class="ml200">
            </div>
            <div class="clear"></div>
        </div>
    </div>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="head" runat="server">
Help - Dashboard
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="AdditionalJS" runat="server">
</asp:Content>
