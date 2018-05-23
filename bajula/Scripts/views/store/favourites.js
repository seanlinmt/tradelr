$(document).ready(function () {

    $('.item .del').bind('click', function () {
        var confirm = window.confirm('Remove favourite?');
        if (!confirm) {
            return;
        }
        var current = this;
        var id = $(this).parents('.item').find('.id').html();
        $.post('/favourites/delete/' + id, null, function (json_result) {
            if (json_result.success) {
                $.jGrowl(json_result.message);
                $(current).parents('.item').fadeOut(function () {
                    $(this).remove();
                });
            }
            else {
                $.jGrowl(json_result.message);
            }
        }, 'json');
    });
});