<%@ Page Title="" Language="C#" MasterPageFile="~/Areas/checkout/Views/Shared/Checkout.master" Inherits="System.Web.Mvc.ViewPage<tradelr.Areas.checkout.Models.OrderCompletedViewModel>" %>
<%@ Import Namespace="tradelr.Areas.dashboard.Models" %>
<%@ Import Namespace="tradelr.Library" %>
<%@ Import Namespace="tradelr.Models.payment" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
<div id="cart_products">
        <h2 class="cart_subheader">
            You have purchased the following</h2>
        <table id="cart_table">
            <tbody>
                <% foreach (var item in Model.cart_items)
                   {%>
                <tr>
                    <td>
                        <img src="<%= item.thumbnail_url %>" alt="<%= item.name %>" />
                    </td>
                    <td>
                        <h3>
                          <span class="smaller"><%= item.quantity %> x</span>  <%= item.name %></h3>
                    </td>
                </tr>
                <%} %>
            </tbody>
        </table>
    </div>
    <div id="checkout_completed">
        <h2 class="cart_subheader">Your Order Number is #<%= Model.order_number %></h2>
        <% if (Model.paymentType == PaymentMethodType.Custom) {%>
        <div id="payment_instructions">
        <h4><%= Model.paymentmethod.name %> instructions</h4>
               <div><%= Model.paymentmethod.instructions.ToHtmlBreak() %></div>
        </div>
           <%}%>
        <div>
        <div class="content">
        <p>You should have received an email receipt for your order. Please keep that for your record. You can also view and update your order details <a href="<%= Model.store_url %><%= Model.order_url %>">here</a>.</p>
        <p>Thank you for shopping at <a href="<%= Model.store_url %>"><%= Model.store_name %></a>.</p>
        </div>
        </div>
    </div>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="HeadPlaceHolder" runat="server">
</asp:Content>
