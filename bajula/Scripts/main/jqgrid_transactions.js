function reloadTransactionsGrid(context) {
    var type = $('#typesList .selected', context).attr('fid');
    var time = $('#timeLine .selected', context).attr('fid');
    var status = $('#statusList .selected', context).attr('fid');
    var term = $('#searchInput', context).val();
    if (term.indexOf('search') != -1) {
        term = '';
    }
    
    var url = '/dashboard/transactions/list?type=' + type + '&interval=' + time + '&term=' + term + '&status=' + status;
    $("#transactionsGridView").setGridParam({ url: url });
    $("#transactionsGridView").trigger("reloadGrid");
}

function transactionsBindToGrid() {
    $("#transactionsGridView").jqGrid({
        altRows: true,
        autowidth: false,
        colNames: ['Invoice/Order #', 'Details', 'Total', 'Status'],
        colModel: [
            { name: 'orderNumber', index: 'orderNumber', width: 130, align: 'left' },
            { name: 'details', width: 452, align: 'left', sortable: false },
            { name: 'total', width: 130, align: 'right' },
            { name: 'status', index: 'status', width: 100, align: 'center' }
          ],
        datatype: 'json',
        loadComplete: function () {
            init_jqgrids('#transactions_all');
        },
        height: '100%',
        hoverrows: false,
        imgpath: '/Content/images',
        mtype: 'POST',
        onPaging: function () {
            $.scrollTo("#container", 800);
        },
        onSortCol: function (index, colIndex, sortOrder) {
            $('#gview_transactionsGridView')
                .find(".ui-jqgrid-htable tr > th")
                .removeClass('jqgrid_sorted');
            $('#gview_transactionsGridView')
                .find(".ui-jqgrid-htable tr > th:eq(" + colIndex + ")")
                .addClass('jqgrid_sorted');
        },
        pager: 'transactionsGridNavigation',
        rowNum: 10,
        rowList: [10, 50, 100, 1000],
        rownumbers: false,
        shrinkToFit: true,
        sortname: 'id',
        sortorder: 'desc',
        url: '/dashboard/transactions/list',
        viewrecords: true,
        viewsortcols: true
    }).navGrid('transactionsGridNavigation', { refresh: false, search: false, edit: false, add: false, del: false });
}
