<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<decimal?>" %>
<h3 id="headingTerms">
     Set Default Tax / GST / VAT</h3>
     <p>The tax rate set here will be used in future products.</p>
<form id="taxForm" action="/dashboard/product/tax" method="post">
<div class="form_entry">
    <div class="form_label">
        <label>
            Tax / GST / VAT (%)</label>
    </div>
    <%= Html.TextBox("taxrate", Model)%>
</div>
<input name="useTax" id="useTax" type="checkbox" checked="checked" />
<label for="useTax">apply value to this product</label>

    <div class="pad5">
        <button id="buttonSave" type="button" class="green ajax">
            save</button>
        <button id="buttonCancel" type="button">
            cancel</button>
            </div>
</form>
<script type="text/javascript">
    $(document).ready(function () {
        $('#taxrate', '#taxForm').numeric({ allow: '.' });
        $('#buttonSave', '#taxForm').click(function () {
            $(this).buttonDisable();
            $(this).trigger('submit');
        });
        $('#buttonCancel', '#taxForm').click(function () {
            dialogBox_close();
        });
        $.validator.addMethod("percent", function (value, element) {
            return this.optional(element) || (parseFloat(value, 10) < 100);
        }, 'must be less than 100%');

        $('#taxForm').submit(function () {
            var action = $(this).attr("action");
            var serialized = $(this).serialize();
            var ok = $('#taxForm').validate({
                invalidHandler: function (form, validator) {
                    $(validator.invalidElements()[0]).focus();
                },
                focusInvalid: false,
                rules: {
                    taxrate: {
                        percent: true
                    }
                }
            }).form();
            if (!ok) {
                $('#buttonSave', '#taxForm').buttonEnable();
                return false;
            }
            // post form
            $.ajax({
                type: "POST",
                url: action,
                data: serialized,
                success: function (data) {
                    // flash message
                    $.jGrowl('Your default tax rate has been updated successfully');

                    // if checkbox is set then set the current term
                    if ($('#useTax:checked', '#taxForm').val() !== undefined) {
                        var tax = $('#taxrate', '#taxForm').val();
                        $('#taxrate', '#productAddForm').val(tax);
                    }
                    $('#buttonSave', '#taxForm').buttonEnable();
                    dialogBox_close();
                }
            });
            return false;
        });
        inputSelectors_init('#taxForm');
    });
</script>
