$(document).ready(function() {
    $('#firstname').watermark('First Name');
    $('#lastname').watermark('Last Name');
    $('#phone').watermark('optional');

    $('#registerButton').click(function() {
        $(this).buttonDisable();
        $('#registerForm').trigger('submit');
    });

    $('#buttonCancel').click(function() {
        dialogBox_close();
    });

    // registration form
    $('#registerForm').submit(function() {
        var serialized = $(this).serialize();
        var action = $(this).attr("action");

        var ok = $('#registerForm').validate({
            invalidHandler: function(form, validator) {
                $(validator.invalidElements()[0]).focus();
            },
            focusInvalid: false,
            rules: {
                firstname: {
                    required: true
                },
                lastname: {
                    required: true
                },
                email: {
                    required: true,
                    email: true
                },
                emailagain: {
                    required: true,
                    email: true,
                    equalTo: "#email"
                },
                password: {
                    required: true,
                    minlength: 6
                }
            }
        }).form();
        if (!ok) {
            $('#registerButton').buttonEnable();
            $('#firstname').watermark('First Name');
            $('#lastname').watermark('Last Name');
            $('#phone').watermark('optional');
            return false;
        }
        // post form
        $.ajax({
            type: "POST",
            url: action,
            data: serialized,
            dataType: "json",
            success: function(data) {
                if (!data.success) {
                    $.jGrowl(data.message);
                }
                else {
                    dialogBox_close();
                    $.jGrowl(data.message);
                }
                $('#registerButton').buttonEnable();
                return false;
            }
        });
        return false;
    });

    // input highlighters
    $("input,textarea").initInputSelectors();
});