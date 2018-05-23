<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<tradelr.Areas.dashboard.Models.product.ContactTransactionsViewModel>" %>
<table id="contact_products_sold" class="normal">
    <thead>
        <tr>
            <td>
                Products Sold To
            </td>
            <td class="w150px ar">
                Unit Price
            </td>
        </tr>
    </thead>
    <tbody>
        <% foreach (var sold in Model.products_sold)
           { %>
        <tr>
            <td>
                <a href="<%= sold.productediturl %>" class="bold">
                    <%= sold.productName %></a>
            </td>
            <td class="w150px ar at">
                <%= sold.unitPrice %>
            </td>
        </tr>
        <% } %>
    </tbody>
</table>
<table id="contact_products_bought" class="normal">
    <thead>
        <tr>
            <td>
                Products Bought From
            </td>
            <td class="w150px ar">
                Unit Price
            </td>
        </tr>
    </thead>
    <tbody>
        <% foreach (var bought in Model.products_bought)
           { %>
        <tr>
            <td>
                <a href="<%= bought.productediturl %>" class="bold">
                    <%= bought.productName %></a>
            </td>
            <td class="w150px ar at">
                <%= bought.unitPrice %>
            </td>
        </tr>
        <% } %>
    </tbody>
</table>
<script type="text/javascript">
    $(document).ready(function () {
        $('thead', '#contact_products_bought,#contact_products_sold').hover(function () {
            $(this).addClass('hover');
        }, function () {
            $(this).removeClass('hover');
        });
        $('thead', '#contact_products_bought,#contact_products_sold').click(function () {
            $(this).next().toggle();
        });
    });
</script>
