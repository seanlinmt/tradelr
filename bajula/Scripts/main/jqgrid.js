function equaliseHeight(context) {
    if ($('.content_filter', context).length == 0) {
        return;
    }
    $('.ui-jqgrid', context).height('auto');
    var sideHeight = $('.content_filter', context).height();
    var gridheight = $('.ui-jqgrid', context).height();
    if (sideHeight > gridheight) {
        $('.ui-jqgrid', context).height(sideHeight);
    }
};

function getFilterByField(context) {
    if (context == undefined) {
        return $.trim($("#filterBy").html());
    }
    return $.trim($("#filterBy", context).html());
};

function setFilterByField(val, context) {
    if (context == undefined) {
        $("#filterBy").html(val);
    }
    $("#filterBy", context).html(val);
};

function getMatchingFilterButton(listid, filterid) {
    if (filterid === "") {
        return $(listid).find('div:first');
    }

    var target = "div[fid='" + filterid + "']";
    var button = $(listid).find(target);

    if (button.length !== 1) {
        button = $(listid).find('div:first');
    }
    
    return button;
};


function init_jqgrids(context) {
    $('#jqgh_total,#jqgh_subtotal,#jqgh_costPrice,#jqgh_groupprice').addClass('ar');
    $('#jqgh_status,#jqgh_supplierInStock,#jqgh_orderDate,#jqgh_favTotal,#jqgh_hits,#jqgh_invtotal').addClass('ac');
    equaliseHeight(context);
};

function sideSelector_render(gridid, context, thumbnail, title, details, identifier, checked) {
    if (checked) {
        if ($('#sideSelected', context).children().length == 0) {
            // first entry
            $('#sideSelected', context).append('<h4>selected</h4>');
        }

        // check that entry is not already selected
        var exist = $("#sideSelected > .entry[alt=" + identifier + "]", context);
        if (exist.length != 0) {
            return;
        }

        var entry = "<div class='entry' alt='" + identifier + "'><span class='thm'><img src='" + thumbnail + "'/></span><span class='title' title='" + details + "'>" + title +
                "</span><span class='icon'><span class='delIcon'></span></span></div>";
        $('#sideSelected', context).append(entry);
        $('#sideSelected', context).slideDown();
    }
    else {
        // look for entry will matching identifier to remove, otherwise just ignore
        var entry = $('#sideSelected .entry[alt=' + identifier + ']', context);
        if (entry.length != 0) {
            $(gridid, context).find('tr[id=' + identifier + ']').removeClass('selected');
            $(entry).fadeOut('fast', function () {
                $(this).remove();
                if ($('#sideSelected', context).children().length == 1) { // last entry
                    $('#sideSelected', context).slideUp();
                }
            });
        }
    }
};

function sideSelector_delete(entryindex, context) {
    $("#sideSelected .entry[alt='" + entryindex + "']", context).slideUp('fast', function () {
        $(this).remove();
        if ($('#sideSelected', context).children().length == 1) { // last entry
            $('#sideSelected', context).slideUp();
        }
    });
}


function sideSelector_bindDelete(gridid) { 
    // bind selected stuff
    $('#sideSelected .delIcon').live('click', function () {
        var id = $(this).parents('.entry').attr('alt');
        $(this).parents('.entry').fadeOut('fast', function () {
            $(this).remove();

            // uncheck checkbox
            $(gridid).find('tr[id=' + id + '] input:checkbox').attr('checked', false);
            $(gridid).find('tr[id=' + id + ']').removeClass('selected-row');
            if ($('#sideSelected').children().length == 1) { // last entry
                $('#sideSelected').slideUp();
            }
        });
    });
};




    