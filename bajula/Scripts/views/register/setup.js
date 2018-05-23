$(document).ready(function () {
    $('#buttonStart').click(function () {
        $(this).buttonDisable();
        $('#setupForm').trigger('submit');
    });

    $('#setupForm').submit(function () {
        var action = $(this).attr("action");
        var serialized = $(this).serialize();

        var ok = $('#setupForm').validate({
            invalidHandler: function (form, validator) {
                $(validator.invalidElements()[0]).focus();
            },
            focusInvalid: false,
            rules: {
                organisation: "required",
                country: "required",
                currency: "required",
                address: "required",
                city: "required",
                postcode: "required",
                phone: "required",
                tos: {
                    required: true
                }
            },
            messages: {
                tos: {
                    required: "You must agree to the Terms of Service to continue."
                }
            }
        }).form();
        if (!ok) {
            $('#buttonStart').buttonEnable();
            return false;
        }

        // post form
        tradelr.ajax.post(action, serialized, function(json_result) {
            if (json_result.success) {
                window.location = '/dashboard';
            }
            else {
                $.jGrowl(json_result.message);
            }
        }, 'json');
        return false;
    });

    $('#profile_address input,#profile_address select,#profile_address textarea').focus(function () {
        $('.help_message').fadeIn('fast');
    });

    $('.profile_others input,.profile_others select').focus(function () {
        $('.help_message').hide();
    });

    $('#city').autocomplete('/city/find', {
        dataType: "json",
        parse: function (data) {
            var rows = new Array();
            if (data.length != undefined && data.length != null) {
                for (var i = 0; i < data.length; i++) {
                    rows[i] = { data: data[i], value: data[i].title, result: data[i].title };
                }
            }
            return rows;
        },
        autoFill: true,
        formatItem: function (row, i, max) {
            return row.title;
        }
    });

    $("#city").bind('keyup', function () {
        $("#citySelected").val('');
    });

    $("#city").result(function (event, data, formatted) {
        if (data)
            $("#citySelected").val(data.id);
    });

    // input highlighters
    inputSelectors_init();

    init_autogrow('#setupForm');
});