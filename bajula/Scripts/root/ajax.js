var tradelr = tradelr || {};
tradelr.ajax = {};
tradelr.ajax.queue = [];
tradelr.ajax.running = [];
(function ($) {
    // option to queue req
    $.ajaxManager = (function () {
        var MAX_CONCURRENT_REQUEST = 6;
        function add(req, abort) {
            if (abort == undefined) {
                abort = true;
            }
            if (abort && tradelr.ajax.queue.length != 0) {
                for (var i = 0; i < tradelr.ajax.queue.length; i++) {
                    ajaxqueue[i].abort();
                }
            }
            if (tradelr.ajax.running.length < MAX_CONCURRENT_REQUEST) {
                tradelr.ajax.running.push(req());
                tradelr.log('executing ..');
            }
            else{
                tradelr.ajax.queue.push(req);
                tradelr.log('add to queue. length: ' + tradelr.ajax.queue.length);
            }
        };

        function kill() {
            for (var i = 0; i < tradelr.ajax.running.length; i++) {
                var req = tradelr.ajax.running.pop();
                req.abort();
            }
        };

        function processqueue(){
            if(tradelr.ajax.queue.length != 0){
                var req = tradelr.ajax.queue.pop();
                tradelr.ajax.running.push(req());
                tradelr.log('process queue. length: ' + tradelr.ajax.queue.length);
            }
        };

        return {
            add: add,
            queuelength: function () {
                return tradelr.ajax.queue.length;
            },
            kill: kill,
            post: function (url, params, fn, type, hideLoader, dontQueue) {
                if (hideLoader == undefined) {
                    tradelr.hideLoader = true;
                }
                else {
                    tradelr.hideLoader = hideLoader;
                }
                var x = function(){
                    return $.ajax({
                        type: 'POST',
                        async: true,
                        url: url,
                        data: params,
                        complete: function () {
                            tradelr.hideLoader = false;
                            $.prettyLoader.hide();
                            tradelr.ajax.running.pop();
                            tradelr.log('ajax req complete');
                            processqueue();
                        },
                        success: fn,
                        dataType: type
                    });
                };
                if (dontQueue == undefined || dontQueue) {
                    tradelr.log('dontqueue');
                    tradelr.ajax.running.push(x());
                }
                else{
                    add(x, false);
                }
            }
        };
    })();

    // kill all when window unloads
    $(window).unload($.ajaxManager.kill);

})(jQuery);

tradelr.ajax.init = function () {
    $(document).ajaxError(function (event, request, settings) {
        $.each($('button.ajax'), function () {
            $(this).buttonEnable();
        });

        switch (request.status) {
            case 200:
                break;
            case 401:
                break;
            case 403:
                break;
            case 500:
                if (request.responseText.indexOf('A potentially dangerous Request.Form') != -1) {
                    $.jGrowl('Please check your input. Please use plain text (no HTML tags).');
                } else {
                    $.jGrowl('An error has occurred and has been noted. Try again or wait for 24 hours for us to resolve the problem.');
                }
                break;
            default:
                if (DEBUG) {
                    throw [request.status, ":", request.responseText].toString();
                }
                break;
        }

        $.prettyLoader.hide();
        tradelr.error(window.location.href + " : " + settings.url + " : " + request.statusText);
    });

    $(document).ajaxComplete(function (event, request, settings) {
        $.each($('button.ajax'), function () {
            $(this).buttonEnable();
        });

        switch (request.status) {
            case 200:
                break;
            case 401:
                $.jGrowl("Your session has expired. <a href='/login?redirect=" + encodeURIComponent(window.location.pathname) + "'>Click here to sign in</a>", { sticky: true });
                break;
            case 403:
                $.jGrowl("You don't have permission to do that");
                break;
            case 500:
                break;
            default:
                if (DEBUG) {
                    throw [request.status, ":", request.responseText].toString();
                }
                break;
        }
    });
};

tradelr.ajax.jsonp = function(url, callback) {
    var requesturl;
    if (url.indexOf('?') != -1) {
        requesturl = url + '&callback=?';
    }
    else {
        requesturl = url + '?callback=?';
    }

    $.getJSON(requesturl, function(json_result) {
        callback(json_result);
    });
};

tradelr.ajax.post = function (url, params, callback, type) {
    $.post(url, params, function (result) {
        if (result.success != undefined && !result.success) {
            if (result.data == tradelr.returncode.NOTLOGGEDIN) {
                $.jGrowl('You are not signed in');
                window.location = '/login?redirect=' + encodeURIComponent(window.location.pathname);
            }
            if (result.data == tradelr.returncode.NOPERMISSION) {
                $.jGrowl('You do not have permission to do that');
            }
            return false;
        }
        return callback(result);
    }, type);

};
