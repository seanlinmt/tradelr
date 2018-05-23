$(document).ready(function () {
    $('#buttonSend').click(function () {
        $(this).buttonDisable();
        $('#buttonSend').trigger("submit");
    });

    $('#buttonCancel').click(function () {
        dialogBox_close();
    });

    $('#message', '#supportForm').focus();

    $('#supportForm').submit(function () {
        var serialized = $(this).serialize();
        var action = $(this).attr("action");
        var ok = $('#supportForm').validate({
            invalidHandler: function (form, validator) {
                $(validator.invalidElements()[0]).focus();
            },
            focusInvalid: false,
            rules: {
                message: {
                    required: true
                }
            }
        }).form();

        if (!ok) {
            $('#buttonSend').buttonEnable();
            return false;
        }

        $.ajax({
            type: "POST",
            url: action,
            data: serialized,
            dataType: "json",
            success: function (json_data) {
                if (json_data.success) {
                    $.jGrowl("Your message has been sent. We will email a reply soon.");
                    dialogBox_close();
                }
                else {
                    $.jGrowl(json_data.message);
                }
                $('#buttonSend').buttonEnable();
                return false;
            }
        });
        return false;
    });

    init_autogrow('#supportForm');
    inputSelectors_init();
});