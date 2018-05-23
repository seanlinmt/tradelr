<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<IEnumerable<tradelr.Areas.dashboard.Models.account.payment.PaymentMethodViewModel>>" %>
<%@ Import Namespace="tradelr.Library" %>
<% foreach (var method in Model)
       {%>
           <li alt="<%= method.id %>" class="relative">
           <strong><%= method.name %></strong> 
           <div class="absolute right10 top"><a href="#" class="hover_edit"></a><a href="#" class="hover_del"></a></div>
           <div><%= method.identifier %></div>
           <div class="smaller"><%= method.instructions.ToHtmlBreak() %></div>
           </li>
      <% } %>
