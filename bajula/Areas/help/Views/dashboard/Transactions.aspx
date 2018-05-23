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
                    Transactions</h1>
                    <div class="legend">
                <% Html.RenderPartial("~/Areas/dashboard/Views/transactions/legend.ascx"); %>
                </div>
                <p></p>
                <h2>
                    Purchase Orders</h2>
                <p>
                    Purchase Orders are used to record purchases from suppliers. Creating a purchase will increment on <strong>On Order</strong> counter for all products included in the order. 
                    Once a Purchase Order is marked as received, the <strong>Available</strong> counter will be incremented for the relevant products.</p>
                    <h2>Sales Invoices</h2>
                    <p>Invoices tracks products sold to customers. Sales invoices are also created when a customer checks out from your online shop. 
When a sales invoice is created, the number of ordered items is deducted from the number of available of products and the number of selling products is increased. </p>
<p>
Once the invoice has been paid, the number of sold products is incremented.</p>
                <div class="mt50 ar">
                    Next: <a href="/help/dashboard/settings">Store Settings</a>
                </div>
            </div>
            <div id="help_content_bottom" class="ml200">
            </div>
            <div class="clear"></div>
        </div>
    </div>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="head" runat="server">
Help - Transactions
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="AdditionalJS" runat="server">
</asp:Content>
