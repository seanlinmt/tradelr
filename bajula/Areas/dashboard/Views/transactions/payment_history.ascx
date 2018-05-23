<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<IEnumerable<FilterBoxListInfo>>" %>
<%@ Import Namespace="tradelr.Libraries" %>
<%@ Import Namespace="tradelr.Libraries.Extensions" %>
<div id="payments_section">
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
    <div id="paymentsTimeLine">
    <h4><img src="/Content/img/headings/list_16.png" /> show</h4>
        <div class="sideboxEntry selected">
            All</div>
        <%= Html.FilterBoxList("timeline", Model)%>
    </div>
</div>
<div class="main_columnright">
    <div id="grid_content">
        <table id="paymentsGridView">
        </table>
        <div id="paymentsGridNavigation" class="scroll">
        </div>
    </div>
</div>
</div>
<script type="text/javascript">
    $(document).ready(function () {
        // bind side filter click event
        $('#paymentsTimeLine', '#payments_section').find('.sideboxEntry').bind("click", function () {
            payment_handle(this, '#paymentsTimeLine', '#payments_section');
        });

        $('#typesList', '#payments_section').find('.sideboxEntry').bind("click", function () {
            payment_handle(this, '#typesList', '#payments_section');
        });
    });

    function payment_handle(target, div, context) {
        $(div, context).find('.sideboxEntry').removeClass('selected');
        $(target).addClass('selected');
        reloadPaymentsGrid(context);
    };
</script>