<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>
<form id="stockUnitAddForm" action="/dashboard/stockUnit/add" method="post">
<div>
        <h3 id="headingAdd">New Stock Unit</h3>
        <div class="form_entry">
            <div class="form_label">
                <label for="unitTitle">
                    Stock Unit</label>
            </div>
            <%= Html.TextBox("unitTitle")%>
            <%= Html.Hidden("unitTitleSelected")%>
        </div>
       
        <div class="pad5">
            <button type="submit" class="green">
                add</button>&nbsp;
            <button class="cancelButton">
                cancel</button>
        </div>
</div>
<%= Html.Hidden("ids") %>
</form>
<script type="text/javascript">
    $(document).ready(function() {
        $('#stockUnitAddForm .cancelButton').bind("click", function() {
            $('#stockUnit').defaultOption();
            dialogBox_close();
            return false;
        });
        $('#stockUnitAddForm').submit(function() {
            var action = $(this).attr("action");
            var serialized = $(this).serialize();

            // post form
            $.ajaxswitch({
                type: "POST",
                url: action,
                dataType: "json",
                data: serialized,
                success: function(json_data) {
                    if (json_data.success) {
                        var data = json_data.data;
                        // add entry to dropdownlist
                        $('#stockUnit').insertOption(data.title, data.id);
                        dialogBox_close();
                    }
                    else {
                        $.jGrowl(json_data.message);
                    }
                }
            });
            return false;
        });
        $('#unitTitle').autocomplete('/dashboard/stockunit/find', {
            dataType: "json",
            parse: function(data) {
                var rows = new Array();
                for (var i = 0; i < data.length; i++) {
                    rows[i] = { data: data[i], value: data[i].title, result: data[i].title };
                }
                return rows;
            },
            autoFill: true,
            formatItem: function(row, i, max) {
                return row.title;
            }
        });
        $("#unitTitle").result(function(event, data, formatted) {
            if (data)
                $("#unitTitleSelected").val(data.id);
        });

        // input highlighters
        inputSelectors_init();

    });
</script>
