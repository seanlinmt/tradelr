<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>
<h3><img src="/Content/img/social/icons/wordpress_32.png" /> Wordpress Credentials</h3>
<form id="passwordForm" action="/wordpress/credentials" method="post">
<p>Your credentials will be stored encrypted.</p>
<div class="form_entry">
    <div class="form_label">
        <label for="url">
            Blog URL</label>
    </div>
    <%= Html.TextBox("url") %>
</div>
<div class="form_entry">
    <div class="form_label">
        <label for="username">
            Username</label>
    </div>
    <%= Html.TextBox("username") %>
</div>
<div class="form_entry">
    <div class="form_label">
        <label for="password">
            Password</label>
    </div>
    <%= Html.Password("password")%>
</div>
<div class="mt10">
    <button id="buttonSave" type="button" class="ajax green">
        save</button>
    <button id="buttonCancel" type="button">
        cancel</button>
</div>
</form>
<script type="text/javascript">
    $(document).ready(function () {
        $('#buttonSave', '#passwordForm').click(function () {
            $(this).buttonDisable();
            $(this).trigger('submit');
        });
        $('#buttonCancel', '#passwordForm').click(function () {
            dialogBox_close();
            $('#buttonlogin').buttonEnable();
            $('#buttonreenter').buttonEnable();
        });

        $('#passwordForm').submit(function () {
            var action = $(this).attr("action");
            var serialized = $(this).serialize();
            var ok = $('#passwordForm').validate({
                invalidHandler: function (form, validator) {
                    $(validator.invalidElements()[0]).focus();
                },
                focusInvalid: false,
                rules: {
                    url: {
                        required: true
                    },
                    username: {
                        required: true
                    },
                    password: {
                        required: true
                    }
                }
            }).form();
            if (!ok) {
                $('#buttonSave', '#passwordForm').buttonEnable();
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
                        $('#notconnected').hide();
                        $('#connected').show();
                        dialogBox_close();
                    }
                    else {
                        $.jGrowl(json_data.message);
                    }
                    $('#buttonSave', '#passwordForm').buttonEnable();
                    return false;
                }
            });
            return false;
        });
        inputSelectors_init();
    });
</script>
