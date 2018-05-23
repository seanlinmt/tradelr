function reloadCouponsGrid(filterBy) {
    var url = '/dashboard/coupons/list?cat=' + filterBy;
    $("#couponsGridView").setGridParam({ url: url });
    $("#couponsGridView").trigger("reloadGrid");
}

function searchCoupons(searchTerm) {
    var url = '/dashboard/coupons/search?term=' + searchTerm;
    $("#couponsGridView").setGridParam({ url: url });
    $("#couponsGridView").trigger("reloadGrid");
}

// NOTE: number of columns must be the same as jqgrid_orders_received
function couponsBindToGrid() {
    $("#couponsGridView").jqGrid({
        afterInsertRow: function (id, data, element) {
            var code = ["<span class=\"jqview\" >", data['code'], "</span>"];
            // update row
            $("#couponsGridView").setRowData(id, { code: code.join('') });
        },
        altRows: true,
        autowidth: true,
        colNames: ['', 'Coupon Code','Discount', 'Description', 'Start Date', 'Expiry Date', 'Impressions', 'Status'],
        colModel: [
            { name: 'id', hidden: true },
            { name: 'code', index: 'code', width: 80, align: 'center' },
            { name: 'discount', index: 'discount', width: 70, align: 'left', sortable:false },
            { name: 'description', width: 325, align: 'left', sortable: false },
            { name: 'startDate', index: 'startDate', width: 70, align: 'left', sorttype: "date" },
            { name: 'expiryDate', index: 'expiryDate', width: 70, align: 'left', sorttype: "date" },
            { name: 'impressions', index: 'impressions', width: 62, align: 'center' },
            { name: 'status', index: 'status', width: 62, align: 'center', sortable:false }
        ],
        datatype: 'json',
        loadComplete: function () {
            // bind view button (uses orderid)
            $('.jqview', '#couponsGridView').click(function () {
                var id = $(this).parents('tr').find('td:first').text();
                dialogBox_open('/dashboard/coupons/edit/' + id, 600);
                return false;
            });

            // add checkbox in header
            init_jqgrids('#marketing_coupons');
        },
        height: '100%',
        hoverrows: false,
        imgpath: '/Content/images',
        mtype: 'POST',
        onPaging: function () {
            $.scrollTo("#container", 800);
        },
        onSortCol: function (index, colIndex, sortOrder) {
            $('#gview_couponsGridView')
                .find(".ui-jqgrid-htable tr > th")
                .removeClass('jqgrid_sorted');
            $('#gview_couponsGridView')
                .find(".ui-jqgrid-htable tr > th:eq(" + colIndex + ")")
                .addClass('jqgrid_sorted');
        },
        pager: 'couponsGridNavigation',
        rowNum: 10,
        rowList: [10, 50, 100, 1000],
        rownumbers: false,
        shrinkToFit: true,
        sortname: 'id',
        sortorder: 'desc',
        url: '/dashboard/coupons/list',
        viewrecords: true,
        viewsortcols: true
    }).navGrid('couponsGridNavigation', { refresh: false, search: false, edit: false, add: false, del: false });
}
