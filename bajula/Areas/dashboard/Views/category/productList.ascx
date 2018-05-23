<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<IEnumerable<ProductBase>>" %>
<%@ Import Namespace="tradelr.Models.products" %>
<div class="fixedHeight scroll_y">
    <%
        foreach (var product in Model)
        {
    %>
    <div class="blockSelectable">
        <div class="fl w50px ac">
            <%= product.thumbnailUrl %>
        </div>
        <div class="content">
            <%= product.title%>
        </div>
        <span class="hidden">
            <%= product.id%></span>
        <div class="clear">
        </div>
    </div>
    <%  } %>
</div>
<div class="clear">
</div>
