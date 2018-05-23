<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<tradelr.Areas.dashboard.Models.shipping.EbayShippingRule>" %>
<tr alt="<%= Model.id %>">
    <td>
        <div><%= Model.name %></div>
        <span class="smaller"><%= Model.shipToLocations %></span>
    </td>
    <td class="ar">
        <%= Model.cost %>
    </td>
    <td class="w60px ac">
        <span class="hover_del"></span>
    </td>
</tr>