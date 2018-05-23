<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>
<%@ Import Namespace="tradelr.Libraries.Extensions"%>
<form id="categoryAddForm" action="<%= Url.Action("Add", "Category") %>" method="post" autocomplete="off">
        <h3 id="headingAdd">enter a category name</h3>
        <div class="form_entry">
            <div class="form_label">
                <label for="categoryTitle">
                    Category Name</label>
            </div>
            <%= Html.TextBox("categoryTitle")%>
            <%= Html.Hidden("categoryTitleSelected")%>
        </div>
        
        <div class="pad5">
            <button type="submit" class="green">
                add</button>&nbsp;
            <button id="cancelButton">
                cancel</button>
        </div>
</form>
<script type="text/javascript">
    $(document).ready(function () {
        $('#categoryTitle').focus();
        $('#cancelButton', '#categoryAddForm').bind("click", function () {
            // when adding from new product page
            $('#maincategory').defaultOption();

            dialogBox_close();
            return false;
        });

        $('#categoryAddForm').submit(function () {
            var action = $(this).attr("action");
            var serialized = $(this).serialize();

            // post form
            $.ajaxswitch({
                type: "POST",
                url: action,
                dataType: "json",
                data: serialized,
                success: function (json_data) {
                    if (json_data.success) {
                        var data = json_data.data;
                        // add entry to dropdownlist
                        $('#maincategory').insertOption(data.title, data.id);

                        // init subcategory
                        $('#subcategory').html('<option value="">None</option>');
                        $('#subcategory').appendable('/dashboard/category/addsub/' + data.id, 'Add Sub Category');

                        // add entry to side filter list
                        var entry = "<div class='sideboxEntry' style='display:none' fid='" + data.id + "'><span class='arrow_right'></span>" + data.title + "<div class='del'></div><div class='edit'></div><div class='add'></div></div>";
                        $('#categoryList > div:last', '#inventory_mine').after(entry);
                        $('#categoryList > div:last', '#inventory_mine').slideDown();
                        dialogBox_close();
                    }
                    else {
                        $.jGrowl(json_data.message);
                    }
                }
            });
            return false;
        });
        $('#categoryTitle').autocomplete('/dashboard/category/find', {
            dataType: "json",
            parse: function (json_data) {
                var rows = new Array();
                var data = json_data.data;
                for (var i = 0; i < data.length; i++) {
                    rows[i] = { data: data[i], value: data[i].title, result: data[i].title };
                }
                return rows;
            },
            autoFill: true,
            formatItem: function (row, i, max) {
                return row.title;
            }
        });
        $("#categoryTitle").result(function (event, data, formatted) {
            if (data)
                $("#categoryTitleSelected").val(data.id);
        });

        // input highlighters
        inputSelectors_init();

    });
</script>

