$('#loginForm').submit(function () {
    var serialized = $(this).serialize();
    var redirect = querySt('redirect');
    if (redirect != undefined) {
        serialized += "&redirect=" + redirect;
    }
    var action = $(this).attr("action");

    var ok = $('#loginForm').validate({
        invalidHandler: function (form, validator) {
            $(validator.invalidElements()[0]).focus();
        },
        focusInvalid: false,
        rules: {
            email: {
                required: true,
                email: true
            },
            password: {
                required: true
            }
        }
    }).form();

    if (!ok) {
        $('#buttonLogin').buttonEnable();
        return false;
    }

    $.ajax({
        type: "POST",
        url: action,
        data: serialized,
        dataType: "json",
        success: function (json_data) {
            if (json_data.success) {
                window.location = json_data.data;
            }
            else {
                $.jGrowl(json_data.message, { sticky: true});
            }
            $('#buttonLogin').buttonEnable();
        }
    });
    return false;
});

$(document).ready(function () {
    $('#buttonLogin').click(function () {
        $(this).buttonDisable();
        $(this).trigger('submit');
    });

    $('#email').focus().addClass('curFocus');
    // input highlighters
    inputSelectors_init();

    $('#signedin').change(function () {
        if ($('#signedin').is(':checked')) {
            $('#checkedmessage').show();
        }
        else {
            $('#checkedmessage').hide();
        }
    });

    // handle login for demo account
    var segments = window.location.host.split('.');
    if (segments.length != 0 && segments[0] == 'demo') {
        $('#email').val('support@tradelr.com');
        $('#password').val('1234%^&*');
        $('#loginForm').trigger('submit');
    }
});