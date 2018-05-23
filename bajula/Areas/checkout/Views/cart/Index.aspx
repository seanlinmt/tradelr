<%@ Page Title="" Language="C#" MasterPageFile="~/Areas/checkout/Views/Shared/Checkout.master"
    Inherits="System.Web.Mvc.ViewPage<tradelr.Areas.checkout.Models.CheckoutViewModel>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div id="cart_products">
        <div class="cart_header">
            Your cart
        </div>
        <table class="cart_table">
            <% foreach (var item in Model.CartItems)
               {%>
               <tr>
            <td class="at">
                <img src="<%= item.thumbnailUrl %>" alt="<%= item.description %>" />
            </td>
            <td>
                <strong class="larger">
                    <%= item.description %></strong>
                <div class="ar">
                    <%= item.quantity %>
                    x
                    <%= Model.currency.symbol %><%= item.UnitPrice.ToString("n" + Model.currency.decimalCount)%>
                </div>
            </td>
            </tr>
            <%} %>
        </table>
        <div class="cart_summary">
            <div class="fr ar">
                <div>
                    <%= Model.sub_total.ToString("n" + Model.currency.decimalCount)%></div>
                <div>
                    <%= Model.discount_amount.ToString("n" + Model.currency.decimalCount)%></div>
                <div class="cart_total">
                    <%= Model.currency.symbol %><%= Model.total.ToString("n" + Model.currency.decimalCount)%></div>
            </div>
            <div class="fr w100px al">
                <div>
                    SubTotal</div>
                <div>
                    Discount
                    <%= !string.IsNullOrEmpty(Model.coupon)?string.Format("<div class='smaller'>coupon: {0}</div>", Model.coupon):"" %></div>
                <div class="cart_total">
                    Total</div>
            </div>
            <div class="clear">
            </div>
        </div>
    </div>
    <div id="content">
        <% if (Model.isLoggedIn)
           {
               if (Model.isDigitalOrder)
               {
                   Html.RenderAction("final_step", "order");
               }
               else
               {
                   Html.RenderAction("address_step", "order");
               }
           }
           else
           {
               Html.RenderPartial("login_step");
           } %>
        <div class="ml10 pad10">
              <img src="/Content/img/icons/arrow_left.png" alt="" /> <a href="<%= Model.store_url %>">return to store</a>
        </div>
    </div>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="HeadPlaceHolder" runat="server">
</asp:Content>
