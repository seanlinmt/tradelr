<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<tradelr.Models.inventory.InventoryLocationItem>" %>
<h3 id="headingTerms"> Edit Inventory Level</h3>
<form id="availableForm" action="/dashboard/inventory/available" method="post">
<span class="tip">Inventory levels are automatically updated when invoices and purchase orders are created. Modify levels only if necessary.</span>
<div class="form_entry">
    <div class="form_label">
        <label for="quantity">
            number in stock</label>
    </div>
    <%= Html.TextBox("quantity", Model.inStock) %>
</div>
<div class="form_entry">
    <div class="form_label">
        <label for="description">
            Reason for change</label>
    </div>
    <%= Html.TextArea("description")%>
</div>
<div class="mt10">
    <button id="buttonSave" type="button" class="ajax green">
        save</button>
    <button id="buttonCancel" type="button">
        cancel</button>
</div>
<%= Html.Hidden("id", Model.id) %>
</form>
<script type="text/javascript">
    $(document).ready(function () {
        $('#quantity', '#availableForm').numeric();
        $('#buttonSave', '#availableForm').click(function () {
            $(this).buttonDisable();
            $(this).trigger('submit');
        });
        $('#buttonCancel', '#availableForm').click(function () {
            dialogBox_close();
        });

        $('#availableForm').submit(function () {
            var action = $(this).attr("action");
            var serialized = $(this).serialize();
            var ok = $('#availableForm').validate({
                invalidHandler: function (form, validator) {
                    $(validator.invalidElements()[0]).focus();
                },
                focusInvalid: false,
                rules: {
                    description: {
                        required: true
                    },
                    quantity: {
                        required: true
                    }
                }
            }).form();
            if (!ok) {
                $('#buttonSave', '#availableForm').buttonEnable();
                return false;
            }
            // post form
            $.ajax({
                type: "POST",
                url: action,
                data: serialized,
                dataType: 'json',
                success: function (json_data) {
                    if (json_data.success) {
                        $.jGrowl(json_data.message);
                        var item = $('.itemid:contains(' + $('#id', '#availableForm').val() + ')', '#inventory').parents('.content_row');
                        $('.instock', item).html($('#quantity', '#availableForm').val());
                        dialogBox_close();
                    }
                    else {
                        $.jGrowl(json_data.message);
                    }
                    $('#buttonSave', '#availableForm').buttonEnable();
                    return false;
                }
            });
            return false;
        });
        inputSelectors_init();
        init_autogrow('#availableForm');
    });
</script>
