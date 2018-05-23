var tradelr = tradelr || {};
tradelr.tabs = {};
tradelr.tabs.last_index = 0;
tradelr.tabs.current_index = 0;
tradelr.tabs.add = function (url, id, name, context) {
    if ($(id, context).length == 0) {
        $(context).tabs("add", id, name);
        $("a[href='" + id + "']", context).after("<span class='mr5 hover_del'></span>");
    }
    $(context).tabs("select", id);
    scrollToTop();
    $(id).showLoadingBlock();
    $.post(url, function (result) {
        $(id).html(result);
    });

};

tradelr.tabs.del = function (self, context) {
    var idx = $(self).parents('li').index();
    $(context).tabs("remove", idx);

    $(context).tabs("select", tradelr.tabs.last_index);
    return false;
};

tradelr.tabs.init = function(context) {
    $(context).tabs({
        show: function (event, ui) {
            tradelr.tabs.last_index = tradelr.tabs.current_index;
            tradelr.tabs.current_index = ui.index;
        }
    });

    $('.hover_del', '.ui-tabs-nav').live('click', function () {
        tradelr.tabs.del(this, context);
        return false;
    });
};