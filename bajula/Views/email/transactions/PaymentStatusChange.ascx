<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>
<p>The payment for <%= ViewData["type"] %> #<%= ViewData["orderNumber"]%> has been <%= ViewData["status"]%>.</p>
<p>To view the transaction, follow the link below:</p>
<p><%= ViewData["viewloc"]%></p>