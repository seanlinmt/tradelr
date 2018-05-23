// passwordform
$('#LinkGetPass').click(function () {
    $(this).parent().fadeOut(function () {
        $('#passwordForm').fadeIn();
    });
    return false;
});

$('#buttonGetNewPass').click(function () {
    $(this).buttonDisable();
    $('#passwordForm').trigger('submit');
    return false;
});

$('#passwordForm').submit(function() {
    var serialized = $(this).serialize();
    var action = $(this).attr("action");

    var ok = $('#passwordForm').validate({
        rules: {
            myemail: {
                required: true,
                email: true
            }
        }
    }).form();

    if (!ok) {
        return false;
    }

    $.ajax({
        type: "POST",
        url: action,
        data: serialized,
        dataType: "json",
        success: function(json_data) {
            if (json_data.success) {
                $('.form_entry', '#passwordForm').html('An email has been sent to you with your new password');
            }
            else { 
                $.jGrowl(json_data.message);
            }
            return false;
        }
    });
    return false;
});

$(document).ready(function() {
    $('#myemail').watermark('enter your email');
    // input highlighters
    inputSelectors_init();
});