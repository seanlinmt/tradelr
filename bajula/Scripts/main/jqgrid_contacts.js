///// contacts grid
function getContactTypeFilter() {
    return $.trim($("#filterByType").html());
}

function setContactTypeFilter(val) {
    $("#filterByType").html(val);
}

function reloadContactsGrid() {
    $("#contactsGridView").setGridParam({
            url: '/dashboard/contacts/grid',
            postData: {
                cat: $('#ContactFilterBy').val(),
                type: $('#ContactFilterByType').val(),
                letter: $('#ContactFilterByLetter').val(),
                term: $('#ContactsSearchBox').val()
            }
        });
    $("#contactsGridView").trigger("reloadGrid");
}

function contactBindToGrid() {
    var selectRow = function (row) {
        row.toggleClass('selected-row');
    };

    $("#contactsGridView").jqGrid({
        afterInsertRow: function (id, data, element) {
            // handle contact name
            var name = data['firstName'];
            var org = name.split(',');
            var orgname = org[0];
            var orgtype = '';
            if (org[1] != undefined) {
                orgtype = org[1];
            }
            var view = orgname + "<span class=\"orgtype\">" + orgtype + "</span>";

            // update row
            $("#contactsGridView").setRowData(id, { sel: '<input class="ml4" type="checkbox"/>', firstName: view });
            $("#" + id, "#contactsGridView").find('td:eq(3)').attr('title', org[0]); 
        },
        altRows: true,
        autowidth: true,
        cellEdit: false,
        cellSubmit: 'remote',
        cellurl: '/dashboard/contacts/update',
        colNames: ['', '', '', 'Main Contact', 'Organization', 'Phone No.', 'Email', ''],
        colModel: [
                    { name: 'pid', hidden: true },
                    { name: 'sel', width: 20, sortable: false, editable: false },
                    { name: 'thumbnail', classes: 'jqgrid_thumb', index: 'thumbnail', width: 50, align: 'center', sortable: false },
                    { name: 'firstName', index: 'firstName', width: 130, align: 'left' },
                    { name: 'name', index: 'organisation1.name', width: 250, align: 'left' },
                    { name: 'phone', index: 'phoneNumber', width: 120, align: 'left', editable: true },
                    { name: 'email', index: 'email', width: 150, align: 'left', editable: true },
                  { name: 'act', width: 50, align: 'center', padding: 4, sortable: false }
                  ],
        datatype: 'json',
        height: '100%',
        hoverrows: false,
        imgpath: '/Content/images',
        loadComplete: function () {
            // add checkbox in header
            $('#gview_contactsGridView')
                .find(".ui-jqgrid-htable tr > th:eq(1)")
                .html('<input id="cbAll" type="checkbox"/>');

            $('#cbAll', '#gview_contactsGridView').bind('click', function () {
                if ($(this).is(':checked')) {
                    $("#contactsGridView input:checkbox").attr('checked', true);
                }
                else {
                    $("#contactsGridView input:checkbox").attr('checked', false);
                }
                $.each($("#contactsGridView input:checkbox"), function () {
                    var row = $(this).parents('tr');
                    selectRow(row);
                });
            });

            // bind individual checkboxes
            $("#contactsGridView input:checkbox").bind('click', function () {
                var row = $(this).parents('tr');
                selectRow(row);
            });
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
            $('#gview_contactsGridView')
                .find(".ui-jqgrid-htable tr > th")
                .removeClass('jqgrid_sorted');
            $('#gview_contactsGridView')
                .find(".ui-jqgrid-htable tr > th:eq(" + colIndex + ")")
                .addClass('jqgrid_sorted');
        },
        pager: $('#contactNavigation'),
        rowNum: 10,
        rowList: [10, 50, 100, 1000],
        rownumbers: false,
        shrinkToFit: true,
        sortname: 'firstName',
        sortorder: 'asc',
        url: '/dashboard/contacts/grid',
        viewrecords: true,
        viewsortcols: true
    }).navGrid('#contactNavigation', { search: false, refresh: false, edit: false, add: false, del: false });
}
