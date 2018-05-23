<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<System.Collections.Generic.IEnumerable<tradelr.Models.inventory.History>>" %>
<h4 class="icon_history">Inventory Change History</h4>
<table id="inventory_history_table">
<thead>
<tr><td class="al">Date</td><td class="al">Description</td><td>In Stock</td><td>Selling</td><td>On Order</td><td>Sold</td></tr>
</thead>
<tbody>
<% foreach (var history in Model){%>
  <tr><td class="al"><%= history.created %></td><td class="al"><%= history.description %></td><td><%= history.instock %></td><td><%= history.reserved %></td><td><%= history.onOrder %></td><td><%= history.sold %></td></tr>
<%} %>
</tbody>
</table>
