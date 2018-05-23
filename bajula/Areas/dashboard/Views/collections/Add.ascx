<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>
<%@ Import Namespace="tradelr.Libraries.Extensions"%>
<form id="collectionAddForm" action="/dashboard/collections/create" method="post">
        <h3 id="headingAdd">enter collection name</h3>
        <div class="form_entry">
            <div class="form_label">
                <label for="title">
                    Collection Title</label>
            </div>
            <%= Html.TextBox("title")%>
        </div>
        <div class="form_entry">
            <div class="form_label">
                <label for="details">
                    Description</label>
            </div>
            <%= Html.TextArea("details")%>
        </div>
        <div class="form_entry">
<%= Html.CheckBox("visible") %> <label for="visible">make this collection visible</label>
</div>
        <div class="pt5">
            <button id="saveButton" type="button" class="green">
                add</button>
            <button id="cancelButton" type="button">
                cancel</button>
        </div>
</form>
<script type="text/javascript">
    $(document).ready(function () {
        $('#title', '#collectionAddForm').focus();
        init_autogrow('#collectionAddForm');
        $('#cancelButton', '#collectionAddForm').bind("click", function () {
            // when adding from new product page
            $('#collection').defaultOption();
            dialogBox_close();
            return false;
        });

        $('#saveButton', '#collectionAddForm').bind("click", function () {
            $(this).trigger('submit');
            return false;
        });

        $('#collectionAddForm').submit(function () {
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
                        $('#collection').insertOption(data.name, data.id);

                        // add entry to side filter list
                        var entry = "<div class='sideboxEntry' style='display:none' fid='" + data.id + "'>" + data.name + "<div class='del'></div><div class='edit'></div></div>";
                        $('#collectionsList > div:last', '#inventory_mine').after(entry);
                        $('#collectionsList > div:last', '#inventory_mine').slideDown();
                        dialogBox_close();
                    }
                    else {
                        $.jGrowl(json_data.message);
                    }
                }
            });
            return false;
        });

        // input highlighters
        inputSelectors_init();

    });
</script>

