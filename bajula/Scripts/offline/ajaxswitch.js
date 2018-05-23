/// <reference path="offline.js" />
/// <reference path="handlers/category.js" />
var tradelr = tradelr || {};
(function ($) {
    $.ajaxswitch = function (settings) {
        if (tradelr.webdb.db == null) {
            if (DEBUG) {
                tradelr.log("loading from online db");
            }
            $.ajax(settings);
        }
        else {
            if (DEBUG) {
                tradelr.log("loading from offline db");
                tradelr.log(settings);
            }
            var segments = settings.url.split('/');
            var target = '/' + segments[1] + '/' + segments[2];  // 0 = blank
            switch (target) {
                case '/dashboard/category/add':
                    var postObj = convertParamStringToObject(settings.data);
                    tradelr.category.add(postObj.categoryTitle, settings.success);
                    break;
                case '/dashboard/category/addsub':
                    var postObj = convertParamStringToObject(settings.data);
                    tradelr.category.addsub(postObj.categoryTitle, postObj.id, settings.success);
                    break;
                case '/dashboard/category/delete':
                    tradelr.category.remove(segments[3], settings.success);
                    break;
                case '/dashboard/category/getsub':
                    var catid = segments[segments.length - 1];
                    var ok = settings.beforeSend();
                    if (ok) {
                        tradelr.category.getsub(catid, settings.success);
                    }
                    break;
                case '/dashboard/inventory/locationDelete':
                    tradelr.inventory.location.remove(segments[3], settings.success);
                    break;
                case '/photos/delete':
                    tradelr.photo.remove(settings.data.ids, segments[3]);
                    break;
                case '/dashboard/product/list': // tested
                    // ignore for linked inventory page
                    if (!settings.data.supplier) {
                        settings.beforeSend();
                        var params = settings.data;
                        tradelr.product.list(params.cat, params.rows, params.page, params.sidx, params.sord, "", params.location, settings.complete);
                    }
                    else {
                        $.ajax(settings);
                    }
                    break;
                case '/dashboard/product/create':
                    tradelr.product.create(settings.data, settings.success);
                    break;
                case '/dashboard/product/delete':
                    tradelr.product.remove(segments[3], settings.data.ids, settings.success);
                    break;
                case '/dashboard/product/update': // tested
                    tradelr.product.update(settings.data, settings.success);
                    break;
                case '/dashboard/stockUnit/add':
                    var postObj = convertParamStringToObject(settings.data);
                    tradelr.stockunit.add(postObj.unitTitle, settings.success);
                    break;
                default:
                    $.jGrowl('Unable to complete action. You are not online.');
                    if (DEBUG) {
                        tradelr.log(target);
                    }
                    break;
            }
        }
        return this;
    };
})(jQuery);