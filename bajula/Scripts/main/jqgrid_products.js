function reloadProductGrid(filterBy) {
    $("#productGridView").setGridParam({ url: '/dashboard/product/list', postData: {
                                                cat: filterBy, 
                                                location: $('#locationFilter').html(),
                                                flag: $('#statusFilter').html(),
                                                collection: $('#collectionsFilter').html(),
                                                term: $('#searchInput', '#inventory_mine').val()
                                            } 
                                        });
    $("#productGridView").trigger("reloadGrid");
}

function formatStockTotals(rowid) {
    var instock = $("#" + rowid, "#productGridView").find('td:eq(5) > .bold');
    var alarmlevel = $("#" + rowid, "#productGridView").find('td:eq(6)');
    if (parseInt(instock.text(), 10) <= parseInt(alarmlevel.text(), 10)) {
        $(instock).css('color', 'red');
    }
    else {
        $(instock).css('color', 'black');
    }
    $(instock).addClass('number_inv');
}


// this has to match order of definition of extension methd
function productBindToGrid() {
    var url = '/dashboard/product/list';
    if (querySt("alarm") == 1) {
        url += '?alarm=1';
    }

    var selectRow = function (row) {
        row.toggleClass('selected-row');
    };

    $("#productGridView").jqGrid({
        afterInsertRow: function (id, data, element) {
            // update row
            $("#productGridView").setRowData(id, { sel: '<input class="ml4" type="checkbox"/>'});

            // can't do the following without returning stock alarm levels
            //formatStockTotals(id);
        },
        afterSubmitCell: function (serverresponse, rowid, cellname, value, iRow, iCol) {
            var json_data = $.evalJSON(serverresponse.responseText);
            var retval = [];
            if (json_data.success) {
                retval.push(true);
                retval.push("");
            }
            else {
                retval.push(false);
                retval.push(json_data.message);
            }
            return retval;
        },
        afterSaveCell: function (rowid, cellname, value, iRow, iCol) {
            formatStockTotals(rowid);
        },
        altRows: true,
        autowidth: false,
        cellEdit: false,
        cellSubmit: 'remote',
        cellurl: '/dashboard/product/update',
        colNames: ['', '', '', 'Product', 'Inventory', 'Total', 'Views'],
        colModel: [
                    { name: 'pid', hidden: true },
                    { name: 'sel', width: 20, sortable: false, editable: false },
                    { name: 'thumbnail', classes: 'jqgrid_thumb', index: 'thumbnail', width: 50, align: 'center', sortable: false },
                    { name: 'title', index: 'title', width: 248, align: 'left', sortable: true },
                    { name: 'variants', width: 354, align: 'left', sortable: false },
                    { name: 'invtotal', width: 70, align: 'center', editable: true, sortable: false },
                    { name: 'hits', index: 'hits', width: 70, align: 'center', sortable: true }
                  ],
        datatype: 'json',
        height: '100%',
        imgpath: '/Content/images',
        loadComplete: function () {
            // add checkbox in header
            $('#gview_productGridView')
                .find(".ui-jqgrid-htable tr > th:eq(1)")
                .html('<input id="cbAll" type="checkbox"/>');

            $('#cbAll', '#gview_productGridView').bind('click', function () {
                if ($(this).is(':checked')) {
                    $("#productGridView input:checkbox").attr('checked', true);
                }
                else {
                    $("#productGridView input:checkbox").attr('checked', false);
                }
                $.each($("#productGridView input:checkbox"), function () {
                    var row = $(this).parents('tr');
                    selectRow(row);
                });
            });

            // bind individual checkboxes
            $("#productGridView input:checkbox").bind('click', function () {
                var row = $(this).parents('tr');
                selectRow(row);
            });

            init_jqgrids('#inventory_mine');
        },
        mtype: 'POST',
        onPaging: function () {
            $.scrollTo("#container", 800);
        },
        onSelectRow: function (id) {
            var row = $('#' + id);
            var cb = row.find('input:checkbox');
            if (cb.is(':checked')) {
                cb.attr('checked', false);
            }
            else {
                cb.attr('checked', true);
            }
            selectRow(row);
        },
        onSortCol: function (index, colIndex, sortOrder) {
            $('#gview_productGridView')
                .find(".ui-jqgrid-htable tr > th")
                .removeClass('jqgrid_sorted');
            $('#gview_productGridView')
                .find(".ui-jqgrid-htable tr > th:eq(" + colIndex + ")")
                .addClass('jqgrid_sorted');
        },
        pager: 'productGridNavigation',
        rowNum: 10,
        rowList: [10, 50, 100, 1000],
        rownumbers: false,
        shrinkToFit: true,
        sortname: 'id',
        sortclass: 'jqgrid_sorted',
        sortorder: 'desc',
        url: url,
        viewrecords: true,
        viewsortcols: true
    }).navGrid('productGridNavigation', { refresh: false, search: false, edit: false, add: false, del: false });
}
