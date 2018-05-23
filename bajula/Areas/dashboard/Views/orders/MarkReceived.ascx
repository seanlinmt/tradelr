<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<long>" %>
<h3 id="headingReceived">
     Order Received</h3>
<form id="receivedForm" action="/dashboard/orders/markreceived/<%= Model %>" method="post">
<div class="form_entry">
    <div class="form_label">
        <label class="required">
            Date order was received</label>
    </div>
    <%= Html.TextBox("actualDate")%>
</div>

    <div class="pad5">
        <button id="buttonSave" type="button" class="ajax green">
            save</button>&nbsp;
        <button id="buttonCancel" type="button">
            cancel</button>
    </div>
</form>
<script type="text/javascript">
    $(document).ready(function () {
        $('#actualDate', '#receivedForm').datepicker(
        {
            dateFormat: 'D, d M y'
        });

        $('#buttonSave', '#receivedForm').click(function () {
            $(this).trigger('submit');
        });

        $('#buttonCancel', '#receivedForm').click(function () {
            dialogBox_close();
        });

        $('#receivedForm').submit(function () {
            var action = $(this).attr('action');
            var serialized = $(this).serialize();
            var ok = $('#receivedForm').validate({
                invalidHandler: function (form, validator) {
                    $(validator.invalidElements()[0]).focus();
                },
                focusInvalid: false,
                rules: {
                    actualDate: {
                        required: true
                    }
                }
            }).form();
            if (!ok) {
                return false;
            }
            // post form
            $.ajax({
                type: "POST",
                url: action,
                data: serialized,
                success: function () {
                    // flash message
                    $.jGrowl('Order marked as received');
                    scrollToTop();
                    window.location.reload();
                }
            });
            return false;
        });
        inputSelectors_init();
    });
</script>