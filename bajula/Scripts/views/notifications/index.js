$(document).ready(function () {
    $('#navhome').addClass('navselected_white');
});

$('.accept', '.notification_row').live('click', function () {
    var container = $(this).parents('.notification_row');
    var id = container.attr('alt');
    $.post('/dashboard/contacts/requestaccept/' + id, function (json_result) {
        if (json_result.success) {
            container.fadeOut('normal', function () {
                $(this).remove();
            });
        }
        else {
            $.jGrowl(json_result.message);
        }
    });
    return false;
});

$('.ignore', '.notification_row').live('click', function () {
    var container = $(this).parents('.notification_row');
    var id = $(this).parents('.notification_row').attr('alt');
    $.post('/dashboard/contacts/requestignore/' + id, function (json_result) {
        if (json_result.success) {
            container.fadeOut('normal', function () {
                $(this).remove();
            });
        }
        else {
            $.jGrowl(json_result.message);
        }
    });
    return false;
});
