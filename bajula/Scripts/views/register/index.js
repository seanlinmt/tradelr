$(document).ready(function () {
    $('#banner_bottom').hide();
    $('.loginPage').alphanumeric();

    $('#registerButton').click(function () {
        $(this).buttonDisable();
        $('#registerForm').trigger('submit');
    });

    $('#useemail').click(function () {
        $('#signup_facebook').slideUp('fast', function () {
            $('#signup_email').slideDown('fast');
        });
        return false;
    });

    $('#cancelButton').click(function () {
        $('#signup_email').slideUp('fast', function () {
            $('#signup_facebook').slideDown('fast');
        });
        return false;
    });

    $('#signin-facebook').click(function () {
        var ok = $('#registerForm').validate().form();
        if (!ok) {
            return false;
        }
        var sitename = $('.loginPage').val();
        var planname = $('#plan', '#registerForm').val();
        window.location = "/fb/register?name=" + sitename + "&plan=" + planname;
    });

    $.validator.addMethod("loginPage", function (value) {
        var result = value.length > 0 && value.length < 65 && /^[a-z0-9]+$/i.test(value);
        return result;
    }
    , 'Length must be less than 64 characters and only contain letters and numbers');

    // registration form
    $('#registerForm').submit(function () {
        var serialized = $(this).serialize();
        var action = $(this).attr("action");

        var ok = $('#registerForm').validate({
            invalidHandler: function (form, validator) {
                $(validator.invalidElements()[0]).focus();
            },
            focusInvalid: false,
            rules: {
                code: {
                    required: false
                },
                email: {
                    required: true,
                    email: true
                },
                password: {
                    required: true,
                    minlength: 6
                },
                passwordConfirm: {
                    required: true,
                    minlength: 6,
                    equalTo: "#password"
                }
            }
        }).form();
        if (!ok) {
            $('#registerButton').buttonEnable();
            return false;
        }
        // post form
        $.ajax({
            type: "POST",
            url: action,
            data: serialized,
            dataType: "json",
            success: function (json_data) {
                if (!json_data.success) {
                    $.jGrowl(json_data.message);
                }
                else {
                    window.location = json_data.data;
                }
                $('#registerButton').buttonEnable();
                return false;
            }
        });
        return false;
    });

    // input highlighters
    $("input,textarea").initInputSelectors();
    $('.loginPage', '#subdomainInput').unbind('focus.input').unbind('blur.input');
    $('.loginPage', '#subdomainInput')
                    .bind('focus.input', null, function () {
                        $(this).parent().addClass("curFocus");
                        $(this).siblings('#tradelr').css("color", "#000");
                        //$(this).addClass('bg_blue');
                    })
                    .bind('blur.input', null, function () {
                        $(this).parent().removeClass("curFocus");
                        //$(this).removeClass('bg_blue');
                        // only grey out if current input is empty
                        if ($(this).val() === '') {
                            $(this).siblings('#tradelr').css("color", "#666");
                        }
                    });
    $('#tradelr', '#subdomainInput').bind('click', function () {
        $('.loginPage', '#subdomainInput').focus();
    });

    if ($('.loginPage', '#subdomainInput').val() !== '') {
        $('#tradelr', '#subdomainInput').css("color", "#000");
    }

    var timer;
    // login name availability check
    $('.loginPage').keyup(function () {
        var selectedName = $('.loginPage').val();

        if (timer !== undefined) {
            clearTimeout(timer);
        }
        timer = setTimeout(function () {
            $.ajax({
                beforeSend: function () {
                    if (selectedName.length == 0) {
                        $('#nameAvail').html('');
                        return false;
                    }
                    return true;
                },
                type: "POST",
                url: "/register/DomainAvailable",
                data: "loginPage=" + selectedName,
                dataType: 'json',
                success: function (json_data) {
                    if (json_data.data) {
                        $('#nameAvail').html("<span class='ok_post'><strong>" + selectedName + "</strong> is available</span>");
                    }
                    else {
                        $('#nameAvail').html("<span class='error_post'><strong>" + selectedName + "</strong> is not available. Please select another name.</span>");
                    }
                }
            });
        }, 500);

    });
});