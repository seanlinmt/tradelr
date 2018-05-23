<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<tradelr.Areas.dashboard.Models.product.ProductTransactionsViewModel>" %>
<div id="search_area" class="fl">
    <span class="search"></span>
    <input type="text" name="searchbox" id="searchInput" class="searchbox" />
</div>
<div class="clear mb10"></div>
<div id="product_transactions_results">
<% Html.RenderAction("TransactionsContent", new{ id = Model.productid}); %>
</div>
<script type="text/javascript">
    var searchtimer;
    var searchterm = "";
    // login name availability check
    $('#searchInput', '#tab_product_<%= Model.productid %>').keyup(function () {
        searchterm = $(this).val();

        if (searchtimer !== undefined) {
            clearTimeout(searchtimer);
        }
        searchtimer = setTimeout(function () {
            $.post('/dashboard/product/transactionscontent/<%= Model.productid %>', { term: searchterm }, function (result) {
                $('#product_transactions_results', '#tab_product_<%= Model.productid %>').html(result);
            });
        }, 500);
    });
</script>