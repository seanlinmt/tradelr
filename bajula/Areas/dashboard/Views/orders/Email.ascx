<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<tradelr.Email.Email>" %>
<h3 id="headingEmail">
     <%= Model.heading %></h3>
<form id="orderEmailForm" action="/dashboard/orders/send" method="post">
<div class="form_entry">
    <div class="form_label">
        <label>
            To:</label>
    </div>
    <%= Model.receiver %>
</div>
<div class="form_entry">
    <div class="form_label">
        <label>
            Subject:</label>
    </div>
    <%= Html.TextBox("subject", Model.subject, 
        new Dictionary<string, object> { { "style", "width:100%" } })%>
</div>
<div class="form_entry">
    <div class="form_label">
        <label>
            Message:</label>
    </div>
    <%= Html.TextArea("message", Model.message)%>
    <span class="tip">Link: <%= Model.viewloc %></span>
    <%= Html.Hidden("viewloc", Model.viewloc) %>
</div>
<div>Best regards,<br />
<%= Model.sender %>
</div>
    <div class="mt10">
        <button id="buttonSend" type="button" class="ajax green">
            send</button>
        <button id="buttonCancel" type="button">
            cancel</button>
    </div>
    <%= Html.Hidden("id", Model.orderID)%>
</form>
<script type="text/javascript">
    $(document).ready(function () {
        var fromNewOrderPage = false;
        var segments = window.location.pathname.split('/');
        $.each($(segments), function (i) {
            if (this == 'add' || this == 'generate') {
                fromNewOrderPage = true;
            }
        });

        $('#buttonSend', '#orderEmailForm').click(function () {
            $(this).buttonDisable();
            $(this).trigger('submit');
        });
        $('#buttonCancel', '#orderEmailForm').click(function () {
            $('#buttonSaveSend').hide();
            dialogBox_close();
        });

        $('#orderEmailForm').submit(function () {
            var action = $(this).attr("action");
            var serialized = $(this).serialize();
            var ok = $('#orderEmailForm').validate({
                invalidHandler: function (form, validator) {
                    $(validator.invalidElements()[0]).focus();
                },
                focusInvalid: false,
                rules: {
                    subject: {
                        required: true
                    },
                    message: {
                        required: true
                    }
                }
            }).form();
            if (!ok) {
                $('#buttonSend').buttonEnable();
                $('#buttonSaveSend').hide();
                return false;
            }
            // post form
            $.ajax({
                type: "POST",
                url: action,
                data: serialized,
                dataType: 'json',
                success: function (json_result) {
                    if (json_result.success) {
                        // flash message
                        $.jGrowl('Email sent!');
                        if (fromNewOrderPage) {
                            
                            if (segments.indexOf('orders') != -1) {
                                window.location = '/dashboard/orders/' + $('#id').val();
                            }
                            else {
                                window.location = '/dashboard/invoices/' + $('#id').val();
                            } 
                            dialogBox_close();
                        }
                        else {
                            $('#buttonSend').buttonEnable();
                            window.location.reload();
                        }
                    }
                    else {
                        var error = json_result.data;
                        if (error == tradelr.returncode.NOPERMISSION) {
                            if (segments[1] == 'orders') {
                                $.jGrowl("You do not have permission to send purchase orders");
                            }
                            else {
                                $.jGrowl("You do not have permission to send invoices");
                            }
                        }
                        $('#buttonSend').buttonEnable();
                    }
                }
            });
            return false;
        });
        inputSelectors_init();
        init_autogrow('#orderEmailForm');
    });
</script>