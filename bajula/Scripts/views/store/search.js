$(document).ready(function () {
    var totalPages = $('#totalProductPages').val();
    if (totalPages > 1) {
        $("#products_pagination").paginate({
            count: totalPages,
            start: 1,
            display: 10,
            border: false,
            text_color: '#79B5E3',
            background_color: 'none',
            text_hover_color: '#2573AF',
            background_hover_color: 'none',
            images: false,
            mouse: 'press',
            onChange: function (page) {
                $.scrollTo('#store_header', 800);
                $.post('/store/search', { term: $('#searchterm').val(), page: page }, function (json_result) {
                    if (json_result.success) {
                        $('#products', '#store_body_content').html(json_result.data);
                        $('.galleryThumbnail').each(function () {
                            $(this).append('<div class="ajaxin"></div>');
                            loadImageAjaxly(this);
                        });
                    }
                    else {
                        $.jGrowl(json_result.message);
                    }
                });
            }
        });
    }

    $('.galleryThumbnail').each(function () {
        $(this).append("<div class=\"ajaxin\"></div>");
        loadImageAjaxly(this);
    });
});

