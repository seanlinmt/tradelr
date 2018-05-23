<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Dictionary<string,object>>" %>
<%@ Import Namespace="tradelr.Libraries.Extensions"%>
<%@ Import Namespace="tradelr.Models.contacts" %>
<form id="listEditForm" action="/dashboard/contacts/listEdit" method="post">
        <h3 class="headingEdit"><span class="font_black"><%= Model["listTitle"] %></span> contact group</h3>
        <%
            Html.RenderPartial("contactList", (IEnumerable<ContactBasic>) Model["contactList"]); %>
        <div id="buttons" class="pt5">
            <button id="buttonUpdate" type="submit" class="green">
                update</button>
            <button id="buttonCancel">
                cancel</button>
        </div>
<%= Html.Hidden("id", Model["filterid"]) %>
<%= Html.Hidden("ids", Model["ids"]) %>
</form>
<script type="text/javascript">
    $(document).ready(function () {
        $('#buttonCancel', '#listEditForm').click(function () {
            dialogBox_close();
            return false;
        });

        $('#buttonUpdate', '#listEditForm').click(function () {
            $(this).buttonDisable();
            $('#listEditForm').trigger('submit');
            return false;
        });

        // highlight contacts in list
        var ids = $('#ids', '#listEditForm').val().split(',');
        var blocks = $('#listEditForm').find('.blockSelectable');
        $.each(blocks, function () {
            var id = $.trim($(this).find('.contactid').text());
            if ($.inArray(id, ids) != -1) {
                $(this).addClass('selected');
            }
        });

        $('#listEditForm button').bind("click", function () {
            $('.buttonRow > button').removeClass('selected');
        });
        $('#listEditForm').submit(function () {
            var action = $(this).attr("action");
            var ids = [];
            var selected = $(this).find(".selected");
            for (var i = 0; i < selected.length; i++) {
                ids.push($(selected[i]).find('.contactid').text());
            }
            $('#ids').val(ids.toString());

            var serialized = $(this).serialize();

            // post form
            $.ajax({
                type: "POST",
                url: action,
                dataType: "json",
                data: serialized,
                success: function (json_data) {
                    if (json_data.success) {
                        dialogBox_close();
                        reloadContactsGrid();
                    }
                    else {
                        $.jGrowl(json_data.message);
                    }
                    $('#buttonUpdate').buttonEnable();
                }
            });
            return false;
        });
    });
</script>
