<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<IEnumerable<tradelr.Areas.dashboard.Models.product.ProductVariantTransaction>>" %>
<% foreach (var variant in Model){%>
<h4><strong><%= variant.sku %></strong> <%= variant.variant_name %></h4>
<table class="normal fl w50p pr10">
 <thead>
 <tr>
 <td>Contacts Bought From</td>
 <td class="w150px ar">Unit Price</td>
 </tr>
 </thead>
 <tbody>
 <% foreach (var contact in variant.products_bought)
    {%>
  <tr><td><a href="<%= contact.contactUrl %>"><%= contact.contactName %></a></td><td class="w150px ar"><%= contact.unitPrice %></td></tr>  
<%} %>
 </tbody>
 </table>
 <table class="normal fl w50p">
 <thead>
 <tr>
 <td>Contacts Sold To</td>
 <td class="w150px ar">Unit Price</td>
 </tr>
 </thead>
 <tbody>
 <% foreach (var contact in variant.products_sold)
    {%>
  <tr><td><a href="<%= contact.contactUrl %>"><%= contact.contactName %></a></td><td class="w150px ar"><%= contact.unitPrice %></td></tr>  
<%} %>
 </tbody>
 </table> 
<% } %>
