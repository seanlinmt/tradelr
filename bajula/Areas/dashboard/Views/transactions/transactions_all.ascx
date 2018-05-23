<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<tradelr.Models.transactions.viewmodel.TransactionViewModel>" %>
<%@ Import Namespace="tradelr.Libraries.Extensions" %>
<%@ Import Namespace="tradelr.Models.users" %>
<div id="transactions_section">
    <div class="content_filter">
        <div id="typesList" class="filter">
            <h4><img src="/Content/img/document.png" />
                type</h4>
            <div fid="0" class="sideboxEntry selected">
                <strong>All</strong></div>
            <div fid="2" class="sideboxEntry">
                <div class="title">
                    invoices</div>
            </div>
            <div fid="1" class="sideboxEntry">
                <div class="title">
                    orders</div>
            </div>
        </div>
        <div id="statusList" class="filter">
        <h4><img src="/Content/img/product_status.png" />  status</h4>
        <div fid='' class="sideboxEntry selected">
                All</div>
            <%= Html.FilterBoxList("status", Model.statuses)%>
        </div>
        <div id="timeLine" class="filter">
        <h4><img src="/Content/img/headings/list_16.png" /> show</h4>
            <div fid='' class="sideboxEntry selected">
                All</div>
            <%= Html.FilterBoxList("timeLine", Model.timeline)%>
        </div>
    </div>
    <div class="main_columnright">
        <div id="grid_content">
            <div class="buttonRow">
                <% if (Model.permission.HasPermission(UserPermission.INVOICES_ADD))
                   {%>
                <button type="button" class="small green" title="create new invoice" onclick="javascript:window.location = '/dashboard/invoices/add';">
                    new invoice</button>
                <%
               }%>
                <% if (Model.permission.HasPermission(UserPermission.ORDERS_ADD))
                   {%>
                <button type="button" class="small green" title="create new order" onclick="javascript:window.location = '/dashboard/orders/add';">
                    new order</button>
                <%
               }%>
                <div id="search_area" class="fr">
    <span class="search"></span>
                    <input type="text" name="searchbox" id="searchInput" class="searchbox" />
    </div>
            </div>
            <table id="transactionsGridView">
            </table>
            <div id="transactionsGridNavigation" class="scroll">
            </div>
        </div>

        <div class="legend mt50">
    <div class="content">
        <h3>
            Transaction Status Legend</h3>
            <% Html.RenderPartial("legend"); %>
    </div>
</div>
    </div>
</div>
<script type="text/javascript">
    $(document).ready(function () {
        // bind side filter click event
        $('#timeLine', '#transactions_section').find('.sideboxEntry').live("click", function () {
            transaction_handle(this, '#timeLine', '#transactions_section');
        });

        // bind transaction types filter
        $('#typesList', '#transactions_section').find('.sideboxEntry').live("click", function () {
            transaction_handle(this, '#typesList', '#transactions_section');
        });

        // bind transaction types filter
        $('#statusList', '#transactions_section').find('.sideboxEntry').live("click", function () {
            transaction_handle(this, '#statusList', '#transactions_section');
        });

        $('#searchInput', '#transactions_section').keyup(function (e) {
            if (!isEnterKey(e)) {
                return;
            }
            reloadTransactionsGrid('#transactions_section');
        });
    });
    
    function transaction_handle(target, div, context) {
        $(div, context).find('.sideboxEntry').removeClass('selected');
        $(target).addClass('selected');
        reloadTransactionsGrid(context);
    };
</script>
