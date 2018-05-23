<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<tradelr.Models.transactions.OrderShippingCost>" %>
<h3>Update Shipping Cost</h3>
<form id="shippingCostForm" action="<%= Url.Action("EditShipping","Orders") %>" method="post">
<div class="form_entry">
    <div class="form_label">
        <label for="quantity" class="required">
            Shipping Cost</label>
    </div>
    <%= Html.TextBox("cost", Model.shippingCost) %>
</div>
<div class="form_entry">
    <div class="form_label">
        <label for="description" class="required">
            Shipping Method</label>
    </div>
    <%= Html.TextBox("method", Model.shippingMethod)%>
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
        $('#cost', '#shippingCostForm').numeric({ allow: '.' });
        $('#buttonSave', '#shippingCostForm').click(function () {
            $(this).trigger('submit');
        });
        $('#buttonCancel', '#shippingCostForm').click(function () {
            dialogBox_close();
        });

        $('#shippingCostForm').submit(function () {
            var action = $(this).attr("action");
            var serialized = $(this).serialize();
            var ok = $('#shippingCostForm').validate({
                invalidHandler: function (form, validator) {
                    $(validator.invalidElements()[0]).focus();
                },
                focusInvalid: false,
                rules: {
                    cost: {
                        required: true
                    },
                    method: {
                        required: true
                    }
                }
            }).form();
            if (!ok) {
                return false;
            }

            $('#buttonSave', '#shippingCostForm').buttonDisable();
            
            // post form
            $.ajax({
                type: "POST",
                url: action,
                data: serialized,
                dataType: 'json',
                success: function (json_data) {
                    if (json_data.success) {
                        $.jGrowl(json_data.message);
                        window.location.reload();
                    }
                    else {
                        $.jGrowl(json_data.message);
                    }
                    return false;
                }
            });
            return false;
        });
        inputSelectors_init();
    });
</script>
