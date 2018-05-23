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
                var url = '';
                var storepath = window.location.pathname;
                if (storepath.indexOf('/store/browse') != -1 || storepath.indexOf('/store') != -1) {
                    var url = '/store/browse/' + $('#mainCategory').val() + '/' +
                                                $('#subCategory').val() + '/' + page;
                }
                else {
                    var url = '/store/tags/' + $('#currenttag').val() + '/' + page;
                }
                $.scrollTo('#products', 800);
                $.get(url, function (result) {
                    $('#products', '#store_body_content').html(result);
                    $('.galleryThumbnail').each(function () {
                        $(this).append('<div class="ajaxin"></div>');
                        loadImageAjaxly(this);
                    });
                });
            }
        });
    }

    $('.galleryThumbnail').each(function () {
        $(this).append("<div class='ajaxin'></div>");
        loadImageAjaxly(this);
    });

    $('.galleryBox').each(function () {
        if ($.trim($('.price', this).html()) == '') {
            $('.priceDetails', this).hide();
        }
    });
});

