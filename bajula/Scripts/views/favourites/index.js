$(document).ready(function () {
    $('#navstore').addClass('navselected_white');

    // bind side filter click event
    $('#categoryList').find('.sideboxEntry').live("click", function () {

        $('#categoryList').find('.sideboxEntry').removeClass('selected');
        $(this).addClass('selected');
        var fid = '';
        if ($(this).attr('fid') != undefined) {
            fid = $(this).attr('fid');
        }
        setFilterByField(fid);
        reloadFavouritesGrid(getFilterByField());
    });
    // init grid
    favBindToGrid(getFilterByField());

    // handle any actions
    $('#' + querySt('a')).trigger('click');
});