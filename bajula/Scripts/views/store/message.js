$(document).ready(function () {
    inputSelectors_init();
    init_autogrow();

    $('#buttonSend').click(function () {
        $(this).buttonDisable();
        $('#buttonSend').trigger("submit");
    });
    $('#buttonCancel').click(function () {
        dialogBox_close();
    });

    $('#messageForm').submit(function () {
        var serialized = $(this).serialize();
        var action = $(this).attr("action");
        var ok = $('#messageForm').validate({
            invalidHandler: function (form, validator) {
                $(validator.invalidElements()[0]).focus();
            },
            focusInvalid: false,
            rules: {
                email: {
                    required: true,
                    email: true
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
                    $.jGrowl("Message sent succesfully. We will get back to you soon.");
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
});