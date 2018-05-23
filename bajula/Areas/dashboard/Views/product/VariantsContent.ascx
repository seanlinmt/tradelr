<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<IEnumerable<tradelr.Models.products.ProductBase>>" %>
<%
        foreach (var product in Model)
        {
    %>
    <div alt="<%= product.id %>" class="product bb">
        <div class="title bold">
            <%= product.title %></div>
            <div>
            <span class="price w100px"><%= product.sellingPrice %>&nbsp;</span>
        <span class="tax">
            <%= product.tax %></span>
            </div>
        
        <div>
            <div class="fl w50px ac mr10">
                <%= product.thumbnailUrl %>
            </div>
            <div class="content">
                <ul class="variant_jqgrid">
                    <% foreach (var variant in product.variants)
                       {%>
                    <li class="blockClickable" alt="<%= variant.id %>">
                        <div class="sku smaller">
                            <%= variant.sku %>&nbsp;</div>
                        <div>
                            <span class="bold mr5">
                                <%= variant.instock == null ? "∞": variant.instock.Value.ToString() %></span>
                                <span title="on order"><%= variant.onorder %></span></div>
                        <span class="attr">
                            <%= variant.color %></span><span class="attr"><%= variant.size %></span></li>
                    <%} %>
                </ul>
            </div>
            <div class="clear">
            </div>
        </div>
    </div>
    <%  } %>
