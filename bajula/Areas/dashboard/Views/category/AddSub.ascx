<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ProductCategory>" %>
<%@ Import Namespace="tradelr.Models.products"%>
<%@ Import Namespace="tradelr.Libraries.Extensions"%>
<form id="categoryAddForm" action="<%= Url.Action("AddSub", "Category") %>" method="post" autocomplete="off">
        <h3 id="headingAdd" class="mtitle"></h3>
        <div class="form_entry">
            <div class="form_label">
                <label for="categoryTitle">
                    Subcategory Name</label>
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
        <%= Html.Hidden("id", Model.id) %>
</form>
<%= Html.Hidden("maincatTitle", Model.title) %>
<script type="text/javascript">
    $(document).ready(function () {
        if (tradelr.webdb.db != null) {
            var category = $('#maincategory').val();
            var sql = "SELECT id,name FROM category WHERE id=" + category;
            tradelr.webdb.sqlGetRows(sql, [], category, function (result) {
                var row = result.item(0);
                $('#id', '#categoryAddForm').val(row['id']);
                $('#maincatTitle').val(row['name']);
                addsub_init();
            });
        }
        else {
            addsub_init();
        }
    });

    function addsub_init() {
        $('#categoryTitle').focus();
        $('.mtitle').text($('#maincatTitle').val());

        $('#cancelButton', '#categoryAddForm').bind("click", function () {
            // when adding from new product page
            $('#subcategory').defaultOption();

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
                        if ($('#subcategory').length != 0) {
                            $('#subcategory').insertOption(data.title, data.id);
                        }

                        // add entry to side filter list
                        var mainid = $('#id', '#categoryAddForm').val();
                        var entry = "<div class='sideboxSubEntry' style='display:block' fid='" + data.id + "'>" + data.title + "<div class='del' title='delete subcategory'></div><div class='edit' title='edit subcategory'></div></div>";
                        var insertionPoint = $('#categoryList .sideboxEntry[fid=' + mainid + ']', '#inventory_mine');
                        $(insertionPoint).after(entry);
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
    }
</script>

