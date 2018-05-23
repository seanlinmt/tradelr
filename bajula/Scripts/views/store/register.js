$('#buttonRegister').click(function() {
    $(this).buttonDisable();
    $('#storeRegisterForm').trigger('submit');
});

$('#storeRegisterForm').submit(function() {
    var serialized = $(this).serialize();
    var action = $(this).attr("action");

    var ok = $('#storeRegisterForm').validate({
        rules: {
            email: {
                required: true,
                email: true
            },
            password: {
                required: true,
                minlength: 6
            }
        }
    }).form();

    if (!ok) {
        $('#buttonRegister').buttonEnable();
        return false;
    }

    $.ajax({
        type: "POST",
        url: action,
        data: serialized,
        dataType: "json",
        success: function(json_data) {
            if (json_data.success) {
                dialogBox_close();
                $.jGrowl('Your account has been created.');
                window.location = '/store/checkout';
            }
            else {
                $.jGrowl(json_data.message);
            }
            $('#buttonRegister').buttonEnable();
            return false;
        }
    });
    return false;
});

$(document).ready(function () {
    $('#firstName').focus().addClass('curFocus');
    init_autogrow('#storeRegisterForm');
    // input highlighters
    inputSelectors_init();

    $('#city').autocomplete('/city/find', {
        dataType: "json",
        parse: function (data) {
            var rows = new Array();
            if (data != null && data.length != null) {
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
        if (data) {
            $("#citySelected").val(data.id);
        }
    });
});