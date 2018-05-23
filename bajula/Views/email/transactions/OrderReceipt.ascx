<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<tradelr.Areas.checkout.Models.emails.OrderReceipt>" %>
<p>
    Thank you for placing your order with <%= Model.shopname %>.</p>
<p>
    <%= Model.message %></p>
<p>
    This email is to confirm the following order.</p>
<p>
    &nbsp;</p>
<p>
    Date
    <%= Model.date %></p>
<p>
    Shipping Address</p>
<%= Model.shippingAddress %>
<p>
    Billing Address</p>
<%= Model.billingAddress %>
    <% foreach (var orderitem in Model.orderitems)
       {%>
    <p>
        <%= orderitem %>
    </p>
    <%   } %>
<table>
    <tr>
        <td>
            Subtotal:
        </td>
        <td style="text-align: right">
            <%= Model.subtotal %>
        </td>
    </tr>
    <tr>
        <td>
            Shipping:
        </td>
        <td style="text-align: right">
            <%= Model.shippingcost %>
        </td>
    </tr>
    <tr>
        <td>
            Discount:
        </td>
        <td style="text-align: right">
            <%= Model.discount %>
        </td>
    </tr>
    <tr>
        <td>
            Total:
        </td>
        <td style="text-align: right">
            <%= Model.totalcost %>
        </td>
    </tr>
</table>
<p>
    &nbsp;</p>
    <p>You can view the status of your order by going to <a href="<%= Model.viewloc %>"><%= Model.viewloc %></a></p>
