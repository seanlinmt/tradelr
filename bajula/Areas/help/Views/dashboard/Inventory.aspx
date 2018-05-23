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
                    Inventory</h1>
                    <img class="img_border" src="/Content/img/help/dashboard/inventory.jpg" alt="inventory" />
                <p>The following 4 counters are used to track the inventory level of a product. If inventory tracking is disabled, the number of stock available will always be infinite.</p>
                <table class="table_blue w500px">
                <thead><tr><td>Counter</td><td>Description</td></tr></thead>
                <tbody>
                <tr><td>Available</td><td>Number currently in hand</td></tr>
                <tr><td>On Order</td><td>Number being ordered</td></tr>
                <tr><td>Selling</td><td>Number being sold</td></tr>
                <tr><td>Sold</td><td>Number sold</td></tr>
                </tbody>
                </table>
                <h2>Product Transactions History</h2>
                <p>Clicking on the total number of items available for a product will open up a new tab to display a list of contacts
                that has bought or sold the product.</p>
                <img class="img_border" src="/Content/img/help/dashboard/inventory_transactions.jpg" alt="inventory transactions" />
                <p></p>
                <div class="mt50 ar">
                    Next: <a href="/help/dashboard/transactions">Transactions</a>
                </div>
            </div>
            <div id="help_content_bottom" class="ml200">
            </div>
            <div class="clear"></div>
        </div>
    </div>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="head" runat="server">
Help - Inventory
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="AdditionalJS" runat="server">
</asp:Content>
