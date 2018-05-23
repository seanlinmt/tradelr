<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<tradelr.Models.transactions.viewmodel.TransactionViewModel>" %>
<%@ Import Namespace="tradelr.Libraries.Extensions" %>
<%@ Import Namespace="tradelr.Models.users" %>
<div class="content_filter">
    <div id="timeLine" class="filter">
        <h4>
            show</h4>
        <div class="sideboxEntry selected">
            All</div>
        <%= Html.FilterBoxList("timeLine", Model.timeline)%>
    </div>
</div>
<div class="main_columnright">
    <div id="grid_content">
        <div class="buttonRow">
            <% if (Model.permission.HasPermission(UserPermission.ORDERS_ADD))
               {%>
            <button id="newInvoice" type="button" class="small green" title="create new order"
                onclick="javascript:window.location = '/dashboard/orders/add';">
                new order</button>
            <%
                }%>
            <div class="search">
                <input type='text' id='searchInput' name='searchInput' />
            </div>
        </div>
        <table id="ordersGridView" class="scroll">
        </table>
        <div id="ordersGridNavigation" class="scroll" style="text-align: center;">
        </div>
    </div>
</div>
<span id="filterBy" class="hidden"></span>
