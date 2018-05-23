<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<IEnumerable<tradelr.Models.products.ProductBase>>" %>
<%
        foreach (var product in Model)
        {
    %>
    <div alt="<%= product.id %>" class="product pad5 bt">
    <table>
    <tbody>
    <tr><td class="bold"><%= product.title %></td><td class="ar pr10"><%= product.sellingPrice %></td></tr>
    <tr><td><div class="fl w50px"><%= product.thumbnailUrl %></div><div class="fl"><ul class="variant_jqgrid">
                    <% foreach (var variant in product.variants)
                       {%>
                    <li alt="<%= variant.id %>">
                        <div class="sku smaller">
                            <%= variant.sku %></div>
                        <div>
                            <span class="bold mr5">
                                <%= variant.instock == null ? "∞": variant.instock.Value.ToString() %></span>
                                <span title="on order"><%= variant.onorder %></span></div>
                        <span class="attr">
                            <%= variant.color %></span><span class="attr"><%= variant.size %></span></li>
                    <%} %>
                </ul></div> 
                </td><td class="ar pr10"><input type="text" id="groupPrice" class="ar" /></td></tr>
    </tbody>
    </table>
    </div>
    <%  } %>
