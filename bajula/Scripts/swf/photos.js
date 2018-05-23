function thm_clicked(ele) {
    if ($(ele).parents(".thumbnail").contents('.thm_overlay').length == 0) {
        $(ele).fadeTo(500, 0.5).addClass("thm_select");
        $(ele).parents(".thumbnail").append("<div class='thm_overlay'></div>");

    }
    else {
        $(ele).fadeTo(500, 1).removeClass("thm_select");
        $(ele).parents(".thumbnail").contents('.thm_overlay').remove();
    }
}



function addImage(data) {
    var info = data.split(',');
    var imageid = info[0];
    var targetid = info[1];
    var url = info[2];
    if ($('.nophoto', targetid).length != 0) {
        $('.nophoto', targetid).hide();
    }

    var thumbnail = "<div class='thumbnail' style='display:none'><img src='" + url +
            "' alt='" + imageid + "' /><div class='del'><span>delete</span></div></div>";
    // 3 locs to handle product, profile, company
    if (targetid !== "#product_images") {
        var existing = $(targetid + " > .results").find('.thumbnail');
        if($(existing).length == 0)
        {
            $(thumbnail).fadeIn().appendTo(targetid + " > .results");
        }
        else
        {
            $(existing).fadeOut('normal', function() {
            $(thumbnail).fadeIn().appendTo(targetid + " > .results");
            });
        }
    }
    else{
        $(thumbnail).fadeIn().appendTo(targetid + " > .results");
    }
    
    
}