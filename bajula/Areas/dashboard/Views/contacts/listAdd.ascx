<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<IEnumerable<ContactBasic>>" %>
<%@ Import Namespace="tradelr.Models.contacts"%>
<%@ Import Namespace="tradelr.Libraries.Extensions"%>
<form id="listAddForm" action="/dashboard/contacts/listAdd" method="post">
        <h3 id="headingAdd">new group</h3>
        <div class="form_entry">
            <div class="form_label">
                <label for="filterTitle">
                    Group Name</label>
            </div>
            <%= Html.TextBox("filterTitle")%>
        </div>
        <p>Select contacts to add to this group</p>
        <%
            Html.RenderPartial("contactList", Model); %>
        <div id="buttons" class="pt5">
            <button id="buttonAdd" type="button" class="green">
                add</button>
            <button id="buttonCancel">
                cancel</button>
        </div>
<%= Html.Hidden("ids") %>
</form>
<script type="text/javascript">
    $(document).ready(function () {
        $('#buttonCancel', '#listAddForm').click(function () {
            dialogBox_close();
            return false;
        });

        $('#buttonAdd', '#listAddForm').click(function () {
            $(this).buttonDisable();
            $('#listAddForm').trigger('submit');
            return false;
        });

        $('#listAddForm button').bind("click", function () {
            $('.buttonRow > button').removeClass('selected');
        });

        $('#listAddForm').submit(function () {
            var action = $(this).attr("action");
            var ids = [];
            var selected = $(this).find(".selected");
            for (var i = 0; i < selected.length; i++) {
                ids.push($(selected[i]).find('.contactid').text());
            }
            $('#ids').val(ids.toString());
            var serialized = $(this).serialize();

            var ok = $('#listAddForm').validate({
                invalidHandler: function (form, validator) {
                    $(validator.invalidElements()[0]).focus();
                },
                focusInvalid: false,
                rules: {
                    filterTitle: "required"
                }
            }).form();
            if (!ok) {
                $('#buttonAdd').buttonEnable();
                return false;
            }

            // post form
            $.ajax({
                type: "POST",
                url: action,
                dataType: "json",
                data: serialized,
                success: function (json_data) {
                    if (json_data.success) {
                        var data = json_data.data;
                        dialogBox_close();
                        var entry = "<div class='sideboxEntry' class='hidden' fid='" + data.id + "'><div class='title'>" + data.title + "</div><div class='edit'></div><div class='del'></div></div>";
                        $('#contactsList > div:last', '#group_pricing').after(entry);
                        $('#contactsList > div:last', '#contacts_mine').after(entry);
                        $('#contactsList > div:last', '#group_pricing').slideDown('slow');
                        $('#contactsList > div:last', '#contacts_mine').slideDown('slow');

                        // trigger click
                        getMatchingFilterButton('#contactsList', data.id).trigger('click');
                    }
                    else {
                        $.jGrowl(json_data.message);
                    }
                    $('#buttonAdd', '#listAddForm').buttonEnable();
                }
            });
            return false;
        });

        // input highlighters
        inputSelectors_init();
    });
</script>