<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Supplier>" %>
<%@ Import Namespace="tradelr.Models.users"%>
<form id="supplierAddForm" action="/supplier/add" method="post">
    <h3 id="headingAdd">Add Supplier</h3>
    <p>Select a contact to mark them as a supplier</p>
    <div class="form_entry">
        <div class="form_label">
            <label for="coTitle">
                Select Contact</label>
        </div>
        <%= Html.DropDownList("supplier", Model.supplierList)%>
    </div>
    <div class="pad5">
        <button id="buttonAdd" type="button" class="green">
            Add</button>&nbsp;
        <button id="buttonCancel" type="button">
            Cancel</button>
    </div>
</form>
<script type="text/javascript">
    $(document).ready(function() {
        $('#supplierAddForm #buttonAdd').bind('click', function() {
            $('#supplierAddForm').trigger('submit');
            return false;
        });

        $('#supplierAddForm #buttonCancel').bind("click", function() {
            $('#supplier').defaultOption();
            dialogBox_close();
            return false;
        });
        $('#supplierAddForm').submit(function() {
            var action = $(this).attr("action");
            var serialized = $(this).serialize();

            // post form
            $.ajax({
                type: "POST",
                url: action,
                dataType: "json",
                data: serialized,
                success: function(json_data) {
                    if (json_data.success) {
                        var data = json_data.data;

                        // add entry to side filter list
                        var entry = "<div class='sideboxEntry' style='display:none'>" + data.name + "<span class='hidden'>" + data.id + "</span></div>";
                        $('#supplierList > div:last').after(entry);
                        $('#supplierList > div:last').slideDown('slow');
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
