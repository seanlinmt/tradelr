<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<InventoryLocation>" %>
<%@ Import Namespace="tradelr.Models.inventory" %>
<span class="name">
    <%= Model.title %></span>
    <% foreach (var item in Model.locationItems) {%>
  <div class="content_row">
    <%if (Model.createmode)
      { %>
      <div class="inventorySKU"><%= item.inventorySKU%></div>
      <div class="fl">
            <div class="form_label">
                <label for="inStock">
                    Inventory Level</label>
            </div>
            <input type="text" name="inStock" class="w150px" />
    </div>
    <div class="fl pl10">
            <div class="form_label">
                <label for="reorderLevel">
                    Stock Alarm Level</label>
            </div>
            <input type="text" name="reorderLevel" class="w150px" />
    </div>
    <div class="clear">
    </div>
    <%} else { %>
    <table class="w300px">
    <thead>
    <tr><td class="inventorySKU bb"><%= item.inventorySKU%></td><td class="bb ar">
    <span class="hover_edit" title='edit available inventory'></span>
 <span class="hover_alarm" title='set low inventory alarm level'></span>
 <span class="hover_history" title='show history'></span>
    </td></tr>
    </thead>
    <tbody>
    <tr><td class="w200px" title="number of items in stock">in stock</td><td class="instock ar"><%= item.inStock %></td></tr>
    <tr><td title="number of items in the process of being sold">selling</td><td class="ar"><%= item.reserved %></td></tr>
    <tr><td title="number of items sold">sold</td><td class="ar"><%= item.sold %></td></tr>
    <tr><td title="number of items on order">on order</td><td class="ar"><%= item.onOrder %></td></tr>
    <tr><td title="inventory alarm level">alarm level</td><td class="alarmlevel ar"><%= item.alarmLevel %></td></tr>
    </tbody>
    </table>
    <input type="hidden" name="inStock" />
    <input type="hidden" name="reorderLevel" />
    <%} %>
    <span class="itemid hidden"><%= item.id %></span>
</div>
<%} %>

<%= Html.Hidden("location", Model.id) %>