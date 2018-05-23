<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<OrderShippedEmailContent>" %>
<%@ Import Namespace="tradelr.Models.transactions"%>
<p>Your order <a href="<%= Model.viewloc %>">#<%= Model.orderNumber %></a> has been shipped to the following address:</p>
<p><%= Model.shippingAddress %></p>
<p></p>
<p>When your order reaches you, don't forget to <a href="<%= Model.viewloc %>">mark the order as received</a>. 
This will update your product inventory as well.</p>
<p></p>
<p>Best regards,<br />
<%= Model.sender %>
</p>