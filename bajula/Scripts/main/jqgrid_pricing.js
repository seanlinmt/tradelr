///// contacts grid
function reloadGroupPricingGrid(filterBy) {
    if (filterBy == '' || filterBy == undefined) {
        filterBy = 'All';
    }

    var url = '/dashboard/group/pricinglist?cat=' + filterBy;
    $("#groupPricingGridView").setGridParam({ url: url });
    $("#groupPricingGridView").trigger("reloadGrid");
}

function groupPricingBindToGrid() {
    var url = '/dashboard/group/pricinglist';

    var selectRow = function (row) {
        row.toggleClass('selected-row');
    };
    $("#groupPricingGridView").jqGrid({
        afterInsertRow: function (id, data, element) {
            // update row
            $("#groupPricingGridView").setRowData(id, { sel: '<input class="ml4" type="checkbox"/>', act: "<span class=\"jqedit\" >edit</span>"});
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
        },
        altRows: true,
        autowidth: true,
        cellEdit: false,
        cellSubmit: 'remote',
        cellurl: '/dashboard/group/pricingupdate',
        colNames: ['', '', '', 'Product', 'Inventory', 'Group Price'],
        colModel: [
                    { name: 'pid', hidden: true },
                    { name: 'sel', width: 20, sortable: false, editable: false },
                    { name: 'thumbnail', classes: 'jqgrid_thumb', index: 'thumbnail', width: 50, align: 'center', sortable: false },
                    { name: 'title', index: 'title', width: 260, align: 'left', sortable: true },
                    { name: 'variants', width: 383, align: 'left', sortable: false },
                    { name: 'groupprice', index: 'price', width: 100, align: 'right', sortable: true }
                  ],
        datatype: 'json',
        height: '100%',
        imgpath: '/Content/images',
        loadComplete: function () {
            // bind edit button
            $('.jqedit', '#groupPricingGridView').click(function () {
                var id = $(this).parents('tr').attr('id');
                alert('edit ' + id);
                return false;
            });

            // add checkbox in header
            $('#gview_groupPricingGridView')
                .find(".ui-jqgrid-htable tr > th:eq(1)")
                .html('<input id="cbAll" type="checkbox"/>');

            $('#cbAll', '#gview_groupPricingGridView').bind('click', function () {
                if ($(this).is(':checked')) {
                    $("#groupPricingGridView input:checkbox").attr('checked', true);
                }
                else {
                    $("#groupPricingGridView input:checkbox").attr('checked', false);
                }
                $.each($("#groupPricingGridView input:checkbox"), function () {
                    var row = $(this).parents('tr');
                    selectRow(row);
                });
            });

            // bind individual checkboxes
            $("#groupPricingGridView input:checkbox").bind('click', function () {
                var row = $(this).parents('tr');
                selectRow(row);
            });

            init_jqgrids('#group_pricing');
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
            $('#gview_groupPricingGridView')
                .find(".ui-jqgrid-htable tr > th")
                .removeClass('jqgrid_sorted');
            $('#gview_groupPricingGridView')
                .find(".ui-jqgrid-htable tr > th:eq(" + colIndex + ")")
                .addClass('jqgrid_sorted');
        },
        pager: 'groupPricingGridNavigation',
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
    }).navGrid('groupPricingGridNavigation', { refresh: false, search: false, edit: false, add: false, del: false });
}