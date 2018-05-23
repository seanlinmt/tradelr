<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<tradelr.Models.subdomain.Statistics>" %>
<%@ Import Namespace="tradelr.Libraries.Extensions" %>
<div id="summary_statistics">
<table id="store_statistics">
<tr><td>
<div class="bold larger"><%= Model.salesThisMonth %></div>
<div>THIS MONTH</div>
<div><%= Model.numberOfSalesThisMonth %> invoices</div>
</td><td>
<div class="bold larger"><%= Model.salesThisYear %></div>
<div>THIS YEAR</div>
<div><%= Model.numberOfSalesThisYear %> invoices</div>
</td><td>
<div class="bold larger"><%= Model.productTotal %></div>
<div>TOTAL PRODUCTS</div>
</td>
<td>
<div class="bold larger"><%= Model.outOfStockTotal %></div>
<div>OUT OF STOCK</div>
</td>
</tr>
<tr><td>
<div class="bold larger"><%= Model.salesUnpaidThisMonth %></div>
<div>UNPAID THIS MONTH</div>
<div><%= Model.numberofSalesUnpaidThisMonth %> invoices</div>
</td><td>
<div class="bold larger"><%= Model.salesUnpaidThisYear %></div>
<div>UNPAID THIS YEAR</div>
<div><%= Model.numberofSalesUnpaidThisYear %> invoices</div>
</td><td colspan="2">
</td></tr>
</table>
<div class="ar smaller">
<a href='#' id="time_1month" class="pr10">1 month</a><a href='#' id="time_2month" class="pr10">2 month</a><a href='#' id="time_4month" class="pr10">4 month</a>
</div>
<div id="statistics_time">
<%
    Html.RenderPartial("statisticsTime", Model);%>
</div>
</div>
<script type="text/javascript">
    $('#time_1month').live('click', function () {
        $.post('/dashboard/dashboard/statisticsTime', { month: -1 }, function (result) {
            if (result == '') {
                $.jGrowl('Bad Request');
            }
            $('#statistics_time').html(result);
        });
        return false;
    });
    $('#time_2month').live('click', function () {
        $.post('/dashboard/dashboard/statisticsTime', { month: -2 }, function (result) {
            if (result == '') {
                $.jGrowl('Bad Request');
            }
            $('#statistics_time').html(result);
        });
        return false;
    });
    $('#time_4month').live('click', function () {
        $.post('/dashboard/dashboard/statisticsTime', { month: -4 }, function (result) {
            if (result == '') {
                $.jGrowl('Bad Request');
            }
            $('#statistics_time').html(result);
        });
        return false;
    });
</script>