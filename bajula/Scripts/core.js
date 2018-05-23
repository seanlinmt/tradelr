var tradelr = tradelr || {};
tradelr.constants = {};
tradelr.constants.facebook = {};
tradelr.constants.facebook.GRAPH_URL = "https://graph.facebook.com/";var tradelr = tradelr || {};
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
window.onerror = function (em, url, ln) {
    var msg = $.toJSON(em) + ", " + url + ", " + ln;

    var getBrowserString = function () {
        var browser = [];
        $.each(jQuery.browser, function (i, val) {
            browser.push(i + " : " + val);
        });
        return browser.toString();
    };
    msg = window.location.toString() + " (" + getBrowserString() + ") :" + escape(msg);
    $.prettyLoader.hide();
    tradelr.error(msg);
    return false;
};


var tradelr = tradelr || {};
tradelr.preverr = '';
tradelr.hideLoader = false;
tradelr.error = function (msg) {
    var st = '';
    //st = printStackTrace();
    var err = tradelr.util.stripTags(msg);
    if (err != tradelr.preverr) {
        tradelr.preverr = err;
        $.post('/error/log/', { message: err }); ;
    }
};

tradelr.error.ajax_handler = function(json_result)
{
    switch (json_result.data) {
        case tradelr.returncode.NOTLOGGEDIN:
            window.location = '/login';
            break;
        case tradelr.returncode.NOPERMISSION:
            $.jGrowl('You do not have permission to do that');
            break;
        default:
            $.jGrowl(json_result.message);
            break;
    }
};

tradelr.log = function(data) {
    if (typeof(console) != 'undefined' && typeof(console.log) == 'function' && DEBUG) {
        console.log(data);
    }
};

tradelr.util = {};

tradelr.util.buildUrl = function(url, parameters) {
    var qs = "";
    for (var key in parameters) {
        var value = parameters[key];
        qs += encodeURIComponent(key) + "=" + encodeURIComponent(value) + "&";
    }
    if (qs.length > 0) {
        qs = qs.substring(0, qs.length - 1); //chop off last "&"
        url = url + "?" + qs;
    }
    return url;
};

tradelr.util.fixselect = function (id, context) {
    if (context == undefined) {
        $(id).width($(id).width() + 14);
    }
    else {
        $(id, context).width($(id, context).width() + 14);
    }
};

tradelr.util.image_scale = function (image, width, height) {
    if (image.offsetWidth > image.offsetHeight) {
        if (image.offsetWidth > width) {
            image.height = image.offsetHeight * width / image.offsetWidth;
            image.width = width;
        }
    }
    else {
        if (image.offsetHeight > height) {
            image.width = image.offsetWidth * height / image.offsetHeight;
            image.height = height;
        }
    }
};

tradelr.util.is_image = function (filename) {
    var imgTypes = ["jpg", "gif", "png", "jpeg"];
    var ext = filename.toLowerCase().split(".").pop();

    if (imgTypes.indexOf(ext) == -1) {
        return false;
    }
    return true;
};

tradelr.util.convertdistance = function (value, toMetric) {
    if (toMetric) {
        return roundNumber(2.54 * value, 1);
    }
    return roundNumber(value / 2.54, 1);
};

tradelr.util.convertweight = function (value, toMetric) {
    if (toMetric) {
        return roundNumber(0.4536 * value, 1);
    }
    return roundNumber(2.2046 * value, 1);
};

tradelr.util.overlay = function (id, add) {
    if (add) {
        $(id).append("<div class='ui-widget-overlay-white'></div>");
    }
    else {
        $('.ui-widget-overlay-white', id).remove();
    }
};

tradelr.util.stripNonNumeric = function(str) {
    str += '';
    var rgx = /^\d|\.|-$/ ;
    var out = '';
    for (var i = 0; i < str.length; i++) {
        if (rgx.test(str.charAt(i))) {
            if (!((str.charAt(i) == '.' && out.indexOf('.') != -1) ||
                (str.charAt(i) == '-' && out.length != 0))) {
                out += str.charAt(i);
            }
        }
    }
    return out;
};

tradelr.util.tofloat = function (text) {
    var parsed = parseFloat(text);
    if (isNaN(parsed)) {
        return 0;
    }
    return parsed;
};

tradelr.util.tomoneystring = function (value, decimalcount) {
    if (isNaN(parseFloat(value))) {
        return value;
    }
    value = value.toFixed(decimalcount);
    var delimiter = ","; // replace comma if desired
    var d;
    var i;
    var a;
    if (value.indexOf('.') == -1) {
        d = '';
        i = value;
    }
    else {
        a = value.split('.', 2);
        d = a[1];
        i = parseInt(a[0]);
    }

    if (isNaN(i)) { return ''; }
    var minus = '';
    if (i < 0) { minus = '-'; }
    i = Math.abs(i);
    var n = new String(i);
    a = [];
    while (n.length > 3) {
        var nn = n.substr(n.length - 3);
        a.unshift(nn);
        n = n.substr(0, n.length - 3);
    }
    if (n.length > 0) { a.unshift(n); }
    n = a.join(delimiter);
    if (d.length < 1) { value = n; }
    else { value = n + '.' + d; }
    value = minus + value;
    return value;
};

tradelr.util.trimLeadingZeros = function (s) {
    while (s.substr(0, 1) == '0' && s.length > 1) {
        s = s.substr(1);
    }
    return s;
};

tradelr.util.stripTags = function (s) {
    return s.replace(/<&#91;^>&#93;*>/g, "");
};

$(document).ready(function () {
    $.fn.extend({
        appendable: function (url, text, width) {
            var select = this;
            $(select).append("<option value=''></option><option class='addNewOption' value='-100'>" + text + "</option>");
            $(select).bind('change', function () {
                if ($(select).val() != '-100') {
                    return;
                }
                dialogBox_open(url, width);
            });
        },
        appendableRedirect: function (url, text, giveWarning) {
            var select = this;
            $(select).append("<option value=''></option><option class='addNewOption' value='-100'>" + text + "</option>");
            $(select).bind('click', function () {
                if ($(select).val() != '-100') {
                    return false;
                }
                if (giveWarning === undefined || giveWarning === true) {
                    var leave = window.confirm('Are you sure you want to add a new entry? You will lose unsaved changes.');
                    if (leave) {
                        window.location = url;
                    }
                    else {
                        $(select).val('');
                        return false;
                    }
                }
                else {
                    window.location = url;
                }
            });
        },
        buttonEnable: function () {
            $(this).removeAttr('disabled');
        },
        buttonDisable: function () {
            $(this).attr('disabled', 'disabled');
        },
        buttonAjaxify: function () {
            $.each(this, function () {
                $(this).bind("ajaxSend", function (event, XMLHttpRequest, ajaxOptions) {
                    $(this).attr('disabled', 'disabled');
                    $(this).after("<img align='absmiddle' src='/Content/img/loading.gif' />");
                }).bind("ajaxComplete", function (event, XMLHttpRequest, ajaxOptions) {
                    $(this).removeAttr('disabled');
                    $(this).next().remove();
                });
            });
        },
        capitaliseFirstLetter: function () {
            var caped = $(this).val().charAt(0).toUpperCase() + $(this).val().slice(1);
            $(this).val(caped);
        },
        // select dropdownlist option with value=""
        defaultOption: function () {
            $(this).children("option:first").attr('selected', 'selected');
        },
        enableGridLines: function () {
            var rows = $(this).find('tr');
            $.each(rows, function () {
                $(this).find('td:not(td:last)').addClass('br');
            });
        },
        history: function (tab) {
            $.historyInit(function (hash) {
                if (hash) {
                    tab.tabs('select', '#' + hash);
                }
                else {
                    tab.tabs('select', 0);
                }

            }, window.location.pathname);
            $(this).click(function () {
                var hash = this.href;
                hash = hash.replace(/^.*#/, '');
                $.historyLoad(hash);
                return false;
            });
            return this;
        },
        initInputSelectors: function (context) {
            if (context == undefined) {
                return this.each(function () {
                    $(this).bind('focus.input', null, function () {
                        $(this).addClass("curFocus");
                    });
                    $(this).bind('blur.input', null, function () {
                        $(this).removeClass("curFocus");
                    });
                });
            }
            else {
                return this.each(function () {
                    $(this, context).bind('focus.input', null, function () {
                        $(this).addClass("curFocus");
                    });
                    $(this, context).bind('blur.input', null, function () {
                        $(this).removeClass("curFocus");
                    });
                });
            }
        },
        insertOption: function (text, val) {
            $(this).children('option:first')
                    .after("<option value='" + val + "'>" + text + "</option>");
            $(this).children('option:eq(1)').attr('selected', 'selected');
        },
        insertTextArea: function (posturl, width, insertButtonText, tip, limit, callback) {
            var self = this;
            var id = new Date().getTime();
            var template = $("<form id='form_" + id + "' method='post'><span class='tip'></span><textarea name='comment' class='w100p'></textarea><div class='charsleft fr'><span id='" + id +
            "_charsleft'></span></div><div class='fl mt10'><button id='" + id + "_add' type='button' class='ajax green'><button id='" + id + "_cancel' type='button'>cancel</button></div><div class='clear'></div></form>");

            $(template).attr('action', posturl);
            $(template).css('width', width);
            if (insertButtonText == undefined || insertButtonText == '') {
                insertButtonText = 'save';
            }
            $('#' + id + '_add', template).text(insertButtonText);

            if (tip == undefined || tip == '') {
                tip = '';
            }
            $('.tip', template).html(tip);

            if (limit != undefined && limit != '') {
                $('textarea', template).limit(limit, '#' + id + '_charsleft');
            }

            // insert into DOM
            $(self).slideUp(function () {
                $(self).after(template);

                // handle events
                inputSelectors_init('#form_' + id);
                init_autogrow('#form_' + id);

                $('#' + id + '_add', '#form_' + id).click(function () {
                    $(this).trigger('submit');
                });
                $('#' + id + '_cancel', '#form_' + id).click(function () {
                    $(this).parents('form').slideUp(function () {
                        $(self).slideDown();
                        $(this).remove();
                    });
                });

                $('#form_' + id).submit(function () {
                    var ok = $(this).validate({
                        invalidHandler: function (form, validator) {
                            $(validator.invalidElements()[0]).focus();
                        },
                        focusInvalid: false,
                        rules: {
                            comment: {
                                required: true
                            }
                        }
                    }).form();
                    if (!ok) {
                        return false;
                    }
                    $('#' + id + '_add', '#form_' + id).buttonDisable();

                    var serialized = $(this).serialize();
                    var action = $(this).attr("action");


                    $.ajax({
                        type: "POST",
                        url: action,
                        data: serialized,
                        success: function (result) {
                            var success = callback(result);
                            if (success) {
                                $('#' + id + '_cancel', '#form_' + id).trigger('click');
                            }
                            return false;
                        }
                    });
                    return false;
                });
            });
        },
        maxwidth: function (maxvalue) {
            var width = $(this).width();
            if (width > maxvalue) {
                $(this).width(maxvalue);
            }
        },
        notify: function (msg) {
            $(this).bar({
                color: '#1E90FF',
                background_color: '#eee',
                removebutton: true,
                message: msg,
                time: 4000
            });
            return false;
        },
        preventChange: function () {
            var oldVal = $(this).val();
            $(this).bind('change', function () {
                $.jGrowl('You cannot change this value');
                $(this).val(oldVal);
                return false;
            });
        },
        removeIn: function (miliseconds) {
            var element = this;
            setTimeout(function () {
                $(element).slideUp('fast', function () {
                    $(this).remove();
                });
            }, miliseconds);
        },
        removeLoading: function () {
            $('.loader', this).remove();
        },
        resetSelected: function () {
            $(this).find('button').removeClass('selected');
        },
        serializeObject: function () {
            var o = {};
            var a = this.serializeArray();
            $.each(a, function () {
                if (o[this.name]) {
                    if (!o[this.name].push) {
                        o[this.name] = [o[this.name]];
                    }
                    // don't include property if no value
                    if (this.value !== '') {
                        o[this.name].push(this.value || '');
                    }
                } else {
                    if (this.value !== '') {
                        o[this.name] = this.value || '';
                    }
                }
            });
            return o;
        },
        showLoading: function (msg) {
            if (msg == null) {
                this.each(function () {
                    $(this).html("<img class='loader' align='absmiddle' src='/Content/img/loading.gif' />");
                });
            }
            else {
                $(this).html("<img class='loader' align='absmiddle' src='/Content/img/loading.gif' /> " + msg);
            }
        },
        showLoading2: function () {
            $(this).html("<img class='loader' align='absmiddle' src='/Content/img/loader/loader_facebook.gif' />");
        },
        showLoadingBlock: function () {
            return this.each(function () {
                if ($('.loader', this).length == 0) {
                    $(this).append("<div class='loader ac pad5 clear'><img src='/Content/img/loading_circle.gif' /></div>");
                }
            });
        },
        showLoadingFacebook: function (size, append) {
            if (size == undefined || size == null) {
                size = 'medium';
            }
            if (append == undefined) {
                append = false;
            }
            return this.each(function () {
                if (append) {
                    $(this).append("<img class='loader' align='absmiddle' src='/Content/img/facebook/indicator_blue_" + size + ".gif' />");
                }
                else {
                    $(this).html("<img class='loader' align='absmiddle' src='/Content/img/facebook/indicator_blue_" + size + ".gif' />");
                }
            });
        },
        textWidth: function () {
            var sensor = $('<div />').css({ margin: 0, padding: 0 });
            $(this).append(sensor);
            var width = sensor.width();
            sensor.remove();
            return width;
        },
        trackUnsavedChanges: function (resetButtonID) {
            isDirty = false;
            $('input, textarea', this).change(function () {
                isDirty = true;
            });
            window.onbeforeunload = function () {
                if (isDirty) {
                    return 'You have unsaved changes.';
                }
            };
            $(resetButtonID).click(function () {
                isDirty = false;
            });
        },
        updateText: function (msg) {
            $(this).slideUp(function () {
                $(this).text(msg).slideDown()
                    .animate({ color: "#000" }, 'fast');
            });
        }
    });
    tradelr.ajax.init();
    init_misc();
    /*
    if (typeof init_gears != "undefined") { 
    init_gears();
    }
    */
});


function colorChange(original, delta) {
    var originalValue = parseInt(original.substring(1), 16);
    var newValue = originalValue + delta;
    return '#' + newValue.toString(16);
}

function convertParamStringToObject(params) {
    var postObj = {};
    var paramsArray = params.split("&");
    for (var i = 0; i < paramsArray.length; i++) {
        var keyvalues = paramsArray[i].split("=");
        var key = keyvalues[0];
        var value = unescape(escapeSpace(keyvalues[1]));
        if (postObj[key] == undefined) {
            postObj[key] = value;
        }
        else {
            var currentval = postObj[key];
            if (currentval instanceof Array) {
                currentval.push(value);
                postObj[key] = currentval;
            }
            else {
                var newArray = [];
                newArray.push(postObj[key]);
                newArray.push(value);
                postObj[key] = newArray;
            }
        }
    }
    if (DEBUG) {
        tradelr.log(postObj);
    }
    return postObj;
}

tradelr.date = {};
tradelr.date.getLocalTime = function() {
    var current_date = new Date();
    var offset = current_date.getTimezoneOffset() * 60;
    return current_date.valueOf() / 1000 + offset;
};

tradelr.date.getTimezoneOffset = function() {
    return new Date().getTimezoneOffset();
};

function dialogBox_close() {
    $('#ajax_dialog').dialog('destroy');
    $('#ajax_dialog').remove();
    $('body').css('overflow-y', 'scroll');
    return false;
}

function dialogBox_open(url, w, h, fn) {
    if (w == undefined || w == '' || w == null) {
        w = 400;
    }
    if (h == undefined || h == '' || h == null){
        h = 'auto';
    }

    $.ajax({
        type: "GET",
        url: url,
        success: function (result) {
            if (typeof (result) != 'string' && !result.success) {
                return tradelr.error.ajax_handler(result);
            }
            
            if ($('#ajax_dialog').length == 0) {
                $('body').append("<div id='ajax_dialog'><div id='ajax_content'></div></div>");
            }
            //$('body').css('overflow', 'hidden');
            $('#ajax_content').html(result);
            $('#ajax_dialog').dialog({
                autoOpen: false,
                closeOnEscape: false,
                draggable: false,
                modal: true,
                resizable: false,
                open: function (event, ui) {
                    $(".ui-dialog-titlebar").hide();
                    if (fn != undefined && typeof (fn) == "function") {
                        fn();
                    }
                },
                overlay: { background: "white" },
                width: w,
                height: h,
                zIndex: 1000
            });
            return $('#ajax_dialog').dialog("open");
        }
    });
};

function dialogBoxTitle_open(url, title, w, modal) {
    if (w == undefined) {
        w = 400;
    }
    if (modal == undefined) {
        modal = false;
    }
    
    $.ajax({
        type: "GET",
        url: url,
        success: function (result) {
            // should never be a json result unless we are not logged in
            if (typeof (result) != 'string' && !result.success) {
                return tradelr.error.ajax_handler(result);
            }
            if ($('#ajax_dialog').length == 0) {
                $('body').append("<div id='ajax_dialog'><div id='ajax_content'></div></div>");
            }
            $('#ajax_content').html(result);
            $('#ajax_dialog').dialog({
                autoOpen: false,
                closeOnEscape: true,
                draggable: true,
                modal: modal,
                resizable: false,
                overlay: { background: "white" },
                width: w,
                height: 'auto',
                title: title,
                zIndex: 1000
            });
            return $('#ajax_dialog').dialog("open");
        }
    });
};


function dialogBox_show(message) {
    $('#container').append("<div id='ajax_msg_dialog'><div id='ajax_msg_content'></div></div>");
    $('#ajax_msg_content').html(message);
    $('#ajax_msg_dialog').dialog({
        buttons: {
            "Ok": function () {
                $(this).dialog("close");
            }
        },
        autoOpen: false,
        closeOnEscape: false,
        draggable: true,
        modal: true,
        resizable: false,
        open: function (event, ui) { $(".ui-dialog-titlebar").hide(); },
        overlay: { background: "white" },
        width: 400,
        height: 'auto',
        zIndex:2000
    });
    $('#ajax_msg_dialog').dialog("open");
}

function inputSelectors_uninit(context) {
    $("input,textarea", context).unbind('focus.input');
    $("input,textarea", context).unbind('blur.input');
}

function escapeSpace(val) {
    return val.replace(/\+/g, ' ');
}

function getCookie(c_name) {
    if (document.cookie.length > 0) {
        c_start = document.cookie.indexOf(c_name + "=");
        if (c_start != -1) {
            c_start = c_start + c_name.length + 1;
            c_end = document.cookie.indexOf(";", c_start);
            if (c_end == -1) c_end = document.cookie.length;
            return unescape(document.cookie.substring(c_start, c_end));
        }
    }
    return "";
}

function getPageID() {
    var segments = window.location.pathname.split('/');
    return segments[segments.length - 1];
}

function getUrlHash() {
    var hash = window.location.href;
    if (!hash.match('#'))
        return undefined;
    hash = hash.replace(/^.*#/, '');
    return hash;
}

function google_loadApi(apikey, callback) {
    var script = document.createElement("script");
    script.src = document.location.protocol + "//www.google.com/jsapi?key=" + apikey + "&callback=" + callback;
    script.type = "text/javascript";
    document.getElementsByTagName("head")[0].appendChild(script);
}

function init_misc() {
    // init all cancel buttons
    $(".cancelDialogBox").live("click", function () {
        $('#ajax_dialog').dialog('close');
        return false;
    });

    // init ajax buttons
    //does not work because cannot select target of ajax call yet, maybe in jquery 1.4
    //$('button.ajax').buttonAjaxify();

    // init selectable rows
    $(".blockSelectable").live("click", function () {
        if ($(this).hasClass('selected')) {
            $(this).animate({ backgroundColor: "#ffffff", color: "#000000" }, 'normal').removeClass('selected');
            $(this).find('span').animate({ color: "#aaaaaa" }, 'normal');
        }
        else {
            $(this).animate({ backgroundColor: "#65affb", color: "#ffffff" }, 'normal', null, function () {
                $(this).addClass('selected');
            });
            $(this).find('span').animate({ color: "#ffffff" }, 'normal');
        }
    });

    // init closeable jgrowl
    $('.jGrowl-notification').live('click', function () {
        $(this).trigger('jGrowl.close');
    });
    $.prettyLoader();
    /*
    var cacheStatusValues = [];
    cacheStatusValues[0] = 'uncached';
    cacheStatusValues[1] = 'idle';
    cacheStatusValues[2] = 'checking';
    cacheStatusValues[3] = 'downloading';
    cacheStatusValues[4] = 'updateready';
    cacheStatusValues[5] = 'obsolete';

    function logEvent(e) {
    var online, status, type, message;
    online = (navigator.onLine) ? 'yes' : 'no';
    status = cacheStatusValues[cache.status];
    type = e.type;
    message = 'online: ' + online;
    message += ', event: ' + type;
    message += ', status: ' + status;
    if (type == 'error' && navigator.onLine) {
    message += ' (probably a syntax error in manifest)';
    }
    tradelr.log(message);
    }
    
    var cache = window.applicationCache;
    cache.addEventListener('cached', logEvent, false);
    cache.addEventListener('checking', logEvent, false);
    cache.addEventListener('downloading', logEvent, false);
    cache.addEventListener('error', logEvent, false);
    cache.addEventListener('noupdate', logEvent, false);
    cache.addEventListener('obsolete', logEvent, false);
    cache.addEventListener('progress', logEvent, false);
    cache.addEventListener('updateready', logEvent, false);
    */
    if ($('html').attr('manifest')) {
        var cache = window.applicationCache;

        cache.addEventListener(
            'updateready',
            function () {
                cache.swapCache();
                //window.location.reload();
                $.jGrowl('Offline cache updated.', { sticky: true });
            },
            false
        );

        cache.addEventListener(
            'error',
            function () {
                if (DEBUG) {
                    tradelr.log('no updates to offline cache');
                }
            },
            false
        );

        if (navigator.onLine && cache.status == 1) {
            if (DEBUG) {
                tradelr.log('updating cache');
            }
            //cache.update();
        }
    }

    // prototype fixes
    if (!Array.prototype.indexOf) {
        Array.prototype.indexOf = function (obj, start) {
            for (var i = (start || 0), j = this.length; i < j; i++) {
                if (this[i] === obj) {
                    return i;
                }
            }
            return -1;
        };
    }
}

function init_autogrow(context, height) {
    var minval = '50px';
    if (height != null) {
        minval = height + 'px';
    }
    
    $("textarea", context).autogrow({ minHeight: minval });
}

function inputSelectors_init(context) {
    if (context == undefined) {
        context = 'body';
    }
    $('input[type="text"],input[type="password"],textarea', context).bind('focus.input', null, function () {
        $(this).addClass("curFocus");
    });
    $('input[type="text"],input[type="password"],textarea', context).bind('blur.input', null, function () {
        $(this).removeClass("curFocus");
    });
}

function isAlphaNumeric(e) {
    var key;
    if (window.event) {
        key = window.event.keyCode;     //IE
    }
    else {
        key = e.which;     //firefox
    }
    if ((key >= 65 && key <= 90) ||
    (key >= 48 && key <= 57) ||
      (key >= 96 && key <= 105) ||
      (key == 8) || (key == 46) ||
      (key >= 37 && key <= 40))
        return true;

    return false;
}


function isEnterKey(e)
{
    var key;
    if(window.event)
    {
        key = window.event.keyCode;     //IE
    }
    else
    {
        key = e.which;     //firefox
    }
    if (key == 13)
        return true;
    
    return false;
}


function isNumeric(e) {
    var key;
    if (window.event) {
        key = window.event.keyCode;     //IE
    }
    else {
        key = e.which;     //firefox
    }
    if ((key >= 48 && key <= 57) ||
      (key >= 96 && key <= 105))
        return true;

    return false;
}


function loadImageAjaxly(element) {
    var img = new Image();
    $(img).load(function () {
        //$(this).css('display', 'none'); // .hide() doesn't work in Safari when the element isn't on the DOM already
        $(this).hide();
        $(element).find('.ajaxin').remove();
        $(element).append(this);
        $(this).fadeIn();
    }).error(function () {
        $(this).attr('src', '/Content/img/thumbs_none_medium.png').show();
    }).attr('src', $(element).attr('alt'));
}

function makeClosure(x) {
    return function () {
        return x;
    }
}

function PadDigits(n, totalDigits) {
    n = n.toString();
    var pd = '';
    if (totalDigits > n.length) {
        for (var i = 0; i < (totalDigits - n.length); i++) {
            pd += '0';
        }
    }
    return pd + n.toString();
}

function parseUrl(data) {
    var e = /((http|ftp):\/)?\/?([^:\/\s]+)((\/\w+)*\/)([\w\-\.]+\.[^#?\s]+)(#[\w\-]+)?/;

    if (data.match(e)) {
        return { url: RegExp['$&'],
            protocol: RegExp.$2,
            host: RegExp.$3,
            path: RegExp.$4,
            file: RegExp.$6,
            hash: RegExp.$7
        };
    }
    else {
        return { url: "", protocol: "", host: "", path: "", file: "", hash: "" };
    }
}

function querySt(ji) 
{
    var hu = window.location.search.substring(1);
    var gy = hu.split("&");
    for (var i = 0; i < gy.length; i++) 
    {
        var ft = gy[i].split("=");
        if (ft[0] == ji) 
        {
            return ft[1];
        }
    }
    return "";
};

function replaceURLWithHTMLLinks(text) {
    var exp = /(\b(https?|ftp|file):\/\/[-A-Z0-9+&@#\/%?=~_|!:,.;]*[-A-Z0-9+&@#\/%=~_|])/ig;
    return text.replace(exp, "<a target='_blank' href='$1'>$1</a>");
}

function resendEmailVerification() {
    $.post('/email/verification');
    $.jGrowl('Email sent');
    return false;
}

function rgbConvert(str) 
{
    if (str.indexOf('#') != -1)
        return str;
        
   str = str.replace(/rgb\(|\)/g, "").split(",");
   str[0] = parseInt(str[0], 10).toString(16).toLowerCase();
   str[1] = parseInt(str[1], 10).toString(16).toLowerCase();
   str[2] = parseInt(str[2], 10).toString(16).toLowerCase();
   str[0] = (str[0].length == 1) ? '0' + str[0] : str[0];
   str[1] = (str[1].length == 1) ? '0' + str[1] : str[1];
   str[2] = (str[2].length == 1) ? '0' + str[2] : str[2];
   return ('#' + str.join(""));

}

function roundNumber(num, dec) {
    var result = Math.round(num * Math.pow(10, dec)) / Math.pow(10, dec);
    return result;
}

function scrollToTop() {
    $('html').animate({ scrollTop: 0 }, 'normal');
}
        
function stopScrolling() {
    var scroll_to_x = 0;
    var scroll_to_y = 0;
    $('.ui-tabs-nav').bind('tabsselect', function(event, ui) {
        scroll_to_x = window.pageXOffset;
        scroll_to_y = window.pageYOffset;
    });
    $('.ui-tabs-nav').bind('tabsshow', function(event, ui) {
        window.scroll(scroll_to_x, scroll_to_y);
    });

}

function supports_canvas() {
    return !!document.createElement('canvas').getContext;
}

function supports_local_storage() {
    return !!window.localStorage;
}

function supports_session_storage() {
    return !!window.sessionStorage;
}

function supports_web_workers() {
    return !!window.Worker;
}

function supports_file_api() {
    if (window.File && window.FileReader && window.FileList) {
        return true;
    }
    return false;
}

function supports_offline() {
    return !!window.applicationCache;
}

function thm_delete(ele, imageid, type) {
    var confirm = window.confirm('Delete photo?');
    if (!confirm) {
        return;
    }

    $.ajaxswitch({
            type: "POST",
            url: '/photos/delete/' + type + '/',
            data: { ids: imageid },
            dataType: 'json'
        });

    // get parent container first
    $(ele).parents(".thumbnail").fadeOut(500, function () {
        $(this).remove();
        if ($('.images_column').find('.thumbnail').length == 0) {
            $('.images_column').find('.nophoto').show();
        }
    });
}

function validateEmail(email) {
    var re = /^(([^<>()[\]\\.,;:\s@\"]+(\.[^<>()[\]\\.,;:\s@\"]+)*)|(\".+\"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/;
    return email.match(re);
}

// prototypes
String.prototype.endsWith = function (suffix) {
    return this.indexOf(suffix, this.length - suffix.length) !== -1;
};
// Domain Public by Eric Wendelin http://eriwen.com/ (2008)
//                  Luke Smith http://lucassmith.name/ (2008)
//                  Loic Dachary <loic@dachary.org> (2008)
//                  Johan Euphrosine <proppy@aminche.com> (2008)
//                  Oyvind Sean Kinsey http://kinsey.no/blog (2010)
//                  Victor Homyakov <victor-homyakov@users.sourceforge.net> (2010)

/**
* Main function giving a function stack trace with a forced or passed in Error
*
* @cfg {Error} e The error to create a stacktrace from (optional)
* @cfg {Boolean} guess If we should try to resolve the names of anonymous functions
* @return {Array} of Strings with functions, lines, files, and arguments where possible
*/
function printStackTrace(options) {
    options = options || { guess: true };
    var ex = options.e || null, guess = !!options.guess;
    var p = new printStackTrace.implementation(), result = p.run(ex);
    return (guess) ? p.guessAnonymousFunctions(result) : result;
}

printStackTrace.implementation = function () {
};

printStackTrace.implementation.prototype = {
    run: function (ex) {
        ex = ex || this.createException();
        // Do not use the stored mode: different exceptions in Chrome
        // may or may not have arguments or stack
        var mode = this.mode(ex);
        // Use either the stored mode, or resolve it
        //var mode = this._mode || this.mode(ex);
        if (mode === 'other') {
            return this.other(arguments.callee);
        } else {
            return this[mode](ex);
        }
    },

    createException: function () {
        try {
            this.undef();
            return null;
        } catch (e) {
            return e;
        }
    },

    /**
    * @return {String} mode of operation for the environment in question.
    */
    mode: function (e) {
        if (e['arguments'] && e.stack) {
            return (this._mode = 'chrome');
        } else if (e.message && typeof window !== 'undefined' && window.opera) {
            return (this._mode = e.stacktrace ? 'opera10' : 'opera');
        } else if (e.stack) {
            return (this._mode = 'firefox');
        }
        return (this._mode = 'other');
    },

    /**
    * Given a context, function name, and callback function, overwrite it so that it calls
    * printStackTrace() first with a callback and then runs the rest of the body.
    *
    * @param {Object} context of execution (e.g. window)
    * @param {String} functionName to instrument
    * @param {Function} function to call with a stack trace on invocation
    */
    instrumentFunction: function (context, functionName, callback) {
        context = context || window;
        var original = context[functionName];
        context[functionName] = function instrumented() {
            callback.call(this, printStackTrace().slice(4));
            return context[functionName]._instrumented.apply(this, arguments);
        };
        context[functionName]._instrumented = original;
    },

    /**
    * Given a context and function name of a function that has been
    * instrumented, revert the function to it's original (non-instrumented)
    * state.
    *
    * @param {Object} context of execution (e.g. window)
    * @param {String} functionName to de-instrument
    */
    deinstrumentFunction: function (context, functionName) {
        if (context[functionName].constructor === Function &&
                context[functionName]._instrumented &&
                context[functionName]._instrumented.constructor === Function) {
            context[functionName] = context[functionName]._instrumented;
        }
    },

    /**
    * Given an Error object, return a formatted Array based on Chrome's stack string.
    *
    * @param e - Error object to inspect
    * @return Array<String> of function calls, files and line numbers
    */
    chrome: function (e) {
        //return e.stack.replace(/^[^\(]+?[\n$]/gm, '').replace(/^\s+at\s+/gm, '').replace(/^Object.<anonymous>\s*\(/gm, '{anonymous}()@').split('\n');
        return e.stack.replace(/^\S[^\(]+?[\n$]/gm, '').
          replace(/^\s+at\s+/gm, '').
          replace(/^([^\(]+?)([\n$])/gm, '{anonymous}()@$1$2').
          replace(/^Object.<anonymous>\s*\(([^\)]+)\)/gm, '{anonymous}()@$1').split('\n');
    },

    /**
    * Given an Error object, return a formatted Array based on Firefox's stack string.
    *
    * @param e - Error object to inspect
    * @return Array<String> of function calls, files and line numbers
    */
    firefox: function (e) {
        return e.stack.replace(/(?:\n@:0)?\s+$/m, '').replace(/^\(/gm, '{anonymous}(').split('\n');
    },

    /**
    * Given an Error object, return a formatted Array based on Opera 10's stacktrace string.
    *
    * @param e - Error object to inspect
    * @return Array<String> of function calls, files and line numbers
    */
    opera10: function (e) {
        var stack = e.stacktrace;
        var lines = stack.split('\n'), ANON = '{anonymous}', lineRE = /.*line (\d+), column (\d+) in ((<anonymous function\:?\s*(\S+))|([^\(]+)\([^\)]*\))(?: in )?(.*)\s*$/i, i, j, len;
        for (i = 2, j = 0, len = lines.length; i < len - 2; i++) {
            if (lineRE.test(lines[i])) {
                var location = RegExp.$6 + ':' + RegExp.$1 + ':' + RegExp.$2;
                var fnName = RegExp.$3;
                fnName = fnName.replace(/<anonymous function\:?\s?(\S+)?>/g, ANON);
                lines[j++] = fnName + '@' + location;
            }
        }

        lines.splice(j, lines.length - j);
        return lines;
    },

    // Opera 7.x-9.x only!
    opera: function (e) {
        var lines = e.message.split('\n'), ANON = '{anonymous}', lineRE = /Line\s+(\d+).*script\s+(http\S+)(?:.*in\s+function\s+(\S+))?/i, i, j, len;

        for (i = 4, j = 0, len = lines.length; i < len; i += 2) {
            //TODO: RegExp.exec() would probably be cleaner here
            if (lineRE.test(lines[i])) {
                lines[j++] = (RegExp.$3 ? RegExp.$3 + '()@' + RegExp.$2 + RegExp.$1 : ANON + '()@' + RegExp.$2 + ':' + RegExp.$1) + ' -- ' + lines[i + 1].replace(/^\s+/, '');
            }
        }

        lines.splice(j, lines.length - j);
        return lines;
    },

    // Safari, IE, and others
    other: function (curr) {
        var ANON = '{anonymous}', fnRE = /function\s*([\w\-$]+)?\s*\(/i, stack = [], fn, args, maxStackSize = 10;
        while (curr && stack.length < maxStackSize) {
            fn = fnRE.test(curr.toString()) ? RegExp.$1 || ANON : ANON;
            args = Array.prototype.slice.call(curr['arguments'] || []);
            stack[stack.length] = fn + '(' + this.stringifyArguments(args) + ')';
            curr = curr.caller;
        }
        return stack;
    },

    /**
    * Given arguments array as a String, subsituting type names for non-string types.
    *
    * @param {Arguments} object
    * @return {Array} of Strings with stringified arguments
    */
    stringifyArguments: function (args) {
        var slice = Array.prototype.slice;
        for (var i = 0; i < args.length; ++i) {
            var arg = args[i];
            if (arg === undefined) {
                args[i] = 'undefined';
            } else if (arg === null) {
                args[i] = 'null';
            } else if (arg.constructor) {
                if (arg.constructor === Array) {
                    if (arg.length < 3) {
                        args[i] = '[' + this.stringifyArguments(arg) + ']';
                    } else {
                        args[i] = '[' + this.stringifyArguments(slice.call(arg, 0, 1)) + '...' + this.stringifyArguments(slice.call(arg, -1)) + ']';
                    }
                } else if (arg.constructor === Object) {
                    args[i] = '#object';
                } else if (arg.constructor === Function) {
                    args[i] = '#function';
                } else if (arg.constructor === String) {
                    args[i] = '"' + arg + '"';
                }
            }
        }
        return args.join(',');
    },

    sourceCache: {},

    /**
    * @return the text from a given URL.
    */
    ajax: function (url) {
        var req = this.createXMLHTTPObject();
        if (!req) {
            return;
        }
        req.open('GET', url, false);
        req.setRequestHeader('User-Agent', 'XMLHTTP/1.0');
        req.send('');
        return req.responseText;
    },

    /**
    * Try XHR methods in order and store XHR factory.
    *
    * @return <Function> XHR function or equivalent
    */
    createXMLHTTPObject: function () {
        var xmlhttp, XMLHttpFactories = [
            function () {
                return new XMLHttpRequest();
            }, function () {
                return new ActiveXObject('Msxml2.XMLHTTP');
            }, function () {
                return new ActiveXObject('Msxml3.XMLHTTP');
            }, function () {
                return new ActiveXObject('Microsoft.XMLHTTP');
            }
        ];
        for (var i = 0; i < XMLHttpFactories.length; i++) {
            try {
                xmlhttp = XMLHttpFactories[i]();
                // Use memoization to cache the factory
                this.createXMLHTTPObject = XMLHttpFactories[i];
                return xmlhttp;
            } catch (e) {
            }
        }
    },

    /**
    * Given a URL, check if it is in the same domain (so we can get the source
    * via Ajax).
    *
    * @param url <String> source url
    * @return False if we need a cross-domain request
    */
    isSameDomain: function (url) {
        return url.indexOf(location.hostname) !== -1;
    },

    /**
    * Get source code from given URL if in the same domain.
    *
    * @param url <String> JS source URL
    * @return <Array> Array of source code lines
    */
    getSource: function (url) {
        if (!(url in this.sourceCache)) {
            this.sourceCache[url] = this.ajax(url).split('\n');
        }
        return this.sourceCache[url];
    },

    guessAnonymousFunctions: function (stack) {
        for (var i = 0; i < stack.length; ++i) {
            var reStack = /\{anonymous\}\(.*\)@(\w+:\/\/([\-\w\.]+)+(:\d+)?[^:]+):(\d+):?(\d+)?/;
            var frame = stack[i], m = reStack.exec(frame);
            if (m) {
                var file = m[1], lineno = m[4], charno = m[7] || 0; //m[7] is character position in Chrome
                if (file && this.isSameDomain(file) && lineno) {
                    var functionName = this.guessAnonymousFunction(file, lineno, charno);
                    stack[i] = frame.replace('{anonymous}', functionName);
                }
            }
        }
        return stack;
    },

    guessAnonymousFunction: function (url, lineNo, charNo) {
        var ret;
        try {
            ret = this.findFunctionName(this.getSource(url), lineNo);
        } catch (e) {
            ret = 'getSource failed with url: ' + url + ', exception: ' + e.toString();
        }
        return ret;
    },

    findFunctionName: function (source, lineNo) {
        // FIXME findFunctionName fails for compressed source
        // (more than one function on the same line)
        // TODO use captured args
        // function {name}({args}) m[1]=name m[2]=args
        var reFunctionDeclaration = /function\s+([^(]*?)\s*\(([^)]*)\)/;
        // {name} = function ({args}) TODO args capture
        // /['"]?([0-9A-Za-z_]+)['"]?\s*[:=]\s*function(?:[^(]*)/
        var reFunctionExpression = /['"]?([0-9A-Za-z_]+)['"]?\s*[:=]\s*function\b/;
        // {name} = eval()
        var reFunctionEvaluation = /['"]?([0-9A-Za-z_]+)['"]?\s*[:=]\s*(?:eval|new Function)\b/;
        // Walk backwards in the source lines until we find
        // the line which matches one of the patterns above
        var code = "", line, maxLines = 10, m;
        for (var i = 0; i < maxLines; ++i) {
            // FIXME lineNo is 1-based, source[] is 0-based
            line = source[lineNo - i];
            if (line) {
                code = line + code;
                m = reFunctionExpression.exec(code);
                if (m && m[1]) {
                    return m[1];
                }
                m = reFunctionDeclaration.exec(code);
                if (m && m[1]) {
                    //return m[1] + "(" + (m[2] || "") + ")";
                    return m[1];
                }
                m = reFunctionEvaluation.exec(code);
                if (m && m[1]) {
                    return m[1];
                }
            }
        }
        return '(?)';
    }
};
function load_analytics(acct) {
    var gaJsHost = (("https:" == document.location.protocol) ? "https://ssl." : "http://www."),
      pageTracker,
      s;
    s = document.createElement('script');
    s.src = gaJsHost + 'google-analytics.com/ga.js';
    s.type = 'text/javascript';
    s.onloadDone = false;
    function init() {
        pageTracker = _gat._getTracker(acct);
        pageTracker._setDomainName(".tradelr.com");
        pageTracker._setAllowLinker(true);
        pageTracker._setAllowHash(false);
        pageTracker._trackPageview();
    }
    s.onload = function() {
        s.onloadDone = true;
        init();
    };
    s.onreadystatechange = function() {
        if (('loaded' === s.readyState || 'complete' === s.readyState) && !s.onloadDone) {
            s.onloadDone = true;
            init();
        }
    };
    document.getElementsByTagName('head')[0].appendChild(s);
}
/*
* jQuery Autocomplete plugin 1.2.2
*
* Copyright (c) 2009 Jörn Zaefferer
*
* Dual licensed under the MIT and GPL licenses:
*   http://www.opensource.org/licenses/mit-license.php
*   http://www.gnu.org/licenses/gpl.html
*
* With small modifications by Alfonso Gómez-Arzola.
* See changelog for details.
*
*/

; (function ($) {

    $.fn.extend({
        autocomplete: function (urlOrData, options) {
            var isUrl = typeof urlOrData == "string";
            options = $.extend({}, $.Autocompleter.defaults, {
                url: isUrl ? urlOrData : null,
                data: isUrl ? null : urlOrData,
                delay: isUrl ? $.Autocompleter.defaults.delay : 10,
                max: options && !options.scroll ? 10 : 150
            }, options);

            // if highlight is set to false, replace it with a do-nothing function
            options.highlight = options.highlight || function (value) { return value; };

            // if the formatMatch option is not specified, then use formatItem for backwards compatibility
            options.formatMatch = options.formatMatch || options.formatItem;

            return this.each(function () {
                new $.Autocompleter(this, options);
            });
        },
        result: function (handler) {
            return this.bind("result", handler);
        },
        search: function (handler) {
            return this.trigger("search", [handler]);
        },
        flushCache: function () {
            return this.trigger("flushCache");
        },
        setOptions: function (options) {
            return this.trigger("setOptions", [options]);
        },
        unautocomplete: function () {
            return this.trigger("unautocomplete");
        }
    });

    $.Autocompleter = function (input, options) {

        var KEY = {
            UP: 38,
            DOWN: 40,
            DEL: 46,
            TAB: 9,
            RETURN: 13,
            ESC: 27,
            COMMA: 188,
            PAGEUP: 33,
            PAGEDOWN: 34,
            BACKSPACE: 8
        };

        var globalFailure = null;
        if (options.failure != null && typeof options.failure == "function") {
            globalFailure = options.failure;
        }

        // Create $ object for input element
        var $input = $(input).attr("autocomplete", "off").addClass(options.inputClass);

        var timeout;
        var previousValue = "";
        var cache = $.Autocompleter.Cache(options);
        var hasFocus = 0;
        var lastKeyPressCode;
        var config = {
            mouseDownOnSelect: false
        };
        var select = $.Autocompleter.Select(options, input, selectCurrent, config);

        var blockSubmit;

        // prevent form submit in opera when selecting with return key
        $.browser.opera && $(input.form).bind("submit.autocomplete", function () {
            if (blockSubmit) {
                blockSubmit = false;
                return false;
            }
        });

        // only opera doesn't trigger keydown multiple times while pressed, others don't work with keypress at all
        $input.bind(($.browser.opera ? "keypress" : "keydown") + ".autocomplete", function (event) {
            // a keypress means the input has focus
            // avoids issue where input had focus before the autocomplete was applied
            hasFocus = 1;
            // track last key pressed
            lastKeyPressCode = event.keyCode;
            switch (event.keyCode) {

                case KEY.UP:
                    if (select.visible()) {
                        event.preventDefault();
                        select.prev();
                    } else {
                        onChange(0, true);
                    }
                    break;

                case KEY.DOWN:
                    if (select.visible()) {
                        event.preventDefault();
                        select.next();
                    } else {
                        onChange(0, true);
                    }
                    break;

                case KEY.PAGEUP:
                    if (select.visible()) {
                        event.preventDefault();
                        select.pageUp();
                    } else {
                        onChange(0, true);
                    }
                    break;

                case KEY.PAGEDOWN:
                    if (select.visible()) {
                        event.preventDefault();
                        select.pageDown();
                    } else {
                        onChange(0, true);
                    }
                    break;

                // matches also semicolon 
                case options.multiple && $.trim(options.multipleSeparator) == "," && KEY.COMMA:
                case KEY.TAB:
                case KEY.RETURN:
                    if (selectCurrent()) {
                        // stop default to prevent a form submit, Opera needs special handling
                        event.preventDefault();
                        blockSubmit = true;
                        return false;
                    }
                    break;

                case KEY.ESC:
                    select.hide();
                    break;

                default:
                    clearTimeout(timeout);
                    timeout = setTimeout(onChange, options.delay);
                    break;
            }
        }).focus(function () {
            // track whether the field has focus, we shouldn't process any
            // results if the field no longer has focus
            hasFocus++;
        }).blur(function () {
            hasFocus = 0;
            if (!config.mouseDownOnSelect) {
                hideResults();
            }
        }).click(function () {
            // show select when clicking in a focused field
            // but if clickFire is true, don't require field
            // to be focused to begin with; just show select
            if (options.clickFire) {
                if (!select.visible()) {
                    onChange(0, true);
                }
            } else {
                if (hasFocus++ > 1 && !select.visible()) {
                    onChange(0, true);
                }
            }
        }).bind("search", function () {
            // TODO why not just specifying both arguments?
            var fn = (arguments.length > 1) ? arguments[1] : null;
            function findValueCallback(q, data) {
                var result;
                if (data && data.length) {
                    for (var i = 0; i < data.length; i++) {
                        if (data[i].result.toLowerCase() == q.toLowerCase()) {
                            result = data[i];
                            break;
                        }
                    }
                }
                if (typeof fn == "function") fn(result);
                else $input.trigger("result", result && [result.data, result.value]);
            }
            $.each(trimWords($input.val()), function (i, value) {
                request(value, findValueCallback, findValueCallback);
            });
        }).bind("flushCache", function () {
            cache.flush();
        }).bind("setOptions", function () {
            $.extend(true, options, arguments[1]);
            // if we've updated the data, repopulate
            if ("data" in arguments[1])
                cache.populate();
        }).bind("unautocomplete", function () {
            select.unbind();
            $input.unbind();
            $(input.form).unbind(".autocomplete");
        });


        function selectCurrent() {
            var selected = select.selected();
            if (!selected)
                return false;

            var v = selected.result;
            previousValue = v;

            if (options.multiple) {
                var words = trimWords($input.val());
                if (words.length > 1) {
                    var seperator = options.multipleSeparator.length;
                    var cursorAt = $(input).selection().start;
                    var wordAt, progress = 0;
                    $.each(words, function (i, word) {
                        progress += word.length;
                        if (cursorAt <= progress) {
                            wordAt = i;
                            return false;
                        }
                        progress += seperator;
                    });
                    words[wordAt] = v;
                    // TODO this should set the cursor to the right position, but it gets overriden somewhere
                    //$.Autocompleter.Selection(input, progress + seperator, progress + seperator);
                    v = words.join(options.multipleSeparator);
                }
                v += options.multipleSeparator;
            }

            $input.val(v);
            hideResultsNow();
            $input.trigger("result", [selected.data, selected.value]);
            return true;
        }

        function onChange(crap, skipPrevCheck) {
            if (lastKeyPressCode == KEY.DEL) {
                select.hide();
                return;
            }

            var currentValue = $input.val();

            if (!skipPrevCheck && currentValue == previousValue)
                return;

            previousValue = currentValue;

            currentValue = lastWord(currentValue);
            if (currentValue.length >= options.minChars) {
                $input.addClass(options.loadingClass);
                if (!options.matchCase)
                    currentValue = currentValue.toLowerCase();
                request(currentValue, receiveData, hideResultsNow);
            } else {
                stopLoading();
                select.hide();
            }
        };

        function trimWords(value) {
            if (!value)
                return [""];
            if (!options.multiple)
                return [$.trim(value)];
            return $.map(value.split(options.multipleSeparator), function (word) {
                return $.trim(value).length ? $.trim(word) : null;
            });
        }

        function lastWord(value) {
            if (!options.multiple)
                return value;
            var words = trimWords(value);
            if (words.length == 1)
                return words[0];
            var cursorAt = $(input).selection().start;
            if (cursorAt == value.length) {
                words = trimWords(value)
            } else {
                words = trimWords(value.replace(value.substring(cursorAt), ""));
            }
            return words[words.length - 1];
        }

        // fills in the input box w/the first match (assumed to be the best match)
        // q: the term entered
        // sValue: the first matching result
        function autoFill(q, sValue) {
            // autofill in the complete box w/the first match as long as the user hasn't entered in more data
            // if the last user key pressed was backspace, don't autofill
            if (options.autoFill && (lastWord($input.val()).toLowerCase() == q.toLowerCase()) && lastKeyPressCode != KEY.BACKSPACE) {
                // fill in the value (keep the case the user has typed)
                $input.val($input.val() + sValue.substring(lastWord(previousValue).length));
                // select the portion of the value not typed by the user (so the next character will erase)
                $(input).selection(previousValue.length, previousValue.length + sValue.length);
            }
        };

        function hideResults() {
            clearTimeout(timeout);
            timeout = setTimeout(hideResultsNow, 200);
        };

        function hideResultsNow() {
            var wasVisible = select.visible();
            select.hide();
            clearTimeout(timeout);
            stopLoading();
            if (options.mustMatch) {
                // call search and run callback
                $input.search(
				function (result) {
				    // if no value found, clear the input box
				    if (!result) {
				        if (options.multiple) {
				            var words = trimWords($input.val()).slice(0, -1);
				            $input.val(words.join(options.multipleSeparator) + (words.length ? options.multipleSeparator : ""));
				        }
				        else {
				            $input.val("");
				            $input.trigger("result", null);
				        }
				    }
				}
			);
            }
        };

        function receiveData(q, data) {
            if (data && data.length && hasFocus) {
                stopLoading();
                select.display(data, q);
                autoFill(q, data[0].value);
                select.show();
            } else {
                hideResultsNow();
            }
        };

        function request(term, success, failure) {
            if (!options.matchCase)
                term = term.toLowerCase();
            var data = cache.load(term);
            // recieve the cached data
            if (data && data.length) {
                success(term, data);
                // if an AJAX url has been supplied, try loading the data now
            } else if ((typeof options.url == "string") && (options.url.length > 0)) {

                var extraParams = {
                    timestamp: +new Date()
                };
                $.each(options.extraParams, function (key, param) {
                    extraParams[key] = typeof param == "function" ? param() : param;
                });

                $.ajax({
                    // try to leverage ajaxQueue plugin to abort previous requests
                    mode: "abort",
                    // limit abortion to this input
                    port: "autocomplete" + input.name,
                    dataType: options.dataType,
                    url: options.url,
                    data: $.extend({
                        q: lastWord(term),
                        limit: options.max
                    }, extraParams),
                    success: function (data) {
                        var parsed = options.parse && options.parse(data) || parse(data);
                        cache.add(term, parsed);
                        success(term, parsed);
                    }
                });
            } else {
                // if we have a failure, we need to empty the list -- this prevents the the [TAB] key from selecting the last successful match
                select.emptyList();
                if (globalFailure != null) {
                    globalFailure();
                }
                else {
                    failure(term);
                }
            }
        };

        function parse(data) {
            var parsed = [];
            var rows = [];
            try {
                rows = data.split("\n");
            } catch(e) {

            } 
            for (var i = 0; i < rows.length; i++) {
                var row = $.trim(rows[i]);
                if (row) {
                    row = row.split("|");
                    parsed[parsed.length] = {
                        data: row,
                        value: row[0],
                        result: options.formatResult && options.formatResult(row, row[0]) || row[0]
                    };
                }
            }
            return parsed;
        };

        function stopLoading() {
            $input.removeClass(options.loadingClass);
        };

    };

    $.Autocompleter.defaults = {
        inputClass: "ac_input",
        resultsClass: "ac_results",
        loadingClass: "ac_loading",
        minChars: 1,
        delay: 400,
        matchCase: false,
        matchSubset: true,
        matchContains: false,
        cacheLength: 100,
        max: 1000,
        mustMatch: false,
        extraParams: {},
        selectFirst: true,
        formatItem: function (row) { return row[0]; },
        formatMatch: null,
        autoFill: false,
        width: 0,
        multiple: false,
        multipleSeparator: " ",
        inputFocus: true,
        clickFire: false,
        highlight: function (value, term) {
            return value.replace(new RegExp("(?![^&;]+;)(?!<[^<>]*)(" + term.replace(/([\^\$\(\)\[\]\{\}\*\.\+\?\|\\])/gi, "\\$1") + ")(?![^<>]*>)(?![^&;]+;)", "gi"), "<strong>$1</strong>");
        },
        scroll: true,
        scrollHeight: 180,
        scrollJumpPosition: true
    };

    $.Autocompleter.Cache = function (options) {

        var data = {};
        var length = 0;

        function matchSubset(s, sub) {
            if (!options.matchCase)
                s = s.toLowerCase();
            var i = s.indexOf(sub);
            if (options.matchContains == "word") {
                i = s.toLowerCase().search("\\b" + sub.toLowerCase());
            }
            if (i == -1) return false;
            return i == 0 || options.matchContains;
        };

        function add(q, value) {
            if (length > options.cacheLength) {
                flush();
            }
            if (!data[q]) {
                length++;
            }
            data[q] = value;
        }

        function populate() {
            if (!options.data) return false;
            // track the matches
            var stMatchSets = {},
			nullData = 0;

            // no url was specified, we need to adjust the cache length to make sure it fits the local data store
            if (!options.url) options.cacheLength = 1;

            // track all options for minChars = 0
            stMatchSets[""] = [];

            // loop through the array and create a lookup structure
            for (var i = 0, ol = options.data.length; i < ol; i++) {
                var rawValue = options.data[i];
                // if rawValue is a string, make an array otherwise just reference the array
                rawValue = (typeof rawValue == "string") ? [rawValue] : rawValue;

                var value = options.formatMatch(rawValue, i + 1, options.data.length);
                if (value === false)
                    continue;

                var firstChar = value.charAt(0).toLowerCase();
                // if no lookup array for this character exists, look it up now
                if (!stMatchSets[firstChar])
                    stMatchSets[firstChar] = [];

                // if the match is a string
                var row = {
                    value: value,
                    data: rawValue,
                    result: options.formatResult && options.formatResult(rawValue) || value
                };

                // push the current match into the set list
                stMatchSets[firstChar].push(row);

                // keep track of minChars zero items
                if (nullData++ < options.max) {
                    stMatchSets[""].push(row);
                }
            };

            // add the data items to the cache
            $.each(stMatchSets, function (i, value) {
                // increase the cache size
                options.cacheLength++;
                // add to the cache
                add(i, value);
            });
        }

        // populate any existing data
        setTimeout(populate, 25);

        function flush() {
            data = {};
            length = 0;
        }

        return {
            flush: flush,
            add: add,
            populate: populate,
            load: function (q) {
                if (!options.cacheLength || !length)
                    return null;
                /* 
                * if dealing w/local data and matchContains than we must make sure
                * to loop through all the data collections looking for matches
                */
                if (!options.url && options.matchContains) {
                    // track all matches
                    var csub = [];
                    // loop through all the data grids for matches
                    for (var k in data) {
                        // don't search through the stMatchSets[""] (minChars: 0) cache
                        // this prevents duplicates
                        if (k.length > 0) {
                            var c = data[k];
                            $.each(c, function (i, x) {
                                // if we've got a match, add it to the array
                                if (matchSubset(x.value, q)) {
                                    csub.push(x);
                                }
                            });
                        }
                    }
                    return csub;
                } else
                // if the exact item exists, use it
                    if (data[q]) {
                        return data[q];
                    } else
                        if (options.matchSubset) {
                            for (var i = q.length - 1; i >= options.minChars; i--) {
                                var c = data[q.substr(0, i)];
                                if (c) {
                                    var csub = [];
                                    $.each(c, function (i, x) {
                                        if (matchSubset(x.value, q)) {
                                            csub[csub.length] = x;
                                        }
                                    });
                                    return csub;
                                }
                            }
                        }
                return null;
            }
        };
    };

    $.Autocompleter.Select = function (options, input, select, config) {
        var CLASSES = {
            ACTIVE: "ac_over"
        };

        var listItems,
		active = -1,
		data,
		term = "",
		needsInit = true,
		element,
		list;

        // Create results
        function init() {
            if (!needsInit)
                return;
            element = $("<div/>")
		.hide()
		.addClass(options.resultsClass)
		.css("position", "absolute")
		.appendTo(document.body)
		.hover(function (event) {
		    // Browsers except FF do not fire mouseup event on scrollbars, resulting in mouseDownOnSelect remaining true, and results list not always hiding.
		    if ($(this).is(":visible")) {
		        input.focus();
		    }
		    config.mouseDownOnSelect = false;
		});

            list = $("<ul/>").appendTo(element).mouseover(function (event) {
                if (target(event).nodeName && target(event).nodeName.toUpperCase() == 'LI') {
                    active = $("li", list).removeClass(CLASSES.ACTIVE).index(target(event));
                    $(target(event)).addClass(CLASSES.ACTIVE);
                }
            }).click(function (event) {
                $(target(event)).addClass(CLASSES.ACTIVE);
                select();
                if (options.inputFocus)
                    input.focus();
                return false;
            }).mousedown(function () {
                config.mouseDownOnSelect = true;
            }).mouseup(function () {
                config.mouseDownOnSelect = false;
            });

            if (options.width > 0)
                element.css("width", options.width);

            needsInit = false;
        }

        function target(event) {
            var element = event.target;
            while (element && element.tagName != "LI")
                element = element.parentNode;
            // more fun with IE, sometimes event.target is empty, just ignore it then
            if (!element)
                return [];
            return element;
        }

        function moveSelect(step) {
            listItems.slice(active, active + 1).removeClass(CLASSES.ACTIVE);
            movePosition(step);
            var activeItem = listItems.slice(active, active + 1).addClass(CLASSES.ACTIVE);
            if (options.scroll) {
                var offset = 0;
                listItems.slice(0, active).each(function () {
                    offset += this.offsetHeight;
                });
                if ((offset + activeItem[0].offsetHeight - list.scrollTop()) > list[0].clientHeight) {
                    list.scrollTop(offset + activeItem[0].offsetHeight - list.innerHeight());
                } else if (offset < list.scrollTop()) {
                    list.scrollTop(offset);
                }
            }
        };

        function movePosition(step) {
            if (options.scrollJumpPosition || (!options.scrollJumpPosition && !((step < 0 && active == 0) || (step > 0 && active == listItems.size() - 1)))) {
                active += step;
                if (active < 0) {
                    active = listItems.size() - 1;
                } else if (active >= listItems.size()) {
                    active = 0;
                }
            }
        }


        function limitNumberOfItems(available) {
            return options.max && options.max < available
			? options.max
			: available;
        }

        function fillList() {
            list.empty();
            var max = limitNumberOfItems(data.length);
            for (var i = 0; i < max; i++) {
                if (!data[i])
                    continue;
                var formatted = options.formatItem(data[i].data, i + 1, max, data[i].value, term);
                if (formatted === false)
                    continue;
                var li = $("<li/>").html(options.highlight(formatted, term)).addClass(i % 2 == 0 ? "ac_even" : "ac_odd").appendTo(list)[0];
                $.data(li, "ac_data", data[i]);
            }
            listItems = list.find("li");
            if (options.selectFirst) {
                listItems.slice(0, 1).addClass(CLASSES.ACTIVE);
                active = 0;
            }
            // apply bgiframe if available
            if ($.fn.bgiframe)
                list.bgiframe();
        }

        return {
            display: function (d, q) {
                init();
                data = d;
                term = q;
                fillList();
            },
            next: function () {
                moveSelect(1);
            },
            prev: function () {
                moveSelect(-1);
            },
            pageUp: function () {
                if (active != 0 && active - 8 < 0) {
                    moveSelect(-active);
                } else {
                    moveSelect(-8);
                }
            },
            pageDown: function () {
                if (active != listItems.size() - 1 && active + 8 > listItems.size()) {
                    moveSelect(listItems.size() - 1 - active);
                } else {
                    moveSelect(8);
                }
            },
            hide: function () {
                element && element.hide();
                listItems && listItems.removeClass(CLASSES.ACTIVE);
                active = -1;
            },
            visible: function () {
                return element && element.is(":visible");
            },
            current: function () {
                return this.visible() && (listItems.filter("." + CLASSES.ACTIVE)[0] || options.selectFirst && listItems[0]);
            },
            show: function () {
                var offset = $(input).offset();
                element.css({
                    width: typeof options.width == "string" || options.width > 0 ? options.width : $(input).width(),
                    top: offset.top + input.offsetHeight,
                    left: offset.left
                }).show();
                if (options.scroll) {
                    list.scrollTop(0);
                    list.css({
                        maxHeight: options.scrollHeight,
                        overflow: 'auto'
                    });

                    if ($.browser.msie && typeof document.body.style.maxHeight === "undefined") {
                        var listHeight = 0;
                        listItems.each(function () {
                            listHeight += this.offsetHeight;
                        });
                        var scrollbarsVisible = listHeight > options.scrollHeight;
                        list.css('height', scrollbarsVisible ? options.scrollHeight : listHeight);
                        if (!scrollbarsVisible) {
                            // IE doesn't recalculate width when scrollbar disappears
                            listItems.width(list.width() - parseInt(listItems.css("padding-left")) - parseInt(listItems.css("padding-right")));
                        }
                    }

                }
            },
            selected: function () {
                var selected = listItems && listItems.filter("." + CLASSES.ACTIVE).removeClass(CLASSES.ACTIVE);
                return selected && selected.length && $.data(selected[0], "ac_data");
            },
            emptyList: function () {
                list && list.empty();
            },
            unbind: function () {
                element && element.remove();
            }
        };
    };

    $.fn.selection = function (start, end) {
        if (start !== undefined) {
            return this.each(function () {
                if (this.createTextRange) {
                    var selRange = this.createTextRange();
                    if (end === undefined || start == end) {
                        selRange.move("character", start);
                        selRange.select();
                    } else {
                        selRange.collapse(true);
                        selRange.moveStart("character", start);
                        selRange.moveEnd("character", end);
                        selRange.select();
                    }
                } else if (this.setSelectionRange) {
                    this.setSelectionRange(start, end);
                } else if (this.selectionStart) {
                    this.selectionStart = start;
                    this.selectionEnd = end;
                }
            });
        }
        var field = this[0];
        if (field.createTextRange) {
            var range = document.selection.createRange(),
			orig = field.value,
			teststring = "<->",
			textLength = range.text.length;
            range.text = teststring;
            var caretAt = field.value.indexOf(teststring);
            field.value = orig;
            this.selection(caretAt, caretAt + textLength);
            return {
                start: caretAt,
                end: caretAt + textLength
            }
        } else if (field.selectionStart !== undefined) {
            return {
                start: field.selectionStart,
                end: field.selectionEnd
            }
        }
    };

})(jQuery);/**
 * Cookie plugin
 *
 * Copyright (c) 2006 Klaus Hartl (stilbuero.de)
 * Dual licensed under the MIT and GPL licenses:
 * http://www.opensource.org/licenses/mit-license.php
 * http://www.gnu.org/licenses/gpl.html
 *
 */

/**
 * Create a cookie with the given name and value and other optional parameters.
 *
 * @example $.cookie('the_cookie', 'the_value');
 * @desc Set the value of a cookie.
 * @example $.cookie('the_cookie', 'the_value', { expires: 7, path: '/', domain: 'jquery.com', secure: true });
 * @desc Create a cookie with all available options.
 * @example $.cookie('the_cookie', 'the_value');
 * @desc Create a session cookie.
 * @example $.cookie('the_cookie', null);
 * @desc Delete a cookie by passing null as value. Keep in mind that you have to use the same path and domain
 *       used when the cookie was set.
 *
 * @param String name The name of the cookie.
 * @param String value The value of the cookie.
 * @param Object options An object literal containing key/value pairs to provide optional cookie attributes.
 * @option Number|Date expires Either an integer specifying the expiration date from now on in days or a Date object.
 *                             If a negative value is specified (e.g. a date in the past), the cookie will be deleted.
 *                             If set to null or omitted, the cookie will be a session cookie and will not be retained
 *                             when the the browser exits.
 * @option String path The value of the path atribute of the cookie (default: path of page that created the cookie).
 * @option String domain The value of the domain attribute of the cookie (default: domain of page that created the cookie).
 * @option Boolean secure If true, the secure attribute of the cookie will be set and the cookie transmission will
 *                        require a secure protocol (like HTTPS).
 * @type undefined
 *
 * @name $.cookie
 * @cat Plugins/Cookie
 * @author Klaus Hartl/klaus.hartl@stilbuero.de
 */

/**
 * Get the value of a cookie with the given name.
 *
 * @example $.cookie('the_cookie');
 * @desc Get the value of a cookie.
 *
 * @param String name The name of the cookie.
 * @return The value of the cookie.
 * @type String
 *
 * @name $.cookie
 * @cat Plugins/Cookie
 * @author Klaus Hartl/klaus.hartl@stilbuero.de
 */
jQuery.cookie = function(name, value, options) {
    if (typeof value != 'undefined') { // name and value given, set cookie
        options = options || {};
        if (value === null) {
            value = '';
            options.expires = -1;
        }
        var expires = '';
        if (options.expires && (typeof options.expires == 'number' || options.expires.toUTCString)) {
            var date;
            if (typeof options.expires == 'number') {
                date = new Date();
                date.setTime(date.getTime() + (options.expires * 24 * 60 * 60 * 1000));
            } else {
                date = options.expires;
            }
            expires = '; expires=' + date.toUTCString(); // use expires attribute, max-age is not supported by IE
        }
        // CAUTION: Needed to parenthesize options.path and options.domain
        // in the following expressions, otherwise they evaluate to undefined
        // in the packed version for some reason...
        var path = options.path ? '; path=' + (options.path) : '';
        var domain = options.domain ? '; domain=' + (options.domain) : '';
        var secure = options.secure ? '; secure' : '';
        document.cookie = [name, '=', encodeURIComponent(value), expires, path, domain, secure].join('');
    } else { // only name given, get cookie
        var cookieValue = null;
        if (document.cookie && document.cookie != '') {
            var cookies = document.cookie.split(';');
            for (var i = 0; i < cookies.length; i++) {
                var cookie = jQuery.trim(cookies[i]);
                // Does this cookie string begin with the name we want?
                if (cookie.substring(0, name.length + 1) == (name + '=')) {
                    cookieValue = decodeURIComponent(cookie.substring(name.length + 1));
                    break;
                }
            }
        }
        return cookieValue;
    }
};/* 
* Auto Expanding Text Area (1.2.2)
* by Chrys Bader (www.chrysbader.com)
* chrysb@gmail.com
*
* Special thanks to:
* Jake Chapa - jake@hybridstudio.com
* John Resig - jeresig@gmail.com
*
* Copyright (c) 2008 Chrys Bader (www.chrysbader.com)
* Licensed under the GPL (GPL-LICENSE.txt) license. 
*
*
* NOTE: This script requires jQuery to work.  Download jQuery at www.jquery.com
*
*/

(function (jQuery) {

    var self = null;

    jQuery.fn.autogrow = function (o) {
        return this.each(function () {
            new jQuery.autogrow(this, o);
        });
    };


    /**
    * The autogrow object.
    *
    * @constructor
    * @name jQuery.autogrow
    * @param Object e The textarea to create the autogrow for.
    * @param Hash o A set of key/value pairs to set as configuration properties.
    * @cat Plugins/autogrow
    */

    jQuery.autogrow = function (e, o) {
        this.options = o || {};
        this.dummy = null;
        this.interval = null;
        this.line_height = this.options.lineHeight || parseInt(jQuery(e).css('line-height'));
        this.min_height = this.options.minHeight || parseInt(jQuery(e).css('min-height'));
        this.max_height = this.options.maxHeight || parseInt(jQuery(e).css('max-height')); ;
        this.expand_callback = this.options.expandCallback;
        this.textarea = jQuery(e);

        if (!this.line_height)
            this.line_height = 18;

        // Only one textarea activated at a time, the one being used
        this.init();
    };

    jQuery.autogrow.fn = jQuery.autogrow.prototype = {
        autogrow: '1.2.2'
    };

    jQuery.autogrow.fn.extend = jQuery.autogrow.extend = jQuery.extend;

    jQuery.autogrow.fn.extend({

        init: function () {
            var self = this;
            this.textarea.css({ overflow: 'hidden', display: 'inline-block' });
            this.textarea.bind('focus', function () { self.startExpand(); }).bind('blur', function () { self.stopExpand(); });
            this.checkExpand();
        },

        startExpand: function () {
            var self = this;
            this.interval = window.setInterval(function () { self.checkExpand(); }, 400);
        },

        stopExpand: function () {
            clearInterval(this.interval);
        },

        checkExpand: function () {

            if (this.dummy == null) {
                this.dummy = jQuery('<div></div>');
                this.dummy.css({
                    'font-size': this.textarea.css('font-size'),
                    'font-family': this.textarea.css('font-family'),
                    'width': this.textarea.css('width'),
                    'padding-top': this.textarea.css('padding-top'),
                    'padding-right': this.textarea.css('padding-right'),
                    'padding-bottom': this.textarea.css('padding-bottom'),
                    'padding-left': this.textarea.css('padding-left'),
                    'line-height': this.line_height + 'px',
                    'overflow-x': 'hidden',
                    'position': 'absolute',
                    'top': 0,
                    'left': -9999
                }).appendTo('body');
            }

            // Strip HTML tags
            var html = this.textarea.val().replace(/(<|>)/g, '');

            // IE is different, as per usual
            if (jQuery.browser.msie) {
                html = html.replace(/\n/g, '<BR>new');
            }
            else {
                html = html.replace(/\n/g, '<br>new');
            }

            if (this.dummy.html() != html) {
                this.dummy.html(html);

                if (this.max_height > 0 && (this.dummy.height() + this.line_height > this.max_height)) {
                    this.textarea.css('overflow-y', 'auto');
                }
                else {
                    this.textarea.css('overflow-y', 'hidden');
                    if (this.textarea.height() < this.dummy.height() + this.line_height || (this.dummy.height() < this.textarea.height())) {
                        this.textarea.animate({ height: (this.dummy.height() + this.line_height) + 'px' }, 100);
                    }
                }
            }

            if (this.expand_callback) {
                var self = this;
                window.setTimeout(function () { self.expand_callback() }, 500);
            }
        }

    });
})(jQuery);/*
 * jQuery validation plug-in 1.7
 *
 * http://bassistance.de/jquery-plugins/jquery-plugin-validation/
 * http://docs.jquery.com/Plugins/Validation
 *
 * Copyright (c) 2006 - 2008 Jörn Zaefferer
 *
 * $Id: jquery.validate.js 6403 2009-06-17 14:27:16Z joern.zaefferer $
 *
 * Dual licensed under the MIT and GPL licenses:
 *   http://www.opensource.org/licenses/mit-license.php
 *   http://www.gnu.org/licenses/gpl.html
 */

(function($) {

$.extend($.fn, {
	// http://docs.jquery.com/Plugins/Validation/validate
	validate: function( options ) {

		// if nothing is selected, return nothing; can't chain anyway
		if (!this.length) {
			options && options.debug && window.console && console.warn( "nothing selected, can't validate, returning nothing" );
			return;
		}

		// check if a validator for this form was already created
		var validator = $.data(this[0], 'validator');
		if ( validator ) {
			return validator;
		}
		
		validator = new $.validator( options, this[0] );
		$.data(this[0], 'validator', validator); 
		
		if ( validator.settings.onsubmit ) {
		
			// allow suppresing validation by adding a cancel class to the submit button
			this.find("input, button").filter(".cancel").click(function() {
				validator.cancelSubmit = true;
			});
			
			// when a submitHandler is used, capture the submitting button
			if (validator.settings.submitHandler) {
				this.find("input, button").filter(":submit").click(function() {
					validator.submitButton = this;
				});
			}
		
			// validate the form on submit
			this.submit( function( event ) {
				if ( validator.settings.debug )
					// prevent form submit to be able to see console output
					event.preventDefault();
					
				function handle() {
					if ( validator.settings.submitHandler ) {
						if (validator.submitButton) {
							// insert a hidden input as a replacement for the missing submit button
							var hidden = $("<input type='hidden'/>").attr("name", validator.submitButton.name).val(validator.submitButton.value).appendTo(validator.currentForm);
						}
						validator.settings.submitHandler.call( validator, validator.currentForm );
						if (validator.submitButton) {
							// and clean up afterwards; thanks to no-block-scope, hidden can be referenced
							hidden.remove();
						}
						return false;
					}
					return true;
				}
					
				// prevent submit for invalid forms or custom submit handlers
				if ( validator.cancelSubmit ) {
					validator.cancelSubmit = false;
					return handle();
				}
				if ( validator.form() ) {
					if ( validator.pendingRequest ) {
						validator.formSubmitted = true;
						return false;
					}
					return handle();
				} else {
					validator.focusInvalid();
					return false;
				}
			});
		}
		
		return validator;
	},
	// http://docs.jquery.com/Plugins/Validation/valid
	valid: function() {
        if ( $(this[0]).is('form')) {
            return this.validate().form();
        } else {
            var valid = true;
            var validator = $(this[0].form).validate();
            this.each(function() {
				valid &= validator.element(this);
            });
            return valid;
        }
    },
	// attributes: space seperated list of attributes to retrieve and remove
	removeAttrs: function(attributes) {
		var result = {},
			$element = this;
		$.each(attributes.split(/\s/), function(index, value) {
			result[value] = $element.attr(value);
			$element.removeAttr(value);
		});
		return result;
	},
	// http://docs.jquery.com/Plugins/Validation/rules
	rules: function(command, argument) {
		var element = this[0];
		
		if (command) {
			var settings = $.data(element.form, 'validator').settings;
			var staticRules = settings.rules;
			var existingRules = $.validator.staticRules(element);
			switch(command) {
			case "add":
				$.extend(existingRules, $.validator.normalizeRule(argument));
				staticRules[element.name] = existingRules;
				if (argument.messages)
					settings.messages[element.name] = $.extend( settings.messages[element.name], argument.messages );
				break;
			case "remove":
				if (!argument) {
					delete staticRules[element.name];
					return existingRules;
				}
				var filtered = {};
				$.each(argument.split(/\s/), function(index, method) {
					filtered[method] = existingRules[method];
					delete existingRules[method];
				});
				return filtered;
			}
		}
		
		var data = $.validator.normalizeRules(
		$.extend(
			{},
			$.validator.metadataRules(element),
			$.validator.classRules(element),
			$.validator.attributeRules(element),
			$.validator.staticRules(element)
		), element);
		
		// make sure required is at front
		if (data.required) {
			var param = data.required;
			delete data.required;
			data = $.extend({required: param}, data);
		}
		
		return data;
	}
});

// Custom selectors
$.extend($.expr[":"], {
	// http://docs.jquery.com/Plugins/Validation/blank
	blank: function(a) {return !$.trim("" + a.value);},
	// http://docs.jquery.com/Plugins/Validation/filled
	filled: function(a) {return !!$.trim("" + a.value);},
	// http://docs.jquery.com/Plugins/Validation/unchecked
	unchecked: function(a) {return !a.checked;}
});

// constructor for validator
$.validator = function( options, form ) {
	this.settings = $.extend( true, {}, $.validator.defaults, options );
	this.currentForm = form;
	this.init();
};

$.validator.format = function(source, params) {
	if ( arguments.length == 1 ) 
		return function() {
			var args = $.makeArray(arguments);
			args.unshift(source);
			return $.validator.format.apply( this, args );
		};
	if ( arguments.length > 2 && params.constructor != Array  ) {
		params = $.makeArray(arguments).slice(1);
	}
	if ( params.constructor != Array ) {
		params = [ params ];
	}
	$.each(params, function(i, n) {
		source = source.replace(new RegExp("\\{" + i + "\\}", "g"), n);
	});
	return source;
};

$.extend($.validator, {
	
	defaults: {
		messages: {},
		groups: {},
		rules: {},
		errorClass: "error",
		validClass: "valid",
		errorElement: "label",
		focusInvalid: true,
		errorContainer: $( [] ),
		errorLabelContainer: $( [] ),
		onsubmit: true,
		ignore: [],
		ignoreTitle: false,
		onfocusin: function(element) {
			this.lastActive = element;
				
			// hide error label and remove error class on focus if enabled
			if ( this.settings.focusCleanup && !this.blockFocusCleanup ) {
				this.settings.unhighlight && this.settings.unhighlight.call( this, element, this.settings.errorClass, this.settings.validClass );
				this.errorsFor(element).hide();
			}
		},
		onfocusout: function(element) {
			if ( !this.checkable(element) && (element.name in this.submitted || !this.optional(element)) ) {
				this.element(element);
			}
		},
		onkeyup: function(element) {
			if ( element.name in this.submitted || element == this.lastElement ) {
				this.element(element);
			}
		},
		onclick: function(element) {
			// click on selects, radiobuttons and checkboxes
			if ( element.name in this.submitted )
				this.element(element);
			// or option elements, check parent select in that case
			else if (element.parentNode.name in this.submitted)
				this.element(element.parentNode);
		},
		highlight: function( element, errorClass, validClass ) {
			$(element).addClass(errorClass).removeClass(validClass);
		},
		unhighlight: function( element, errorClass, validClass ) {
			$(element).removeClass(errorClass).addClass(validClass);
		}
	},

	// http://docs.jquery.com/Plugins/Validation/Validator/setDefaults
	setDefaults: function(settings) {
		$.extend( $.validator.defaults, settings );
	},

	messages: {
		required: "This field is required.",
		remote: "Please fix this field.",
		email: "Please enter a valid email address.",
		url: "Please enter a valid URL.",
		date: "Please enter a valid date.",
		dateISO: "Please enter a valid date (ISO).",
		number: "Please enter a valid number.",
		digits: "Please enter only digits.",
		creditcard: "Please enter a valid credit card number.",
		equalTo: "Please enter the same value again.",
		accept: "Please enter a value with a valid extension.",
		maxlength: $.validator.format("Please enter no more than {0} characters."),
		minlength: $.validator.format("Please enter at least {0} characters."),
		rangelength: $.validator.format("Please enter a value between {0} and {1} characters long."),
		range: $.validator.format("Please enter a value between {0} and {1}."),
		max: $.validator.format("Please enter a value less than or equal to {0}."),
		min: $.validator.format("Please enter a value greater than or equal to {0}.")
	},
	
	autoCreateRanges: false,
	
	prototype: {
		
		init: function() {
			this.labelContainer = $(this.settings.errorLabelContainer);
			this.errorContext = this.labelContainer.length && this.labelContainer || $(this.currentForm);
			this.containers = $(this.settings.errorContainer).add( this.settings.errorLabelContainer );
			this.submitted = {};
			this.valueCache = {};
			this.pendingRequest = 0;
			this.pending = {};
			this.invalid = {};
			this.reset();
			
			var groups = (this.groups = {});
			$.each(this.settings.groups, function(key, value) {
				$.each(value.split(/\s/), function(index, name) {
					groups[name] = key;
				});
			});
			var rules = this.settings.rules;
			$.each(rules, function(key, value) {
				rules[key] = $.validator.normalizeRule(value);
			});
			
			function delegate(event) {
				var validator = $.data(this[0].form, "validator"),
					eventType = "on" + event.type.replace(/^validate/, "");
				validator.settings[eventType] && validator.settings[eventType].call(validator, this[0] );
			}
			$(this.currentForm)
				.validateDelegate(":text, :password, :file, select, textarea", "focusin focusout keyup", delegate)
				.validateDelegate(":radio, :checkbox, select, option", "click", delegate);

			if (this.settings.invalidHandler)
				$(this.currentForm).bind("invalid-form.validate", this.settings.invalidHandler);
		},

		// http://docs.jquery.com/Plugins/Validation/Validator/form
		form: function() {
			this.checkForm();
			$.extend(this.submitted, this.errorMap);
			this.invalid = $.extend({}, this.errorMap);
			if (!this.valid())
				$(this.currentForm).triggerHandler("invalid-form", [this]);
			this.showErrors();
			return this.valid();
		},
		
		checkForm: function() {
			this.prepareForm();
			for ( var i = 0, elements = (this.currentElements = this.elements()); elements[i]; i++ ) {
				this.check( elements[i] );
			}
			return this.valid(); 
		},
		
		// http://docs.jquery.com/Plugins/Validation/Validator/element
		element: function( element ) {
			element = this.clean( element );
			this.lastElement = element;
			this.prepareElement( element );
			this.currentElements = $(element);
			var result = this.check( element );
			if ( result ) {
				delete this.invalid[element.name];
			} else {
				this.invalid[element.name] = true;
			}
			if ( !this.numberOfInvalids() ) {
				// Hide error containers on last error
				this.toHide = this.toHide.add( this.containers );
			}
			this.showErrors();
			return result;
		},

		// http://docs.jquery.com/Plugins/Validation/Validator/showErrors
		showErrors: function(errors) {
			if(errors) {
				// add items to error list and map
				$.extend( this.errorMap, errors );
				this.errorList = [];
				for ( var name in errors ) {
					this.errorList.push({
						message: errors[name],
						element: this.findByName(name)[0]
					});
				}
				// remove items from success list
				this.successList = $.grep( this.successList, function(element) {
					return !(element.name in errors);
				});
			}
			this.settings.showErrors
				? this.settings.showErrors.call( this, this.errorMap, this.errorList )
				: this.defaultShowErrors();
		},
		
		// http://docs.jquery.com/Plugins/Validation/Validator/resetForm
		resetForm: function() {
			if ( $.fn.resetForm )
				$( this.currentForm ).resetForm();
			this.submitted = {};
			this.prepareForm();
			this.hideErrors();
			this.elements().removeClass( this.settings.errorClass );
		},
		
		numberOfInvalids: function() {
			return this.objectLength(this.invalid);
		},
		
		objectLength: function( obj ) {
			var count = 0;
			for ( var i in obj )
				count++;
			return count;
		},
		
		hideErrors: function() {
			this.addWrapper( this.toHide ).hide();
		},
		
		valid: function() {
			return this.size() == 0;
		},
		
		size: function() {
			return this.errorList.length;
		},
		
		focusInvalid: function() {
			if( this.settings.focusInvalid ) {
				try {
					$(this.findLastActive() || this.errorList.length && this.errorList[0].element || [])
					.filter(":visible")
					.focus()
					// manually trigger focusin event; without it, focusin handler isn't called, findLastActive won't have anything to find
					.trigger("focusin");
				} catch(e) {
					// ignore IE throwing errors when focusing hidden elements
				}
			}
		},
		
		findLastActive: function() {
			var lastActive = this.lastActive;
			return lastActive && $.grep(this.errorList, function(n) {
				return n.element.name == lastActive.name;
			}).length == 1 && lastActive;
		},
		
		elements: function() {
			var validator = this,
				rulesCache = {};
			
			// select all valid inputs inside the form (no submit or reset buttons)
			// workaround $Query([]).add until http://dev.jquery.com/ticket/2114 is solved
			return $([]).add(this.currentForm.elements)
			.filter(":input")
			.not(":submit, :reset, :image, [disabled]")
			.not( this.settings.ignore )
			.filter(function() {
				!this.name && validator.settings.debug && window.console && console.error( "%o has no name assigned", this);
			
				// select only the first element for each name, and only those with rules specified
				if ( this.name in rulesCache || !validator.objectLength($(this).rules()) )
					return false;
				
				rulesCache[this.name] = true;
				return true;
			});
		},
		
		clean: function( selector ) {
			return $( selector )[0];
		},
		
		errors: function() {
			return $( this.settings.errorElement + "." + this.settings.errorClass, this.errorContext );
		},
		
		reset: function() {
			this.successList = [];
			this.errorList = [];
			this.errorMap = {};
			this.toShow = $([]);
			this.toHide = $([]);
			this.currentElements = $([]);
		},
		
		prepareForm: function() {
			this.reset();
			this.toHide = this.errors().add( this.containers );
		},
		
		prepareElement: function( element ) {
			this.reset();
			this.toHide = this.errorsFor(element);
		},
	
		check: function( element ) {
			element = this.clean( element );
			
			// if radio/checkbox, validate first element in group instead
			if (this.checkable(element)) {
				element = this.findByName( element.name )[0];
			}
			
			var rules = $(element).rules();
			var dependencyMismatch = false;
			for( method in rules ) {
				var rule = { method: method, parameters: rules[method] };
				try {
					var result = $.validator.methods[method].call( this, element.value.replace(/\r/g, ""), element, rule.parameters );
					
					// if a method indicates that the field is optional and therefore valid,
					// don't mark it as valid when there are no other rules
					if ( result == "dependency-mismatch" ) {
						dependencyMismatch = true;
						continue;
					}
					dependencyMismatch = false;
					
					if ( result == "pending" ) {
						this.toHide = this.toHide.not( this.errorsFor(element) );
						return;
					}
					
					if( !result ) {
						this.formatAndAdd( element, rule );
						return false;
					}
				} catch(e) {
					this.settings.debug && window.console && console.log("exception occured when checking element " + element.id
						 + ", check the '" + rule.method + "' method", e);
					throw e;
				}
			}
			if (dependencyMismatch)
				return;
			if ( this.objectLength(rules) )
				this.successList.push(element);
			return true;
		},
		
		// return the custom message for the given element and validation method
		// specified in the element's "messages" metadata
		customMetaMessage: function(element, method) {
			if (!$.metadata)
				return;
			
			var meta = this.settings.meta
				? $(element).metadata()[this.settings.meta]
				: $(element).metadata();
			
			return meta && meta.messages && meta.messages[method];
		},
		
		// return the custom message for the given element name and validation method
		customMessage: function( name, method ) {
			var m = this.settings.messages[name];
			return m && (m.constructor == String
				? m
				: m[method]);
		},
		
		// return the first defined argument, allowing empty strings
		findDefined: function() {
			for(var i = 0; i < arguments.length; i++) {
				if (arguments[i] !== undefined)
					return arguments[i];
			}
			return undefined;
		},
		
		defaultMessage: function( element, method) {
			return this.findDefined(
				this.customMessage( element.name, method ),
				this.customMetaMessage( element, method ),
				// title is never undefined, so handle empty string as undefined
				!this.settings.ignoreTitle && element.title || undefined,
				$.validator.messages[method],
				"<strong>Warning: No message defined for " + element.name + "</strong>"
			);
		},
		
		formatAndAdd: function( element, rule ) {
			var message = this.defaultMessage( element, rule.method ),
				theregex = /\$?\{(\d+)\}/g;
			if ( typeof message == "function" ) {
				message = message.call(this, rule.parameters, element);
			} else if (theregex.test(message)) {
				message = jQuery.format(message.replace(theregex, '{$1}'), rule.parameters);
			}			
			this.errorList.push({
				message: message,
				element: element
			});
			
			this.errorMap[element.name] = message;
			this.submitted[element.name] = message;
		},
		
		addWrapper: function(toToggle) {
			if ( this.settings.wrapper )
				toToggle = toToggle.add( toToggle.parent( this.settings.wrapper ) );
			return toToggle;
		},
		
		defaultShowErrors: function() {
			for ( var i = 0; this.errorList[i]; i++ ) {
				var error = this.errorList[i];
				this.settings.highlight && this.settings.highlight.call( this, error.element, this.settings.errorClass, this.settings.validClass );
				this.showLabel( error.element, error.message );
			}
			if( this.errorList.length ) {
				this.toShow = this.toShow.add( this.containers );
			}
			if (this.settings.success) {
				for ( var i = 0; this.successList[i]; i++ ) {
					this.showLabel( this.successList[i] );
				}
			}
			if (this.settings.unhighlight) {
				for ( var i = 0, elements = this.validElements(); elements[i]; i++ ) {
					this.settings.unhighlight.call( this, elements[i], this.settings.errorClass, this.settings.validClass );
				}
			}
			this.toHide = this.toHide.not( this.toShow );
			this.hideErrors();
			this.addWrapper( this.toShow ).show();
		},
		
		validElements: function() {
			return this.currentElements.not(this.invalidElements());
		},
		
		invalidElements: function() {
			return $(this.errorList).map(function() {
				return this.element;
			});
		},
		
		showLabel: function(element, message) {
			var label = this.errorsFor( element );
			if ( label.length ) {
				// refresh error/success class
				label.removeClass().addClass( this.settings.errorClass );
			
				// check if we have a generated label, replace the message then
				label.attr("generated") && label.html(message);
			} else {
				// create label
				label = $("<" + this.settings.errorElement + "/>")
					.attr({"for":  this.idOrName(element), generated: true})
					.addClass(this.settings.errorClass)
					.html(message || "");
				if ( this.settings.wrapper ) {
					// make sure the element is visible, even in IE
					// actually showing the wrapped element is handled elsewhere
					label = label.hide().show().wrap("<" + this.settings.wrapper + "/>").parent();
				}
				if ( !this.labelContainer.append(label).length )
					this.settings.errorPlacement
						? this.settings.errorPlacement(label, $(element) )
						: label.insertAfter(element);
			}
			if ( !message && this.settings.success ) {
				label.text("");
				typeof this.settings.success == "string"
					? label.addClass( this.settings.success )
					: this.settings.success( label );
			}
			this.toShow = this.toShow.add(label);
		},
		
		errorsFor: function(element) {
			var name = this.idOrName(element);
    		return this.errors().filter(function() {
				return $(this).attr('for') == name;
			});
		},
		
		idOrName: function(element) {
			return this.groups[element.name] || (this.checkable(element) ? element.name : element.id || element.name);
		},

		checkable: function( element ) {
			return /radio|checkbox/i.test(element.type);
		},
		
		findByName: function( name ) {
			// select by name and filter by form for performance over form.find("[name=...]")
			var form = this.currentForm;
			return $(document.getElementsByName(name)).map(function(index, element) {
				return element.form == form && element.name == name && element  || null;
			});
		},
		
		getLength: function(value, element) {
			switch( element.nodeName.toLowerCase() ) {
			case 'select':
				return $("option:selected", element).length;
			case 'input':
				if( this.checkable( element) )
					return this.findByName(element.name).filter(':checked').length;
			}
			return value.length;
		},
	
		depend: function(param, element) {
			return this.dependTypes[typeof param]
				? this.dependTypes[typeof param](param, element)
				: true;
		},
	
		dependTypes: {
			"boolean": function(param, element) {
				return param;
			},
			"string": function(param, element) {
				return !!$(param, element.form).length;
			},
			"function": function(param, element) {
				return param(element);
			}
		},
		
		optional: function(element) {
			return !$.validator.methods.required.call(this, $.trim(element.value), element) && "dependency-mismatch";
		},
		
		startRequest: function(element) {
			if (!this.pending[element.name]) {
				this.pendingRequest++;
				this.pending[element.name] = true;
			}
		},
		
		stopRequest: function(element, valid) {
			this.pendingRequest--;
			// sometimes synchronization fails, make sure pendingRequest is never < 0
			if (this.pendingRequest < 0)
				this.pendingRequest = 0;
			delete this.pending[element.name];
			if ( valid && this.pendingRequest == 0 && this.formSubmitted && this.form() ) {
				$(this.currentForm).submit();
				this.formSubmitted = false;
			} else if (!valid && this.pendingRequest == 0 && this.formSubmitted) {
				$(this.currentForm).triggerHandler("invalid-form", [this]);
				this.formSubmitted = false;
			}
		},
		
		previousValue: function(element) {
			return $.data(element, "previousValue") || $.data(element, "previousValue", {
				old: null,
				valid: true,
				message: this.defaultMessage( element, "remote" )
			});
		}
		
	},
	
	classRuleSettings: {
		required: {required: true},
		email: {email: true},
		url: {url: true},
		date: {date: true},
		dateISO: {dateISO: true},
		dateDE: {dateDE: true},
		number: {number: true},
		numberDE: {numberDE: true},
		digits: {digits: true},
		creditcard: {creditcard: true}
	},
	
	addClassRules: function(className, rules) {
		className.constructor == String ?
			this.classRuleSettings[className] = rules :
			$.extend(this.classRuleSettings, className);
	},
	
	classRules: function(element) {
		var rules = {};
		var classes = $(element).attr('class');
		classes && $.each(classes.split(' '), function() {
			if (this in $.validator.classRuleSettings) {
				$.extend(rules, $.validator.classRuleSettings[this]);
			}
		});
		return rules;
	},
	
	attributeRules: function(element) {
		var rules = {};
		var $element = $(element);
		
		for (method in $.validator.methods) {
			var value = $element.attr(method);
			if (value) {
				rules[method] = value;
			}
		}
		
		// maxlength may be returned as -1, 2147483647 (IE) and 524288 (safari) for text inputs
		if (rules.maxlength && /-1|2147483647|524288/.test(rules.maxlength)) {
			delete rules.maxlength;
		}
		
		return rules;
	},
	
	metadataRules: function(element) {
		if (!$.metadata) return {};
		
		var meta = $.data(element.form, 'validator').settings.meta;
		return meta ?
			$(element).metadata()[meta] :
			$(element).metadata();
	},
	
	staticRules: function(element) {
		var rules = {};
		var validator = $.data(element.form, 'validator');
		if (validator.settings.rules) {
			rules = $.validator.normalizeRule(validator.settings.rules[element.name]) || {};
		}
		return rules;
	},
	
	normalizeRules: function(rules, element) {
		// handle dependency check
		$.each(rules, function(prop, val) {
			// ignore rule when param is explicitly false, eg. required:false
			if (val === false) {
				delete rules[prop];
				return;
			}
			if (val.param || val.depends) {
				var keepRule = true;
				switch (typeof val.depends) {
					case "string":
						keepRule = !!$(val.depends, element.form).length;
						break;
					case "function":
						keepRule = val.depends.call(element, element);
						break;
				}
				if (keepRule) {
					rules[prop] = val.param !== undefined ? val.param : true;
				} else {
					delete rules[prop];
				}
			}
		});
		
		// evaluate parameters
		$.each(rules, function(rule, parameter) {
			rules[rule] = $.isFunction(parameter) ? parameter(element) : parameter;
		});
		
		// clean number parameters
		$.each(['minlength', 'maxlength', 'min', 'max'], function() {
			if (rules[this]) {
				rules[this] = Number(rules[this]);
			}
		});
		$.each(['rangelength', 'range'], function() {
			if (rules[this]) {
				rules[this] = [Number(rules[this][0]), Number(rules[this][1])];
			}
		});
		
		if ($.validator.autoCreateRanges) {
			// auto-create ranges
			if (rules.min && rules.max) {
				rules.range = [rules.min, rules.max];
				delete rules.min;
				delete rules.max;
			}
			if (rules.minlength && rules.maxlength) {
				rules.rangelength = [rules.minlength, rules.maxlength];
				delete rules.minlength;
				delete rules.maxlength;
			}
		}
		
		// To support custom messages in metadata ignore rule methods titled "messages"
		if (rules.messages) {
			delete rules.messages;
		}
		
		return rules;
	},
	
	// Converts a simple string to a {string: true} rule, e.g., "required" to {required:true}
	normalizeRule: function(data) {
		if( typeof data == "string" ) {
			var transformed = {};
			$.each(data.split(/\s/), function() {
				transformed[this] = true;
			});
			data = transformed;
		}
		return data;
	},
	
	// http://docs.jquery.com/Plugins/Validation/Validator/addMethod
	addMethod: function(name, method, message) {
		$.validator.methods[name] = method;
		$.validator.messages[name] = message != undefined ? message : $.validator.messages[name];
		if (method.length < 3) {
			$.validator.addClassRules(name, $.validator.normalizeRule(name));
		}
	},

	methods: {

		// http://docs.jquery.com/Plugins/Validation/Methods/required
		required: function(value, element, param) {
			// check if dependency is met
			if ( !this.depend(param, element) )
				return "dependency-mismatch";
			switch( element.nodeName.toLowerCase() ) {
			case 'select':
				// could be an array for select-multiple or a string, both are fine this way
				var val = $(element).val();
				return val && val.length > 0;
			case 'input':
				if ( this.checkable(element) )
					return this.getLength(value, element) > 0;
			default:
				return $.trim(value).length > 0;
			}
		},
		
		// http://docs.jquery.com/Plugins/Validation/Methods/remote
		remote: function(value, element, param) {
			if ( this.optional(element) )
				return "dependency-mismatch";
			
			var previous = this.previousValue(element);
			if (!this.settings.messages[element.name] )
				this.settings.messages[element.name] = {};
			previous.originalMessage = this.settings.messages[element.name].remote;
			this.settings.messages[element.name].remote = previous.message;
			
			param = typeof param == "string" && {url:param} || param; 
			
			if ( previous.old !== value ) {
				previous.old = value;
				var validator = this;
				this.startRequest(element);
				var data = {};
				data[element.name] = value;
				$.ajax($.extend(true, {
					url: param,
					mode: "abort",
					port: "validate" + element.name,
					dataType: "json",
					data: data,
					success: function(response) {
						validator.settings.messages[element.name].remote = previous.originalMessage;
						var valid = response === true;
						if ( valid ) {
							var submitted = validator.formSubmitted;
							validator.prepareElement(element);
							validator.formSubmitted = submitted;
							validator.successList.push(element);
							validator.showErrors();
						} else {
							var errors = {};
							var message = (previous.message = response || validator.defaultMessage( element, "remote" ));
							errors[element.name] = $.isFunction(message) ? message(value) : message;
							validator.showErrors(errors);
						}
						previous.valid = valid;
						validator.stopRequest(element, valid);
					}
				}, param));
				return "pending";
			} else if( this.pending[element.name] ) {
				return "pending";
			}
			return previous.valid;
		},

		// http://docs.jquery.com/Plugins/Validation/Methods/minlength
		minlength: function(value, element, param) {
			return this.optional(element) || this.getLength($.trim(value), element) >= param;
		},
		
		// http://docs.jquery.com/Plugins/Validation/Methods/maxlength
		maxlength: function(value, element, param) {
			return this.optional(element) || this.getLength($.trim(value), element) <= param;
		},
		
		// http://docs.jquery.com/Plugins/Validation/Methods/rangelength
		rangelength: function(value, element, param) {
			var length = this.getLength($.trim(value), element);
			return this.optional(element) || ( length >= param[0] && length <= param[1] );
		},
		
		// http://docs.jquery.com/Plugins/Validation/Methods/min
		min: function( value, element, param ) {
			return this.optional(element) || value >= param;
		},
		
		// http://docs.jquery.com/Plugins/Validation/Methods/max
		max: function( value, element, param ) {
			return this.optional(element) || value <= param;
		},
		
		// http://docs.jquery.com/Plugins/Validation/Methods/range
		range: function( value, element, param ) {
			return this.optional(element) || ( value >= param[0] && value <= param[1] );
		},
		
		// http://docs.jquery.com/Plugins/Validation/Methods/email
		email: function(value, element) {
			// contributed by Scott Gonzalez: http://projects.scottsplayground.com/email_address_validation/
			return this.optional(element) || /^((([a-z]|\d|[!#\$%&'\*\+\-\/=\?\^_`{\|}~]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])+(\.([a-z]|\d|[!#\$%&'\*\+\-\/=\?\^_`{\|}~]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])+)*)|((\x22)((((\x20|\x09)*(\x0d\x0a))?(\x20|\x09)+)?(([\x01-\x08\x0b\x0c\x0e-\x1f\x7f]|\x21|[\x23-\x5b]|[\x5d-\x7e]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(\\([\x01-\x09\x0b\x0c\x0d-\x7f]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF]))))*(((\x20|\x09)*(\x0d\x0a))?(\x20|\x09)+)?(\x22)))@((([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])*([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])))\.)+(([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])*([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])))\.?$/i.test(value);
		},
	
		// http://docs.jquery.com/Plugins/Validation/Methods/url
		url: function(value, element) {
			// contributed by Scott Gonzalez: http://projects.scottsplayground.com/iri/
			return this.optional(element) || /^(https?|ftp):\/\/(((([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(%[\da-f]{2})|[!\$&'\(\)\*\+,;=]|:)*@)?(((\d|[1-9]\d|1\d\d|2[0-4]\d|25[0-5])\.(\d|[1-9]\d|1\d\d|2[0-4]\d|25[0-5])\.(\d|[1-9]\d|1\d\d|2[0-4]\d|25[0-5])\.(\d|[1-9]\d|1\d\d|2[0-4]\d|25[0-5]))|((([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])*([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])))\.)+(([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])*([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])))\.?)(:\d*)?)(\/((([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(%[\da-f]{2})|[!\$&'\(\)\*\+,;=]|:|@)+(\/(([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(%[\da-f]{2})|[!\$&'\(\)\*\+,;=]|:|@)*)*)?)?(\?((([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(%[\da-f]{2})|[!\$&'\(\)\*\+,;=]|:|@)|[\uE000-\uF8FF]|\/|\?)*)?(\#((([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(%[\da-f]{2})|[!\$&'\(\)\*\+,;=]|:|@)|\/|\?)*)?$/i.test(value);
		},
        
		// http://docs.jquery.com/Plugins/Validation/Methods/date
		date: function(value, element) {
			return this.optional(element) || !/Invalid|NaN/.test(new Date(value));
		},
	
		// http://docs.jquery.com/Plugins/Validation/Methods/dateISO
		dateISO: function(value, element) {
			return this.optional(element) || /^\d{4}[\/-]\d{1,2}[\/-]\d{1,2}$/.test(value);
		},
	
		// http://docs.jquery.com/Plugins/Validation/Methods/number
		number: function(value, element) {
			return this.optional(element) || /^-?(?:\d+|\d{1,3}(?:,\d{3})+)(?:\.\d+)?$/.test(value);
		},
	
		// http://docs.jquery.com/Plugins/Validation/Methods/digits
		digits: function(value, element) {
			return this.optional(element) || /^\d+$/.test(value);
		},
		
		// http://docs.jquery.com/Plugins/Validation/Methods/creditcard
		// based on http://en.wikipedia.org/wiki/Luhn
		creditcard: function(value, element) {
			if ( this.optional(element) )
				return "dependency-mismatch";
			// accept only digits and dashes
			if (/[^0-9-]+/.test(value))
				return false;
			var nCheck = 0,
				nDigit = 0,
				bEven = false;

			value = value.replace(/\D/g, "");

			for (var n = value.length - 1; n >= 0; n--) {
				var cDigit = value.charAt(n);
				var nDigit = parseInt(cDigit, 10);
				if (bEven) {
					if ((nDigit *= 2) > 9)
						nDigit -= 9;
				}
				nCheck += nDigit;
				bEven = !bEven;
			}

			return (nCheck % 10) == 0;
		},
		
		// http://docs.jquery.com/Plugins/Validation/Methods/accept
		accept: function(value, element, param) {
			param = typeof param == "string" ? param.replace(/,/g, '|') : "png|jpe?g|gif";
			return this.optional(element) || value.match(new RegExp(".(" + param + ")$", "i")); 
		},
		
		// http://docs.jquery.com/Plugins/Validation/Methods/equalTo
		equalTo: function(value, element, param) {
			// bind to the blur event of the target in order to revalidate whenever the target field is updated
			// TODO find a way to bind the event just once, avoiding the unbind-rebind overhead
			var target = $(param).unbind(".validate-equalTo").bind("blur.validate-equalTo", function() {
				$(element).valid();
			});
			return value == target.val();
		}
		
	}
	
});

// deprecated, use $.validator.format instead
$.format = $.validator.format;

})(jQuery);

// ajax mode: abort
// usage: $.ajax({ mode: "abort"[, port: "uniqueport"]});
// if mode:"abort" is used, the previous request on that port (port can be undefined) is aborted via XMLHttpRequest.abort() 
;(function($) {
	var ajax = $.ajax;
	var pendingRequests = {};
	$.ajax = function(settings) {
		// create settings for compatibility with ajaxSetup
		settings = $.extend(settings, $.extend({}, $.ajaxSettings, settings));
		var port = settings.port;
		if (settings.mode == "abort") {
			if ( pendingRequests[port] ) {
				pendingRequests[port].abort();
			}
			return (pendingRequests[port] = ajax.apply(this, arguments));
		}
		return ajax.apply(this, arguments);
	};
})(jQuery);

// provides cross-browser focusin and focusout events
// IE has native support, in other browsers, use event caputuring (neither bubbles)

// provides delegate(type: String, delegate: Selector, handler: Callback) plugin for easier event delegation
// handler is only called when $(event.target).is(delegate), in the scope of the jquery-object for event.target 
;(function($) {
	// only implement if not provided by jQuery core (since 1.4)
	// TODO verify if jQuery 1.4's implementation is compatible with older jQuery special-event APIs
	if (!jQuery.event.special.focusin && !jQuery.event.special.focusout && document.addEventListener) {
		$.each({
			focus: 'focusin',
			blur: 'focusout'	
		}, function( original, fix ){
			$.event.special[fix] = {
				setup:function() {
					this.addEventListener( original, handler, true );
				},
				teardown:function() {
					this.removeEventListener( original, handler, true );
				},
				handler: function(e) {
					arguments[0] = $.event.fix(e);
					arguments[0].type = fix;
					return $.event.handle.apply(this, arguments);
				}
			};
			function handler(e) {
				e = $.event.fix(e);
				e.type = fix;
				return $.event.handle.call(this, e);
			}
		});
	};
	$.extend($.fn, {
		validateDelegate: function(delegate, type, handler) {
			return this.bind(type, function(event) {
				var target = $(event.target);
				if (target.is(delegate)) {
					return handler.apply(target, arguments);
				}
			});
		}
	});
})(jQuery);
/**
 * jGrowl 1.2.4
 *
 * Dual licensed under the MIT (http://www.opensource.org/licenses/mit-license.php)
 * and GPL (http://www.opensource.org/licenses/gpl-license.php) licenses.
 *
 * Written by Stan Lemon <stosh1985@gmail.com>
 * Last updated: 2009.12.13
 *
 * jGrowl is a jQuery plugin implementing unobtrusive userland notifications.  These 
 * notifications function similarly to the Growl Framework available for
 * Mac OS X (http://growl.info).
 *
 * To Do:
 * - Move library settings to containers and allow them to be changed per container
 *
 * Changes in 1.2.4
 * - Fixed IE bug with the close-all button
 * - Fixed IE bug with the filter CSS attribute (special thanks to gotwic)
 * - Update IE opacity CSS
 * - Changed font sizes to use "em", and only set the base style
 *
 * Changes in 1.2.3
 * - The callbacks no longer use the container as context, instead they use the actual notification
 * - The callbacks now receive the container as a parameter after the options parameter
 * - beforeOpen and beforeClose now check the return value, if it's false - the notification does
 *   not continue.  The open callback will also halt execution if it returns false.
 * - Fixed bug where containers would get confused
 * - Expanded the pause functionality to pause an entire container.
 *
 * Changes in 1.2.2
 * - Notification can now be theme rolled for jQuery UI, special thanks to Jeff Chan!
 *
 * Changes in 1.2.1
 * - Fixed instance where the interval would fire the close method multiple times.
 * - Added CSS to hide from print media
 * - Fixed issue with closer button when div { position: relative } is set
 * - Fixed leaking issue with multiple containers.  Special thanks to Matthew Hanlon!
 *
 * Changes in 1.2.0
 * - Added message pooling to limit the number of messages appearing at a given time.
 * - Closing a notification is now bound to the notification object and triggered by the close button.
 *
 * Changes in 1.1.2
 * - Added iPhone styled example
 * - Fixed possible IE7 bug when determining if the ie6 class shoudl be applied.
 * - Added template for the close button, so that it's content could be customized.
 *
 * Changes in 1.1.1
 * - Fixed CSS styling bug for ie6 caused by a mispelling
 * - Changes height restriction on default notifications to min-height
 * - Added skinned examples using a variety of images
 * - Added the ability to customize the content of the [close all] box
 * - Added jTweet, an example of using jGrowl + Twitter
 *
 * Changes in 1.1.0
 * - Multiple container and instances.
 * - Standard $.jGrowl() now wraps $.fn.jGrowl() by first establishing a generic jGrowl container.
 * - Instance methods of a jGrowl container can be called by $.fn.jGrowl(methodName)
 * - Added glue preferenced, which allows notifications to be inserted before or after nodes in the container
 * - Added new log callback which is called before anything is done for the notification
 * - Corner's attribute are now applied on an individual notification basis.
 *
 * Changes in 1.0.4
 * - Various CSS fixes so that jGrowl renders correctly in IE6.
 *
 * Changes in 1.0.3
 * - Fixed bug with options persisting across notifications
 * - Fixed theme application bug
 * - Simplified some selectors and manipulations.
 * - Added beforeOpen and beforeClose callbacks
 * - Reorganized some lines of code to be more readable
 * - Removed unnecessary this.defaults context
 * - If corners plugin is present, it's now customizable.
 * - Customizable open animation.
 * - Customizable close animation.
 * - Customizable animation easing.
 * - Added customizable positioning (top-left, top-right, bottom-left, bottom-right, center)
 *
 * Changes in 1.0.2
 * - All CSS styling is now external.
 * - Added a theme parameter which specifies a secondary class for styling, such
 *   that notifications can be customized in appearance on a per message basis.
 * - Notification life span is now customizable on a per message basis.
 * - Added the ability to disable the global closer, enabled by default.
 * - Added callbacks for when a notification is opened or closed.
 * - Added callback for the global closer.
 * - Customizable animation speed.
 * - jGrowl now set itself up and tears itself down.
 *
 * Changes in 1.0.1:
 * - Removed dependency on metadata plugin in favor of .data()
 * - Namespaced all events
 */
(function($) {

	/** jGrowl Wrapper - Establish a base jGrowl Container for compatibility with older releases. **/
	$.jGrowl = function( m , o ) {
		// To maintain compatibility with older version that only supported one instance we'll create the base container.
		if ( $('#jGrowl').size() == 0 ) 
			$('<div id="jGrowl"></div>').addClass($.jGrowl.defaults.position).appendTo('body');

		// Create a notification on the container.
		$('#jGrowl').jGrowl(m,o);
	};


	/** Raise jGrowl Notification on a jGrowl Container **/
	$.fn.jGrowl = function( m , o ) {
		if ( $.isFunction(this.each) ) {
			var args = arguments;

			return this.each(function() {
				var self = this;

				/** Create a jGrowl Instance on the Container if it does not exist **/
				if ( $(this).data('jGrowl.instance') == undefined ) {
					$(this).data('jGrowl.instance', $.extend( new $.fn.jGrowl(), { notifications: [], element: null, interval: null } ));
					$(this).data('jGrowl.instance').startup( this );
				}

				/** Optionally call jGrowl instance methods, or just raise a normal notification **/
				if ( $.isFunction($(this).data('jGrowl.instance')[m]) ) {
					$(this).data('jGrowl.instance')[m].apply( $(this).data('jGrowl.instance') , $.makeArray(args).slice(1) );
				} else {
					$(this).data('jGrowl.instance').create( m , o );
				}
			});
		};
	};

	$.extend( $.fn.jGrowl.prototype , {

		/** Default JGrowl Settings **/
		defaults: {
			pool: 			0,
			header: 		'',
			group: 			'',
			sticky: 		false,
			position: 		'top-right', // Is this still needed?
			glue: 			'after',
			theme: 			'default',
			corners: 		'10px',
			check: 			250,
			life: 			5000,
			speed: 			'normal',
			easing: 		'swing',
			closer: 		true,
			closeTemplate: '',
			closerTemplate: '<div><img src="/Content/img/button_del.png" alt=""/> close all </div>',
			log: 			function(e,m,o) {},
			beforeOpen: 	function(e,m,o) {},
			open: 			function(e,m,o) {},
			beforeClose: 	function(e,m,o) {},
			close: 			function(e,m,o) {},
			animateOpen: 	{
				opacity: 	'show'
			},
			animateClose: 	{
				opacity: 	'hide'
			}
		},
		
		notifications: [],
		
		/** jGrowl Container Node **/
		element: 	null,
	
		/** Interval Function **/
		interval:   null,
		
		/** Create a Notification **/
		create: 	function( message , o ) {
			var o = $.extend({}, this.defaults, o);

			this.notifications.push({ message: message , options: o });
			
			o.log.apply( this.element , [this.element,message,o] );
		},
		
		render: 		function( notification ) {
			var self = this;
			var message = notification.message;
			var o = notification.options;

			var notification = $(
				'<div class="jGrowl-notification ui-state-highlight ui-corner-all' + 
				((o.group != undefined && o.group != '') ? ' ' + o.group : '') + '">' +
				'<div class="close">' + o.closeTemplate + '</div>' +
				'<div class="header">' + o.header + '</div>' +
				'<div class="message">' + message + '</div></div>'
			).data("jGrowl", o).addClass(o.theme).children('div.close').bind("click.jGrowl", function() {
				$(this).parent().trigger('jGrowl.close');
			}).parent();


			/** Notification Actions **/
			$(notification).bind("mouseover.jGrowl", function() {
				$('div.jGrowl-notification', self.element).data("jGrowl.pause", true);
			}).bind("mouseout.jGrowl", function() {
				$('div.jGrowl-notification', self.element).data("jGrowl.pause", false);
			}).bind('jGrowl.beforeOpen', function() {
				if ( o.beforeOpen.apply( notification , [notification,message,o,self.element] ) != false ) {
					$(this).trigger('jGrowl.open');
				}
			}).bind('jGrowl.open', function() {
				if ( o.open.apply( notification , [notification,message,o,self.element] ) != false ) {
					if ( o.glue == 'after' ) {
						$('div.jGrowl-notification:last', self.element).after(notification);
					} else {
						$('div.jGrowl-notification:first', self.element).before(notification);
					}
					
					$(this).animate(o.animateOpen, o.speed, o.easing, function() {
						// Fixes some anti-aliasing issues with IE filters.
						if ($.browser.msie && (parseInt($(this).css('opacity'), 10) === 1 || parseInt($(this).css('opacity'), 10) === 0))
							this.style.removeAttribute('filter');

						$(this).data("jGrowl").created = new Date();
					});
				}
			}).bind('jGrowl.beforeClose', function() {
				if ( o.beforeClose.apply( notification , [notification,message,o,self.element] ) != false )
					$(this).trigger('jGrowl.close');
			}).bind('jGrowl.close', function() {
				// Pause the notification, lest during the course of animation another close event gets called.
				$(this).data('jGrowl.pause', true);
				$(this).animate(o.animateClose, o.speed, o.easing, function() {
					$(this).remove();
					var close = o.close.apply( notification , [notification,message,o,self.element] );

					if ( $.isFunction(close) )
						close.apply( notification , [notification,message,o,self.element] );
				});
			}).trigger('jGrowl.beforeOpen');
		
			/** Optional Corners Plugin **/
			if ( $.fn.corner != undefined ) $(notification).corner( o.corners );

			/** Add a Global Closer if more than one notification exists **/
			if ( $('div.jGrowl-notification:parent', self.element).size() > 1 && 
				 $('div.jGrowl-closer', self.element).size() == 0 && this.defaults.closer != false ) {
				$(this.defaults.closerTemplate).addClass('jGrowl-closer ui-state-highlight ui-corner-all').addClass(this.defaults.theme)
					.appendTo(self.element).animate(this.defaults.animateOpen, this.defaults.speed, this.defaults.easing)
					.bind("click.jGrowl", function() {
						$(this).siblings().children('div.close').trigger("click.jGrowl");

						if ( $.isFunction( self.defaults.closer ) ) {
							self.defaults.closer.apply( $(this).parent()[0] , [$(this).parent()[0]] );
						}
					});
			};
		},

		/** Update the jGrowl Container, removing old jGrowl notifications **/
		update:	 function() {
			$(this.element).find('div.jGrowl-notification:parent').each( function() {
				if ( $(this).data("jGrowl") != undefined && $(this).data("jGrowl").created != undefined && 
					 ($(this).data("jGrowl").created.getTime() + $(this).data("jGrowl").life)  < (new Date()).getTime() && 
					 $(this).data("jGrowl").sticky != true && 
					 ($(this).data("jGrowl.pause") == undefined || $(this).data("jGrowl.pause") != true) ) {

					// Pause the notification, lest during the course of animation another close event gets called.
					$(this).trigger('jGrowl.beforeClose');
				}
			});

			if ( this.notifications.length > 0 && 
				 (this.defaults.pool == 0 || $(this.element).find('div.jGrowl-notification:parent').size() < this.defaults.pool) )
				this.render( this.notifications.shift() );

			if ( $(this.element).find('div.jGrowl-notification:parent').size() < 2 ) {
				$(this.element).find('div.jGrowl-closer').animate(this.defaults.animateClose, this.defaults.speed, this.defaults.easing, function() {
					$(this).remove();
				});
			}
		},

		/** Setup the jGrowl Notification Container **/
		startup:	function(e) {
			this.element = $(e).addClass('jGrowl').append('<div class="jGrowl-notification"></div>');
			this.interval = setInterval( function() { 
				$(e).data('jGrowl.instance').update(); 
			}, this.defaults.check);
			
			if ($.browser.msie && parseInt($.browser.version) < 7 && !window["XMLHttpRequest"]) {
				$(this.element).addClass('ie6');
			}
		},

		/** Shutdown jGrowl, removing it and clearing the interval **/
		shutdown:   function() {
			$(this.element).removeClass('jGrowl').find('div.jGrowl-notification').remove();
			clearInterval( this.interval );
		},
		
		close: 	function() {
			$(this.element).find('div.jGrowl-notification').each(function(){
				$(this).trigger('jGrowl.beforeClose');
			});
		}
	});
	
	/** Reference the Defaults Object for compatibility with older versions of jGrowl **/
	$.jGrowl.defaults = $.fn.jGrowl.prototype.defaults;

})(jQuery);/*
 * jQuery JSON Plugin
 * version: 2.1 (2009-08-14)
 *
 * This document is licensed as free software under the terms of the
 * MIT License: http://www.opensource.org/licenses/mit-license.php
 *
 * Brantley Harris wrote this plugin. It is based somewhat on the JSON.org 
 * website's http://www.json.org/json2.js, which proclaims:
 * "NO WARRANTY EXPRESSED OR IMPLIED. USE AT YOUR OWN RISK.", a sentiment that
 * I uphold.
 *
 * It is also influenced heavily by MochiKit's serializeJSON, which is 
 * copyrighted 2005 by Bob Ippolito.
 */
 
(function($) {
    /** jQuery.toJSON( json-serializble )
        Converts the given argument into a JSON respresentation.

        If an object has a "toJSON" function, that will be used to get the representation.
        Non-integer/string keys are skipped in the object, as are keys that point to a function.

        json-serializble:
            The *thing* to be converted.
     **/
    $.toJSON = function(o)
    {
        if (typeof(JSON) == 'object' && JSON.stringify)
            return JSON.stringify(o);
        
        var type = typeof(o);
    
        if (o === null)
            return "null";
    
        if (type == "undefined")
            return undefined;
        
        if (type == "number" || type == "boolean")
            return o + "";
    
        if (type == "string")
            return $.quoteString(o);
    
        if (type == 'object')
        {
            if (typeof o.toJSON == "function") 
                return $.toJSON( o.toJSON() );
            
            if (o.constructor === Date)
            {
                var month = o.getUTCMonth() + 1;
                if (month < 10) month = '0' + month;

                var day = o.getUTCDate();
                if (day < 10) day = '0' + day;

                var year = o.getUTCFullYear();
                
                var hours = o.getUTCHours();
                if (hours < 10) hours = '0' + hours;
                
                var minutes = o.getUTCMinutes();
                if (minutes < 10) minutes = '0' + minutes;
                
                var seconds = o.getUTCSeconds();
                if (seconds < 10) seconds = '0' + seconds;
                
                var milli = o.getUTCMilliseconds();
                if (milli < 100) milli = '0' + milli;
                if (milli < 10) milli = '0' + milli;

                return '"' + year + '-' + month + '-' + day + 'T' +
                             hours + ':' + minutes + ':' + seconds + 
                             '.' + milli + 'Z"'; 
            }

            if (o.constructor === Array) 
            {
                var ret = [];
                for (var i = 0; i < o.length; i++)
                    ret.push( $.toJSON(o[i]) || "null" );

                return "[" + ret.join(",") + "]";
            }
        
            var pairs = [];
            for (var k in o) {
                var name;
                var type = typeof k;

                if (type == "number")
                    name = '"' + k + '"';
                else if (type == "string")
                    name = $.quoteString(k);
                else
                    continue;  //skip non-string or number keys
            
                if (typeof o[k] == "function") 
                    continue;  //skip pairs where the value is a function.
            
                var val = $.toJSON(o[k]);
            
                pairs.push(name + ":" + val);
            }

            return "{" + pairs.join(", ") + "}";
        }
    };

    /** jQuery.evalJSON(src)
        Evaluates a given piece of json source.
     **/
    $.evalJSON = function(src)
    {
        if (typeof(JSON) == 'object' && JSON.parse)
            return JSON.parse(src);
        return eval("(" + src + ")");
    };
    
    /** jQuery.secureEvalJSON(src)
        Evals JSON in a way that is *more* secure.
    **/
    $.secureEvalJSON = function(src)
    {
        if (typeof(JSON) == 'object' && JSON.parse)
            return JSON.parse(src);
        
        var filtered = src;
        filtered = filtered.replace(/\\["\\\/bfnrtu]/g, '@');
        filtered = filtered.replace(/"[^"\\\n\r]*"|true|false|null|-?\d+(?:\.\d*)?(?:[eE][+\-]?\d+)?/g, ']');
        filtered = filtered.replace(/(?:^|:|,)(?:\s*\[)+/g, '');
        
        if (/^[\],:{}\s]*$/.test(filtered))
            return eval("(" + src + ")");
        else
            throw new SyntaxError("Error parsing JSON, source is not valid.");
    };

    /** jQuery.quoteString(string)
        Returns a string-repr of a string, escaping quotes intelligently.  
        Mostly a support function for toJSON.
    
        Examples:
            >>> jQuery.quoteString("apple")
            "apple"
        
            >>> jQuery.quoteString('"Where are we going?", she asked.')
            "\"Where are we going?\", she asked."
     **/
    $.quoteString = function(string)
    {
        if (string.match(_escapeable))
        {
            return '"' + string.replace(_escapeable, function (a) 
            {
                var c = _meta[a];
                if (typeof c === 'string') return c;
                c = a.charCodeAt();
                return '\\u00' + Math.floor(c / 16).toString(16) + (c % 16).toString(16);
            }) + '"';
        }
        return '"' + string + '"';
    };
    
    var _escapeable = /["\\\x00-\x1f\x7f-\x9f]/g;
    
    var _meta = {
        '\b': '\\b',
        '\t': '\\t',
        '\n': '\\n',
        '\f': '\\f',
        '\r': '\\r',
        '"' : '\\"',
        '\\': '\\\\'
    };
})(jQuery);
/*
 * jQuery Tooltip plugin 1.3
 *
 * http://bassistance.de/jquery-plugins/jquery-plugin-tooltip/
 * http://docs.jquery.com/Plugins/Tooltip
 *
 * Copyright (c) 2006 - 2008 Jörn Zaefferer
 *
 * $Id: jquery.tooltip.js 5741 2008-06-21 15:22:16Z joern.zaefferer $
 * 
 * Dual licensed under the MIT and GPL licenses:
 *   http://www.opensource.org/licenses/mit-license.php
 *   http://www.gnu.org/licenses/gpl.html
 */
 
;(function($) {
	
		// the tooltip element
	var helper = {},
		// the current tooltipped element
		current,
		// the title of the current element, used for restoring
		title,
		// timeout id for delayed tooltips
		tID,
		// IE 5.5 or 6
		IE = $.browser.msie && /MSIE\s(5\.5|6\.)/.test(navigator.userAgent),
		// flag for mouse tracking
		track = false;
	
	$.tooltip = {
		blocked: false,
		defaults: {
			delay: 200,
			fade: false,
			showURL: true,
			extraClass: "",
			top: 15,
			left: 15,
			id: "tooltip"
		},
		block: function() {
			$.tooltip.blocked = !$.tooltip.blocked;
		}
	};
	
	$.fn.extend({
		tooltip: function(settings) {
			settings = $.extend({}, $.tooltip.defaults, settings);
			createHelper(settings);
			return this.each(function() {
					$.data(this, "tooltip", settings);
					this.tOpacity = helper.parent.css("opacity");
					// copy tooltip into its own expando and remove the title
					this.tooltipText = this.title;
					$(this).removeAttr("title");
					// also remove alt attribute to prevent default tooltip in IE
					this.alt = "";
				})
				.mouseover(save)
				.mouseout(hide)
				.click(hide);
		},
		fixPNG: IE ? function() {
			return this.each(function () {
				var image = $(this).css('backgroundImage');
				if (image.match(/^url\(["']?(.*\.png)["']?\)$/i)) {
					image = RegExp.$1;
					$(this).css({
						'backgroundImage': 'none',
						'filter': "progid:DXImageTransform.Microsoft.AlphaImageLoader(enabled=true, sizingMethod=crop, src='" + image + "')"
					}).each(function () {
						var position = $(this).css('position');
						if (position != 'absolute' && position != 'relative')
							$(this).css('position', 'relative');
					});
				}
			});
		} : function() { return this; },
		unfixPNG: IE ? function() {
			return this.each(function () {
				$(this).css({'filter': '', backgroundImage: ''});
			});
		} : function() { return this; },
		hideWhenEmpty: function() {
			return this.each(function() {
				$(this)[ $(this).html() ? "show" : "hide" ]();
			});
		},
		url: function() {
			return this.attr('href') || this.attr('src');
		}
	});
	
	function createHelper(settings) {
		// there can be only one tooltip helper
		if( helper.parent )
			return;
		// create the helper, h3 for title, div for url
		helper.parent = $('<div id="' + settings.id + '"><h3></h3><div class="body"></div><div class="url"></div></div>')
			// add to document
			.appendTo(document.body)
			// hide it at first
			.hide();
			
		// apply bgiframe if available
		if ( $.fn.bgiframe )
			helper.parent.bgiframe();
		
		// save references to title and url elements
		helper.title = $('h3', helper.parent);
		helper.body = $('div.body', helper.parent);
		helper.url = $('div.url', helper.parent);
	}
	
	function settings(element) {
		return $.data(element, "tooltip");
	}
	
	// main event handler to start showing tooltips
	function handle(event) {
		// show helper, either with timeout or on instant
		if( settings(this).delay )
			tID = setTimeout(show, settings(this).delay);
		else
			show();
		
		// if selected, update the helper position when the mouse moves
		track = !!settings(this).track;
		$(document.body).bind('mousemove', update);
			
		// update at least once
		update(event);
	}
	
	// save elements title before the tooltip is displayed
	function save() {
		// if this is the current source, or it has no title (occurs with click event), stop
		if ( $.tooltip.blocked || this == current || (!this.tooltipText && !settings(this).bodyHandler) )
			return;

		// save current
		current = this;
		title = this.tooltipText;
		
		if ( settings(this).bodyHandler ) {
			helper.title.hide();
			var bodyContent = settings(this).bodyHandler.call(this);
			if (bodyContent.nodeType || bodyContent.jquery) {
				helper.body.empty().append(bodyContent)
			} else {
				helper.body.html( bodyContent );
			}
			helper.body.show();
		} else if ( settings(this).showBody ) {
			var parts = title.split(settings(this).showBody);
			helper.title.html(parts.shift()).show();
			helper.body.empty();
			for(var i = 0, part; (part = parts[i]); i++) {
				if(i > 0)
					helper.body.append("<br/>");
				helper.body.append(part);
			}
			helper.body.hideWhenEmpty();
		} else {
			helper.title.html(title).show();
			helper.body.hide();
		}
		
		// if element has href or src, add and show it, otherwise hide it
		if( settings(this).showURL && $(this).url() )
			helper.url.html( $(this).url().replace('http://', '') ).show();
		else 
			helper.url.hide();
		
		// add an optional class for this tip
		helper.parent.addClass(settings(this).extraClass);

		// fix PNG background for IE
		if (settings(this).fixPNG )
			helper.parent.fixPNG();
			
		handle.apply(this, arguments);
	}
	
	// delete timeout and show helper
	function show() {
		tID = null;
		if ((!IE || !$.fn.bgiframe) && settings(current).fade) {
			if (helper.parent.is(":animated"))
				helper.parent.stop().show().fadeTo(settings(current).fade, current.tOpacity);
			else
				helper.parent.is(':visible') ? helper.parent.fadeTo(settings(current).fade, current.tOpacity) : helper.parent.fadeIn(settings(current).fade);
		} else {
			helper.parent.show();
		}
		update();
	}
	
	/**
	 * callback for mousemove
	 * updates the helper position
	 * removes itself when no current element
	 */
	function update(event)	{
		if($.tooltip.blocked)
			return;
		
		if (event && event.target.tagName == "OPTION") {
			return;
		}
		
		// stop updating when tracking is disabled and the tooltip is visible
		if ( !track && helper.parent.is(":visible")) {
			$(document.body).unbind('mousemove', update)
		}
		
		// if no current element is available, remove this listener
		if( current == null ) {
			$(document.body).unbind('mousemove', update);
			return;	
		}
		
		// remove position helper classes
		helper.parent.removeClass("viewport-right").removeClass("viewport-bottom");
		
		var left = helper.parent[0].offsetLeft;
		var top = helper.parent[0].offsetTop;
		if (event) {
			// position the helper 15 pixel to bottom right, starting from mouse position
			left = event.pageX + settings(current).left;
			top = event.pageY + settings(current).top;
			var right='auto';
			if (settings(current).positionLeft) {
				right = $(window).width() - left;
				left = 'auto';
			}
			helper.parent.css({
				left: left,
				right: right,
				top: top
			});
		}
		
		var v = viewport(),
			h = helper.parent[0];
		// check horizontal position
		if (v.x + v.cx < h.offsetLeft + h.offsetWidth) {
			left -= h.offsetWidth + 20 + settings(current).left;
			helper.parent.css({left: left + 'px'}).addClass("viewport-right");
		}
		// check vertical position
		if (v.y + v.cy < h.offsetTop + h.offsetHeight) {
			top -= h.offsetHeight + 20 + settings(current).top;
			helper.parent.css({top: top + 'px'}).addClass("viewport-bottom");
		}
	}
	
	function viewport() {
		return {
			x: $(window).scrollLeft(),
			y: $(window).scrollTop(),
			cx: $(window).width(),
			cy: $(window).height()
		};
	}
	
	// hide helper and restore added classes and the title
	function hide(event) {
		if($.tooltip.blocked)
			return;
		// clear timeout if possible
		if(tID)
			clearTimeout(tID);
		// no more current element
		current = null;
		
		var tsettings = settings(this);
		function complete() {
			helper.parent.removeClass( tsettings.extraClass ).hide().css("opacity", "");
		}
		if ((!IE || !$.fn.bgiframe) && tsettings.fade) {
			if (helper.parent.is(':animated'))
				helper.parent.stop().fadeTo(tsettings.fade, 0, complete);
			else
				helper.parent.stop().fadeOut(tsettings.fade, complete);
		} else
			complete();
		
		if( settings(this).fixPNG )
			helper.parent.unfixPNG();
	}
	
})(jQuery);
/*
 * JQZoom Evolution 1.0.1 - Javascript Image magnifier
 *
 * Copyright (c) Engineer Renzi Marco(www.mind-projects.it)
 *
 * $Date: 12-12-2008
 *
 *	ChangeLog:
 *  
 * $License : GPL,so any change to the code you should copy and paste this section,and would be nice to report this to me(renzi.mrc@gmail.com).
 */
(function ($) {
    $.fn.jqzoom = function (options) {
        var settings = {
            zoomType: 'standard', //standard/reverse/innerzoom
            zoomWidth: 200, 	//zoomed width default width
            zoomHeight: 200, 	//zoomed div default width
            xOffset: 10, 	//zoomed div default offset
            yOffset: 0,
            position: "right", //zoomed div default position,offset position is to the right of the image
            lens: true, //zooming lens over the image,by default is 1;
            lensReset: false,
            imageOpacity: 0.2,
            title: true,
            alwaysOn: false,
            showEffect: 'show',
            hideEffect: 'hide',
            fadeinSpeed: 'fast',
            fadeoutSpeed: 'slow',
            preloadImages: false,
            showPreload: true,
            preloadText: 'Loading zoom',
            preloadPosition: 'center'   //bycss
        };

        //extending options
        options = options || {};
        $.extend(settings, options);


        return this.each(function () {
            var a = $(this);
            var aTitle = a.attr('title'); //variabile per memorizzare il titolo href
            $(a).removeAttr('title');
            $(a).css('outline-style', 'none');


            var img = $("img", this);
            var imageTitle = img.attr('title');
            img.removeAttr('title'); //variabile per memorizzare il titolo immagine


            var smallimage = new Smallimage(img);
            var smallimagedata = {};
            //imageborder
            var btop = 0;
            var bleft = 0;

            var loader = null;     //variabile per memorizzare oggetto loader
            loader = new Loader();

            var ZoomTitle = (trim(aTitle).length > 0) ? aTitle :
			(trim(imageTitle).length > 0) ? imageTitle : null;  //setting zoomtitle
            var ZoomTitleObj = new zoomTitle();

            var largeimage = new Largeimage(a[0].href);

            var lens = new Lens();
            var lensdata = {};
            //lensborder



            var largeimageloaded = false;
            var scale = {}; //rapporto tra immagine grande e piccola scale.x/scale.y
            var stage = null; // quadrato che mostra l'immagine ingrandita
            var running = false; // running = true quando si verifica l'evento che mostra lo zoom(adesso mouseover).
            var mousepos = {};
            var firstime = 0;
            var preloadshow = false;
            var isMouseDown = false;
            var dragstatus = false;
            //loading smallimagedata
            smallimage.loadimage();

            $(this).click(function () { return false; });

            $(this).parent().unbind('mouseenter').bind('mouseenter', function () {
                showNotice();
                return false;
            });

            $(this).parent().unbind('mouseleave').bind('mouseleave', function () {
                deactivate();
                removeNotice();
                return false;
            });

            var noticeVisible = false;
            function showNotice() {
                if (!running && !noticeVisible) {
                    var zoom = $('<div class="zoom"></div>');
                    a.after(zoom);
                    zoom.click(function (e) {
                        mousepos.x = e.pageX;
                        mousepos.y = e.pageY;
                        removeNotice();
                        activate();
                        return false;
                    });
                    noticeVisible = true;
                }
            }

            function removeNotice() {
                if (noticeVisible) {
                    a.next().remove();
                    noticeVisible = false;
                }

            }

            //ALWAYS ON
            if (settings.alwaysOn) {
                setTimeout(function () { activate(); }, 150);
            }

            function activate() {

                if (!running) {

                    //finding border
                    smallimage.findborder();

                    running = true;

                    //rimuovo il titolo al mouseover
                    imageTitle = img.attr('title');
                    img.removeAttr('title');
                    aTitle = a.attr('title');
                    $(a).removeAttr('title');

                    //se non c� creo l'oggetto largeimage
                    if (!largeimage || $.browser.safari) {
                        largeimage = new Largeimage(a[0].href);
                    }

                    //se l'immagine grande non � stata caricata la carico
                    if (!largeimageloaded || $.browser.safari) {
                        largeimage.loadimage();
                    } else {
                        //after preload
                        if (settings.zoomType != 'innerzoom') {
                            stage = new Stage();
                            stage.activate();
                        }
                        lens = new Lens;
                        lens.activate();
                    }

                    //hack per MAC
                    if ($.browser.safari) {
                        if (settings.zoomType != 'innerzoom') //se innerzoom non mostro la finestra dello zoom
                        {
                            stage = new Stage();
                            stage.activate();
                        }
                        if ($('div.jqZoomPup').length <= 0) {
                            lens = new Lens();
                        }
                        //if(settings.zoomType == 'innerzoom'){lens = new Lens()};
                        lens.activate();
                        (settings.alwaysOn) ? lens.center() : lens.setposition(null);
                    }

                    a[0].blur();
                    //alert($('div.jqZoomPup').length);
                    return false;
                }
            }

            function deactivate() {
                if (settings.zoomType == 'reverse' && !settings.alwaysOn) {
                    img.css({ 'opacity': 1 });
                }

                if (!settings.alwaysOn) {
                    //resetting parameters
                    running = false;
                    largeimageloaded = false;
                    $(lens.node).unbind('mousemove');
                    lens.remove();
                    if ($('div.jqZoomWindow').length > 0) {
                        if (stage != null) {
                            stage.remove();
                        }
                    }
                    if ($('div.jqZoomTitle').length > 0) {
                        ZoomTitleObj.remove();
                    }
                    //resetting title
                    img.attr('title', imageTitle);
                    a.attr('title', aTitle);
                    $().unbind();

                    a.unbind('mousemove');
                    //resetto il parametro che mi dice che � la prima volta che mostor lo zoom
                    firstime = 0;
                    //remove ieiframe
                    if (jQuery('.zoom_ieframe').length > 0) {
                        jQuery('.zoom_ieframe').remove();
                    }
                } else {
                    if (settings.lensReset) {
                        switch (settings.zoomType) {
                            case 'innerzoom':
                                largeimage.setcenter();
                                break;
                            default:
                                lens.center();
                                break;
                        }
                    }
                }

                //non so se serve da provare
                if (settings.alwaysOn) {
                    activate();
                }
            };

            //smallimage
            function Smallimage(image) {
                this.node = image[0];

                this.loadimage = function () {
                    this.node.src = image[0].src;
                    if (typeof smallimagedata.top === "undefined") {
                        this.node.onload();
                    }
                };
                this.findborder = function () {
                    var bordertop = '';
                    bordertop = $(img).css('border-top-width');
                    btop = '';
                    var borderleft = '';
                    borderleft = $(img).css('border-left-width');
                    bleft = '';
                    /*if($.browser.msie)
                    {
                    var temp = bordertop.split(' ');

                    bordertop = temp[1];
                    var temp = borderleft.split(' ');
                    borderleft = temp[1];
                    }*/

                    if (bordertop) {
                        for (i = 0; i < 3; i++) {
                            var x = [];
                            x = bordertop.substr(i, 1);

                            if (isNaN(x) == false) {
                                btop = btop + '' + bordertop.substr(i, 1);
                            } else {
                                break;
                            }
                        }
                    }

                    if (borderleft) {
                        for (i = 0; i < 3; i++) {
                            if (!isNaN(borderleft.substr(i, 1))) {
                                bleft = bleft + borderleft.substr(i, 1)
                            } else {
                                break;
                            }
                        }
                    }
                    btop = (btop.length > 0) ? eval(btop) : 0;
                    bleft = (bleft.length > 0) ? eval(bleft) : 0;


                }
                this.node.onload = function () {
                    //setto il cursor e la posizione dell'href


                    a.css({ 'cursor': 'crosshair', 'display': 'block' });

                    if (a.css('position') != 'absolute' && a.parent().css('position')) {
                        a.css({ 'cursor': 'crosshair', 'position': 'relative', 'display': 'block' });
                    }
                    if (a.parent().css('position') != 'absolute') {
                        a.parent().css('position', 'relative');
                        //a.css('position','relative');
                    }
                    else {
                        //a.css('position','relative');
                    }
                    // following did not seem to fix anything
                    /*
                    if ($.browser.safari || $.browser.opera) {
                    $(img).css({ position: 'absolute', top: '0px', left: '0px' });
                    }
                    */
                    
                    /*if(a.css('position')!= 'absolute' && a.parent().css('position'))
                    {
                    a.css({'cursor':'crosshair','position':'relative','display':'block'});
                    }
                    if(a.parent().css('position') != 'absolute')
                    {
                    alert('in');
                    a.parent().css('position','relative');
                    //a.css('position','relative');
                    }
                    else{
                    //a.css('position','relative');
                    }*/



                    /*
                    if(a.parent().css('position') != 'relative' && a.css('position') != 'absolute')
                    {
                    a.css({'cursor':'crosshair','position':'relative','display':'block'});
                    }*/

                    //al docuemnt ready viene caricato l'src quindi viene azionato l'onload e carico tutti i dati
                    smallimagedata.w = $(this).width();
                    smallimagedata.h = $(this).height();


                    //non viene fatta assegnazione alla variabile globale
                    smallimagedata.h = $(this).height();
                    smallimagedata.pos = $(this).offset();
                    smallimagedata.pos.l = $(this).offset().left;
                    smallimagedata.pos.t = $(this).offset().top;
                    smallimagedata.pos.r = smallimagedata.w + smallimagedata.pos.l;
                    smallimagedata.pos.b = smallimagedata.h + smallimagedata.pos.t;

                    //per sicurezza setto l'altezza e la width dell'href
                    a.height(smallimagedata.h);
                    a.width(smallimagedata.w);


                    //PRELOAD IMAGES
                    if (settings.preloadImages) {
                        largeimage.loadimage();
                    }



                };



                return this;
            };



            //Lens
            function Lens() {


                //creating element and adding class
                this.node = document.createElement("div");
                $(this.node).addClass('jqZoomPup');

                this.node.onerror = function () {
                    $(lens.node).remove();
                    lens = new Lens();
                    lens.activate();
                };




                //funzione privata per il caricamento dello zoom
                this.loadlens = function () {


                    switch (settings.zoomType) {
                        case 'reverse':
                            this.image = new Image();
                            this.image.src = smallimage.node.src; // fires off async
                            this.node.appendChild(this.image);
                            $(this.node).css({ 'opacity': 1 });
                            break;
                        case 'innerzoom':

                            this.image = new Image();
                            this.image.src = largeimage.node.src; // fires off async
                            this.node.appendChild(this.image);
                            $(this.node).css({ 'opacity': 1 });
                            break
                        default:
                            break;
                    }



                    switch (settings.zoomType) {
                        case 'innerzoom':
                            lensdata.w = smallimagedata.w;
                            lensdata.h = smallimagedata.h;
                            break;
                        default:
                            lensdata.w = (settings.zoomWidth) / scale.x;
                            lensdata.h = (settings.zoomHeight) / scale.y;
                            break;
                    }

                    $(this.node).css({
                        width: lensdata.w + 'px',
                        height: lensdata.h + 'px',
                        position: 'absolute',
                        /*cursor: 'crosshair',*/
                        display: 'none',
                        //border: '1px solid blue'
                        borderWidth: 1 + 'px'
                    });
                    a.append(this.node);
                }
                return this;
            };

            Lens.prototype.activate = function () {
                //carico la lente
                this.loadlens();

                switch (settings.zoomType) {
                    case 'reverse':
                        img.css({ 'opacity': settings.imageOpacity });

                        (settings.alwaysOn) ? lens.center() : lens.setposition(null);
                        //lens.center();
                        //bindo ad a il mousemove della lente
                        a.bind('mousemove', function (e) {
                            mousepos.x = e.pageX;
                            mousepos.y = e.pageY;
                            lens.setposition(e);
                        });
                        break;
                    case 'innerzoom':

                        //	lens = new Lens();
                        //	lens.activate();

                        $(this.node).css({ top: 0, left: 0 });
                        if (settings.title) {
                            ZoomTitleObj.loadtitle();
                        }

                        largeimage.setcenter();

                        a.bind('mousemove', function (e) {
                            mousepos.x = e.pageX;
                            mousepos.y = e.pageY;
                            largeimage.setinner(e);

                            /*if(settings.zoomType == 'innerzoom' && running)
                            {
                            $(a).mousemove(function(){
                            if($('div.jqZoomPup').length <= 0)
                            {
                            lens = new Lens();
                            lens.activate();
                            }
                            });
                            }*/

                            /*if($('div.jqZoomPup').length <= 0)
                            {
                            lens = new Lens();
                            lens.activate();
                            }*/

                        });
                        break;
                    default:
                        /*$(document).mousemove(function(e){
                        if(isMouseDown && dragstatus != false){
                        lens.setposition( e );
                        }
                        });
                        lens.center()


                        dragstatus = 'on'
                        $(document).mouseup(function(e){
                        if(isMouseDown && dragstatus != false){
                        isMouseDown = false;
                        dragstatus = false;

                        }
                        });

                        $(this.node).mousedown(function(e){
                        $('div.jqZoomPup').css("cursor", "move");
                        $(this.node).css("position", "absolute");

                        // set z-index
                        $(this.node).css("z-index", parseInt( new Date().getTime()/1000 ));
                        if($.browser.safari)
                        {
                        $(a).css("cursor", "move");
                        }
                        isMouseDown    = true;
                        dragstatus = 'on';
                        lens.setposition( e );
                        });
                        */


                        (settings.alwaysOn) ? lens.center() : lens.setposition(null);

                        //bindo ad a il mousemove della lente
                        $(a).bind('mousemove', function (e) {

                            mousepos.x = e.pageX;
                            mousepos.y = e.pageY;
                            lens.setposition(e);
                        });

                        break;
                }


                return this;
            };

            Lens.prototype.setposition = function (e) {


                if (e) {
                    mousepos.x = e.pageX;
                    mousepos.y = e.pageY;
                }

                if (firstime == 0) {
                    var lensleft = (smallimagedata.w) / 2 - (lensdata.w) / 2;
                    var lenstop = (smallimagedata.h) / 2 - (lensdata.h) / 2;
                    //ADDED

                    $('div.jqZoomPup').show()
                    if (settings.lens) {
                        this.node.style.visibility = 'visible';
                    }
                    else {
                        this.node.style.visibility = 'hidden';
                        $('div.jqZoomPup').hide();
                    }
                    //ADDED
                    firstime = 1;

                } else {
                    var lensleft = mousepos.x - smallimagedata.pos.l - (lensdata.w) / 2;
                    var lenstop = mousepos.y - smallimagedata.pos.t - (lensdata.h) / 2;
                }


                //a sinistra
                if (overleft()) {
                    lensleft = 0 + bleft;
                } else
                //a destra
                    if (overright()) {
                        if ($.browser.msie) {
                            lensleft = smallimagedata.w - lensdata.w + bleft + 1;
                        } else {
                            lensleft = smallimagedata.w - lensdata.w + bleft - 1;
                        }


                    }

                //in alto
                if (overtop()) {
                    lenstop = 0 + btop;
                } else
                //sotto
                    if (overbottom()) {

                        if ($.browser.msie) {
                            lenstop = smallimagedata.h - lensdata.h + btop + 1;
                        } else {
                            lenstop = smallimagedata.h - lensdata.h - 1 + btop;
                        }

                    }
                lensleft = parseInt(lensleft);
                lenstop = parseInt(lenstop);

                //setto lo zoom ed un eventuale immagine al centro
                $('div.jqZoomPup', a).css({ top: lenstop, left: lensleft });

                if (settings.zoomType == 'reverse') {
                    $('div.jqZoomPup img', a).css({ 'position': 'absolute', 'top': -(lenstop - btop + 1), 'left': -(lensleft - bleft + 1) });
                }

                this.node.style.left = lensleft + 'px';
                this.node.style.top = lenstop + 'px';

                //setto l'immagine grande
                largeimage.setposition();

                function overleft() {
                    return mousepos.x - (lensdata.w + 2 * 1) / 2 - bleft < smallimagedata.pos.l;
                }

                function overright() {

                    return mousepos.x + (lensdata.w + 2 * 1) / 2 > smallimagedata.pos.r + bleft;
                }

                function overtop() {
                    return mousepos.y - (lensdata.h + 2 * 1) / 2 - btop < smallimagedata.pos.t;
                }

                function overbottom() {
                    return mousepos.y + (lensdata.h + 2 * 1) / 2 > smallimagedata.pos.b + btop;
                }

                return this;
            };


            //mostra la lente al centro dell'immagine
            Lens.prototype.center = function () {
                $('div.jqZoomPup', a).css('display', 'none');
                var lensleft = (smallimagedata.w) / 2 - (lensdata.w) / 2;
                var lenstop = (smallimagedata.h) / 2 - (lensdata.h) / 2;
                this.node.style.left = lensleft + 'px';
                this.node.style.top = lenstop + 'px';
                $('div.jqZoomPup', a).css({ top: lenstop, left: lensleft });

                if (settings.zoomType == 'reverse') {
                    /*if($.browser.safari){
                    alert('safari');
                    alert(2*bleft);
                    $('div.jqZoomPup img',a).css({'position': 'absolute','top': -( lenstop - btop +1) ,'left': -(lensleft - 2*bleft)  });
                    }else
                    {*/
                    $('div.jqZoomPup img', a).css({ 'position': 'absolute', 'top': -(lenstop - btop + 1), 'left': -(lensleft - bleft + 1) });
                    //}
                }

                largeimage.setposition();
                if ($.browser.msie) {
                    $('div.jqZoomPup', a).show();
                } else {
                    setTimeout(function () { $('div.jqZoomPup').fadeIn('fast'); }, 10);
                }
            };


            //ritorna l'offset
            Lens.prototype.getoffset = function () {
                var o = {};
                o.left = parseInt(this.node.style.left);
                o.top = parseInt(this.node.style.top);
                return o;
            };

            //rimuove la lente
            Lens.prototype.remove = function () {

                if (settings.zoomType == 'innerzoom') {
                    $('div.jqZoomPup', a).fadeOut('fast', function () { /*$('div.jqZoomPup img').remove();*/$(this).remove(); });
                } else {
                    //$('div.jqZoomPup img').remove();
                    $('div.jqZoomPup', a).remove();
                }
            };

            Lens.prototype.findborder = function () {
                var bordertop = '';
                bordertop = $('div.jqZoomPup').css('borderTop');
                //alert(bordertop);
                lensbtop = '';
                var borderleft = '';
                borderleft = $('div.jqZoomPup').css('borderLeft');
                lensbleft = '';
                if ($.browser.msie) {
                    var temp = bordertop.split(' ');

                    bordertop = temp[1];
                    var temp = borderleft.split(' ');
                    borderleft = temp[1];
                }

                if (bordertop) {
                    for (i = 0; i < 3; i++) {
                        var x = [];
                        x = bordertop.substr(i, 1);

                        if (isNaN(x) == false) {
                            lensbtop = lensbtop + '' + bordertop.substr(i, 1);
                        } else {
                            break;
                        }
                    }
                }

                if (borderleft) {
                    for (i = 0; i < 3; i++) {
                        if (!isNaN(borderleft.substr(i, 1))) {
                            lensbleft = lensbleft + borderleft.substr(i, 1)
                        } else {
                            break;
                        }
                    }
                }


                lensbtop = (lensbtop.length > 0) ? eval(lensbtop) : 0;
                lensbleft = (lensbleft.length > 0) ? eval(lensbleft) : 0;
            }

            //LARGEIMAGE
            function Largeimage(url) {
                this.url = url;
                this.node = new Image();

                /*if(settings.preloadImages)
                {
                preload.push(new Image());
                preload.slice(-1).src = url ;
                }*/

                this.loadimage = function () {


                    if (!this.node)
                        this.node = new Image();

                    this.node.style.position = 'absolute';
                    this.node.style.display = 'none';
                    this.node.style.left = '-5000px';
                    this.node.style.top = '10px';
                    loader = new Loader();

                    if (settings.showPreload && !preloadshow) {
                        loader.show();
                        preloadshow = true;
                    }

                    document.body.appendChild(this.node);
                    this.node.src = this.url; // fires off async
                }

                this.node.onload = function () {
                    this.style.display = 'block';
                    var w = Math.round($(this).width());
                    var h = Math.round($(this).height());

                    this.style.display = 'none';

                    //setting scale
                    scale.x = (w / smallimagedata.w);
                    scale.y = (h / smallimagedata.h);





                    if ($('div.preload').length > 0) {
                        $('div.preload').remove();
                    }

                    largeimageloaded = true;

                    if (settings.zoomType != 'innerzoom' && running) {
                        stage = new Stage();
                        stage.activate();
                    }

                    if (running) {
                        //alert('in');
                        lens = new Lens();

                        lens.activate();

                    }
                    //la attivo

                    if ($('div.preload').length > 0) {
                        $('div.preload').remove();
                    }
                }
                return this;
            }


            Largeimage.prototype.setposition = function () {
                this.node.style.left = Math.ceil(-scale.x * parseInt(lens.getoffset().left) + bleft) + 'px';
                this.node.style.top = Math.ceil(-scale.y * parseInt(lens.getoffset().top) + btop) + 'px';
            };

            //setto la posizione dell'immagine grande nel caso di innerzoom
            Largeimage.prototype.setinner = function (e) {
                this.node.style.left = Math.ceil(-scale.x * Math.abs(e.pageX - smallimagedata.pos.l)) + 'px';
                this.node.style.top = Math.ceil(-scale.y * Math.abs(e.pageY - smallimagedata.pos.t)) + 'px';
                $('div.jqZoomPup img', a).css({ 'position': 'absolute', 'top': this.node.style.top, 'left': this.node.style.left });
            };


            Largeimage.prototype.setcenter = function () {
                this.node.style.left = Math.ceil(-scale.x * Math.abs((smallimagedata.w) / 2)) + 'px';
                this.node.style.top = Math.ceil(-scale.y * Math.abs((smallimagedata.h) / 2)) + 'px';


                $('div.jqZoomPup img', a).css({ 'position': 'absolute', 'top': this.node.style.top, 'left': this.node.style.left });
            };


            //STAGE
            function Stage() {

                var leftpos = smallimagedata.pos.l;
                var toppos = smallimagedata.pos.t;
                //creating element and class
                this.node = document.createElement("div");
                $(this.node).addClass('jqZoomWindow');

                $(this.node)
				.css({
				    position: 'absolute',
				    width: Math.round(settings.zoomWidth) + 'px',
				    height: Math.round(settings.zoomHeight) + 'px',
				    display: 'none',
				    zIndex: 10000,
				    overflow: 'hidden'
				});

                //fa il positionamento
                switch (settings.position) {
                    case "right":

                        leftpos = (smallimagedata.pos.r + Math.abs(settings.xOffset) + settings.zoomWidth < screen.width)
				? (smallimagedata.pos.l + smallimagedata.w + Math.abs(settings.xOffset))
				: (smallimagedata.pos.l - settings.zoomWidth - Math.abs(settings.xOffset));

                        topwindow = smallimagedata.pos.t + settings.yOffset + settings.zoomHeight;
                        toppos = (topwindow < screen.height && topwindow > 0)
				? smallimagedata.pos.t + settings.yOffset
				: smallimagedata.pos.t;

                        break;
                    case "left":

                        leftpos = (smallimagedata.pos.l - Math.abs(settings.xOffset) - settings.zoomWidth > 0)
				? (smallimagedata.pos.l - Math.abs(settings.xOffset) - settings.zoomWidth)
				: (smallimagedata.pos.l + smallimagedata.w + Math.abs(settings.xOffset));

                        topwindow = smallimagedata.pos.t + settings.yOffset + settings.zoomHeight;
                        toppos = (topwindow < screen.height && topwindow > 0)
				? smallimagedata.pos.t + settings.yOffset
				: smallimagedata.pos.t;

                        break;
                    case "top":

                        toppos = (smallimagedata.pos.t - Math.abs(settings.yOffset) - settings.zoomHeight > 0)
				? (smallimagedata.pos.t - Math.abs(settings.yOffset) - settings.zoomHeight)
				: (smallimagedata.pos.t + smallimagedata.h + Math.abs(settings.yOffset));


                        leftwindow = smallimagedata.pos.l + settings.xOffset + settings.zoomWidth;
                        leftpos = (leftwindow < screen.width && leftwindow > 0)
				? smallimagedata.pos.l + settings.xOffset
				: smallimagedata.pos.l;

                        break;
                    case "bottom":


                        toppos = (smallimagedata.pos.b + Math.abs(settings.yOffset) + settings.zoomHeight < $('body').height())
				? (smallimagedata.pos.b + Math.abs(settings.yOffset))
				: (smallimagedata.pos.t - settings.zoomHeight - Math.abs(settings.yOffset));


                        leftwindow = smallimagedata.pos.l + settings.xOffset + settings.zoomWidth;
                        leftpos = (leftwindow < screen.width && leftwindow > 0)
				? smallimagedata.pos.l + settings.xOffset
				: smallimagedata.pos.l;

                        break;
                    default:

                        leftpos = (smallimagedata.pos.l + smallimagedata.w + settings.xOffset + settings.zoomWidth < screen.width)
				? (smallimagedata.pos.l + smallimagedata.w + Math.abs(settings.xOffset))
				: (smallimagedata.pos.l - settings.zoomWidth - Math.abs(settings.xOffset));

                        toppos = (smallimagedata.pos.b + Math.abs(settings.yOffset) + settings.zoomHeight < screen.height)
				? (smallimagedata.pos.b + Math.abs(settings.yOffset))
				: (smallimagedata.pos.t - settings.zoomHeight - Math.abs(settings.yOffset));

                        break;
                }

                this.node.style.left = leftpos + 'px';
                this.node.style.top = toppos + 'px';
                return this;
            }


            Stage.prototype.activate = function () {

                if (!this.node.firstChild)
                    this.node.appendChild(largeimage.node);


                if (settings.title) {
                    ZoomTitleObj.loadtitle();
                }



                document.body.appendChild(this.node);


                switch (settings.showEffect) {
                    case 'show':
                        $(this.node).show();
                        break;
                    case 'fadein':
                        $(this.node).fadeIn(settings.fadeinSpeed);
                        break;
                    default:
                        $(this.node).show();
                        break;
                }

                $(this.node).show();

                if ($.browser.msie && $.browser.version < 7) {
                    this.ieframe = $('<iframe class="zoom_ieframe" frameborder="0" src="#"></iframe>')
	          .css({ position: "absolute", left: this.node.style.left, top: this.node.style.top, zIndex: 99, width: settings.zoomWidth, height: settings.zoomHeight })
	          .insertBefore(this.node);
                };


                largeimage.node.style.display = 'block';
            }

            Stage.prototype.remove = function () {
                switch (settings.hideEffect) {
                    case 'hide':
                        $('.jqZoomWindow').remove();
                        break;
                    case 'fadeout':
                        $('.jqZoomWindow').fadeOut(settings.fadeoutSpeed);
                        break;
                    default:
                        $('.jqZoomWindow').remove();
                        break;
                }
            }

            function zoomTitle() {

                this.node = jQuery('<div />')
				.addClass('jqZoomTitle')
				.html('' + ZoomTitle + '');

                this.loadtitle = function () {
                    if (settings.zoomType == 'innerzoom') {
                        $(this.node)
					.css({ position: 'absolute',
					    top: smallimagedata.pos.b + 3,
					    left: (smallimagedata.pos.l + 1),
					    width: smallimagedata.w
					})
					.appendTo('body');
                    } else {
                        $(this.node).appendTo(stage.node);
                    }
                };
            }

            zoomTitle.prototype.remove = function () {
                $('.jqZoomTitle').remove();
            }


            function Loader() {

                this.node = document.createElement("div");
                $(this.node).addClass('preload');
                $(this.node).html(settings.preloadText); //appendo il testo

                $(this.node)
				.appendTo("body")
				.css('visibility', 'hidden');



                this.show = function () {
                    switch (settings.preloadPosition) {
                        case 'center':
                            loadertop = smallimagedata.pos.t + (smallimagedata.h - $(this.node).height()) / 2;
                            loaderleft = smallimagedata.pos.l + (smallimagedata.w - $(this.node).width()) / 2;
                            break;
                        default:
                            var loaderoffset = this.getoffset();
                            loadertop = !isNaN(loaderoffset.top) ? smallimagedata.pos.t + loaderoffset.top : smallimagedata.pos.t + 0;
                            loaderleft = !isNaN(loaderoffset.left) ? smallimagedata.pos.l + loaderoffset.left : smallimagedata.pos.l + 0;
                            break;
                    }

                    //setting position
                    $(this.node).css({
                        top: loadertop,
                        left: loaderleft,
                        position: 'absolute',
                        visibility: 'visible'
                    });
                }
                return this;
            }

            Loader.prototype.getoffset = function () {
                var o = null;
                o = $('div.preload').offset();
                return o;
            }

        });
    }
})(jQuery);

	function trim(stringa)
	{
	    while (stringa.substring(0,1) == ' '){
	        stringa = stringa.substring(1, stringa.length);
	    }
	    while (stringa.substring(stringa.length-1, stringa.length) == ' '){
	        stringa = stringa.substring(0,stringa.length-1);
	    }
	    return stringa;
	}(function($){

	$.fn.alphanumeric = function(p) { 

		p = $.extend({
			ichars: "!@#$%^&*()+=[]\\\';,/{}|\":<>?~`.- ",
			nchars: "",
			allow: ""
		  }, p);	

		return this.each
			(
				function() 
				{

					if (p.nocaps) p.nchars += "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
					if (p.allcaps) p.nchars += "abcdefghijklmnopqrstuvwxyz";
					
					s = p.allow.split('');
					for ( i=0;i<s.length;i++) if (p.ichars.indexOf(s[i]) != -1) s[i] = "\\" + s[i];
					p.allow = s.join('|');
					
					var reg = new RegExp(p.allow,'gi');
					var ch = p.ichars + p.nchars;
					ch = ch.replace(reg,'');

					$(this).keypress
						(
							function (e)
								{
								
									if (!e.charCode) k = String.fromCharCode(e.which);
										else k = String.fromCharCode(e.charCode);
										
									if (ch.indexOf(k) != -1) e.preventDefault();
									if (e.ctrlKey&&k=='v') e.preventDefault();
									
								}
								
						);
						
					$(this).bind('contextmenu',function () {return false});
									
				}
			);

	};

	$.fn.numeric = function(p) {
	
		var az = "abcdefghijklmnopqrstuvwxyz";
		az += az.toUpperCase();

		p = $.extend({
			nchars: az
		  }, p);	
		  	
		return this.each (function()
			{
				$(this).alphanumeric(p);
			}
		);
			
	};
	
	$.fn.alpha = function(p) {

		var nm = "1234567890";

		p = $.extend({
			nchars: nm
		  }, p);	

		return this.each (function()
			{
				$(this).alphanumeric(p);
			}
		);
			
	};	

})(jQuery);
/**
 * jQuery.ScrollTo
 * Copyright (c) 2007-2009 Ariel Flesler - aflesler(at)gmail(dot)com | http://flesler.blogspot.com
 * Dual licensed under MIT and GPL.
 * Date: 5/25/2009
 *
 * @projectDescription Easy element scrolling using jQuery.
 * http://flesler.blogspot.com/2007/10/jqueryscrollto.html
 * Works with jQuery +1.2.6. Tested on FF 2/3, IE 6/7/8, Opera 9.5/6, Safari 3, Chrome 1 on WinXP.
 *
 * @author Ariel Flesler
 * @version 1.4.2
 *
 * @id jQuery.scrollTo
 * @id jQuery.fn.scrollTo
 * @param {String, Number, DOMElement, jQuery, Object} target Where to scroll the matched elements.
 *	  The different options for target are:
 *		- A number position (will be applied to all axes).
 *		- A string position ('44', '100px', '+=90', etc ) will be applied to all axes
 *		- A jQuery/DOM element ( logically, child of the element to scroll )
 *		- A string selector, that will be relative to the element to scroll ( 'li:eq(2)', etc )
 *		- A hash { top:x, left:y }, x and y can be any kind of number/string like above.
*		- A percentage of the container's dimension/s, for example: 50% to go to the middle.
 *		- The string 'max' for go-to-end. 
 * @param {Number} duration The OVERALL length of the animation, this argument can be the settings object instead.
 * @param {Object,Function} settings Optional set of settings or the onAfter callback.
 *	 @option {String} axis Which axis must be scrolled, use 'x', 'y', 'xy' or 'yx'.
 *	 @option {Number} duration The OVERALL length of the animation.
 *	 @option {String} easing The easing method for the animation.
 *	 @option {Boolean} margin If true, the margin of the target element will be deducted from the final position.
 *	 @option {Object, Number} offset Add/deduct from the end position. One number for both axes or { top:x, left:y }.
 *	 @option {Object, Number} over Add/deduct the height/width multiplied by 'over', can be { top:x, left:y } when using both axes.
 *	 @option {Boolean} queue If true, and both axis are given, the 2nd axis will only be animated after the first one ends.
 *	 @option {Function} onAfter Function to be called after the scrolling ends. 
 *	 @option {Function} onAfterFirst If queuing is activated, this function will be called after the first scrolling ends.
 * @return {jQuery} Returns the same jQuery object, for chaining.
 *
 * @desc Scroll to a fixed position
 * @example $('div').scrollTo( 340 );
 *
 * @desc Scroll relatively to the actual position
 * @example $('div').scrollTo( '+=340px', { axis:'y' } );
 *
 * @dec Scroll using a selector (relative to the scrolled element)
 * @example $('div').scrollTo( 'p.paragraph:eq(2)', 500, { easing:'swing', queue:true, axis:'xy' } );
 *
 * @ Scroll to a DOM element (same for jQuery object)
 * @example var second_child = document.getElementById('container').firstChild.nextSibling;
 *			$('#container').scrollTo( second_child, { duration:500, axis:'x', onAfter:function(){
 *				alert('scrolled!!');																   
 *			}});
 *
 * @desc Scroll on both axes, to different values
 * @example $('div').scrollTo( { top: 300, left:'+=200' }, { axis:'xy', offset:-20 } );
 */
;(function( $ ){
	
	var $scrollTo = $.scrollTo = function( target, duration, settings ){
		$(window).scrollTo( target, duration, settings );
	};

	$scrollTo.defaults = {
		axis:'xy',
		duration: parseFloat($.fn.jquery) >= 1.3 ? 0 : 1
	};

	// Returns the element that needs to be animated to scroll the window.
	// Kept for backwards compatibility (specially for localScroll & serialScroll)
	$scrollTo.window = function( scope ){
		return $(window)._scrollable();
	};

	// Hack, hack, hack :)
	// Returns the real elements to scroll (supports window/iframes, documents and regular nodes)
	$.fn._scrollable = function(){
		return this.map(function(){
			var elem = this,
				isWin = !elem.nodeName || $.inArray( elem.nodeName.toLowerCase(), ['iframe','#document','html','body'] ) != -1;

				if( !isWin )
					return elem;

			var doc = (elem.contentWindow || elem).document || elem.ownerDocument || elem;
			
			return $.browser.safari || doc.compatMode == 'BackCompat' ?
				doc.body : 
				doc.documentElement;
		});
	};

	$.fn.scrollTo = function( target, duration, settings ){
		if( typeof duration == 'object' ){
			settings = duration;
			duration = 0;
		}
		if( typeof settings == 'function' )
			settings = { onAfter:settings };
			
		if( target == 'max' )
			target = 9e9;
			
		settings = $.extend( {}, $scrollTo.defaults, settings );
		// Speed is still recognized for backwards compatibility
		duration = duration || settings.speed || settings.duration;
		// Make sure the settings are given right
		settings.queue = settings.queue && settings.axis.length > 1;
		
		if( settings.queue )
			// Let's keep the overall duration
			duration /= 2;
		settings.offset = both( settings.offset );
		settings.over = both( settings.over );

		return this._scrollable().each(function(){
			var elem = this,
				$elem = $(elem),
				targ = target, toff, attr = {},
				win = $elem.is('html,body');

			switch( typeof targ ){
				// A number will pass the regex
				case 'number':
				case 'string':
					if( /^([+-]=)?\d+(\.\d+)?(px|%)?$/.test(targ) ){
						targ = both( targ );
						// We are done
						break;
					}
					// Relative selector, no break!
					targ = $(targ,this);
				case 'object':
					// DOMElement / jQuery
					if( targ.is || targ.style )
						// Get the real position of the target 
						toff = (targ = $(targ)).offset();
			}
			$.each( settings.axis.split(''), function( i, axis ){
				var Pos	= axis == 'x' ? 'Left' : 'Top',
					pos = Pos.toLowerCase(),
					key = 'scroll' + Pos,
					old = elem[key],
					max = $scrollTo.max(elem, axis);

				if( toff ){// jQuery / DOMElement
					attr[key] = toff[pos] + ( win ? 0 : old - $elem.offset()[pos] );

					// If it's a dom element, reduce the margin
					if( settings.margin ){
						attr[key] -= parseInt(targ.css('margin'+Pos)) || 0;
						attr[key] -= parseInt(targ.css('border'+Pos+'Width')) || 0;
					}
					
					attr[key] += settings.offset[pos] || 0;
					
					if( settings.over[pos] )
						// Scroll to a fraction of its width/height
						attr[key] += targ[axis=='x'?'width':'height']() * settings.over[pos];
				}else{ 
					var val = targ[pos];
					// Handle percentage values
					attr[key] = val.slice && val.slice(-1) == '%' ? 
						parseFloat(val) / 100 * max
						: val;
				}

				// Number or 'number'
				if( /^\d+$/.test(attr[key]) )
					// Check the limits
					attr[key] = attr[key] <= 0 ? 0 : Math.min( attr[key], max );

				// Queueing axes
				if( !i && settings.queue ){
					// Don't waste time animating, if there's no need.
					if( old != attr[key] )
						// Intermediate animation
						animate( settings.onAfterFirst );
					// Don't animate this axis again in the next iteration.
					delete attr[key];
				}
			});

			animate( settings.onAfter );			

			function animate( callback ){
				$elem.animate( attr, duration, settings.easing, callback && function(){
					callback.call(this, target, settings);
				});
			};

		}).end();
	};
	
	// Max scrolling position, works on quirks mode
	// It only fails (not too badly) on IE, quirks mode.
	$scrollTo.max = function( elem, axis ){
		var Dim = axis == 'x' ? 'Width' : 'Height',
			scroll = 'scroll'+Dim;
		
		if( !$(elem).is('html,body') )
			return elem[scroll] - $(elem)[Dim.toLowerCase()]();
		
		var size = 'client' + Dim,
			html = elem.ownerDocument.documentElement,
			body = elem.ownerDocument.body;

		return Math.max( html[scroll], body[scroll] ) 
			 - Math.min( html[size]  , body[size]   );
			
	};

	function both( val ){
		return typeof val == 'object' ? val : { top:val, left:val };
	};

})( jQuery );/* limits chars
*/
(function ($) {
    $.fn.limit = function (limit, element) {
        $.each(this, function () {
            var interval, f;
            var self = $(this);

            $(this).focus(function () {
                interval = window.setInterval(substring, 100);
            });

            $(this).blur(function () {
                clearInterval(interval);
                substring();
            });

            function substring() {
                var val = $(self).val();
                var length = val.length;
                if (length > limit) {
                    $(self).val($(self).val().substring(0, limit));
                }
                if ($(element).html() != limit - length) {
                    $(element).html((limit - length <= 0) ? '0' : limit - length);
                }
            }
            substring();
        });
    };
})(jQuery);/*	
	Watermark plugin for jQuery
	Version: 3.0.4
	http://jquery-watermark.googlecode.com/

	Copyright (c) 2009-2010 Todd Northrop
	http://www.speednet.biz/
	
	January 14, 2010

	Requires:  jQuery 1.2.3+
	
	This program is free software: you can redistribute it and/or modify
	it under the terms of the GNU General Public License as published by
	the Free Software Foundation, either version 3 of the License, or
	(at your option) any later version, subject to the following conditions:
	
	The above copyright notice and this permission notice shall be
	included in all copies or substantial portions of the Software.
	
	This program is distributed in the hope that it will be useful,
	but WITHOUT ANY WARRANTY; without even the implied warranty of
	MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
	GNU General Public License for more details.

	You should have received a copy of the GNU General Public License
	along with this program.  If not, see <http://www.gnu.org/licenses/>.
------------------------------------------------------*/

;(function ($) {

var
	// Will speed up references to undefined
	undefined,

	// String constants for data names
	dataFlag = "watermark",
	dataClass = "watermarkClass",
	dataFocus = "watermarkFocus",
	dataFormSubmit = "watermarkSubmit",
	dataMaxLen = "watermarkMaxLength",
	dataPassword = "watermarkPassword",
	dataText = "watermarkText",
	
	// Includes only elements with watermark defined
	selWatermarkDefined = ":data(" + dataFlag + ")",

	// Includes only elements capable of having watermark
	selWatermarkAble = ":text,:password,:search,textarea",
	
	// triggerFns:
	// Array of function names to look for in the global namespace.
	// Any such functions found will be hijacked to trigger a call to
	// hideAll() any time they are called.  The default value is the
	// ASP.NET function that validates the controls on the page
	// prior to a postback.
	// 
	// Am I missing other important trigger function(s) to look for?
	// Please leave me feedback:
	// http://code.google.com/p/jquery-watermark/issues/list
	triggerFns = [
		"Page_ClientValidate"
	],
	
	// Holds a value of true if a watermark was displayed since the last
	// hideAll() was executed. Avoids repeatedly calling hideAll().
	pageDirty = false;

// Extends jQuery with a custom selector - ":data(...)"
// :data(<name>)  Includes elements that have a specific name defined in the jQuery data collection. (Only the existence of the name is checked; the value is ignored.)
// :data(<name>=<value>)  Includes elements that have a specific jQuery data name defined, with a specific value associated with it.
// :data(<name>!=<value>)  Includes elements that have a specific jQuery data name defined, with a value that is not equal to the value specified.
// :data(<name>^=<value>)  Includes elements that have a specific jQuery data name defined, with a value that starts with the value specified.
// :data(<name>$=<value>)  Includes elements that have a specific jQuery data name defined, with a value that ends with the value specified.
// :data(<name>*=<value>)  Includes elements that have a specific jQuery data name defined, with a value that contains the value specified.
$.extend($.expr[":"], {
	"search": function (elem) {
		return "search" === elem.type;
	},
	
	"data": function (element, index, matches, set) {
		var data, parts = /^((?:[^=!^$*]|[!^$*](?!=))+)(?:([!^$*]?=)(.*))?$/.exec(matches[3]);
		if (parts) {
			data = $(element).data(parts[1]);
			
			if (data !== undefined) {

				if (parts[2]) {
					data = "" + data;
				
					switch (parts[2]) {
						case "=":
							return (data == parts[3]);
						case "!=":
							return (data != parts[3]);
						case "^=":
							return (data.slice(0, parts[3].length) == parts[3]);
						case "$=":
							return (data.slice(-parts[3].length) == parts[3]);
						case "*=":
							return (data.indexOf(parts[3]) !== -1);
					}
				}

				return true;
			}
		}
		
		return false;
	}
});

$.watermark = {

	// Current version number of the plugin
	version: "3.0.4",
		
	// Default options used when watermarks are instantiated.
	// Can be changed to affect the default behavior for all
	// new or updated watermarks.
	// BREAKING CHANGE:  The $.watermark.className
	// property that was present prior to version 3.0.2 must
	// be changed to $.watermark.options.className
	options: {
		
		// Default class name for all watermarks
		className: "watermark",
		
		// If true, plugin will detect and use native browser support for
		// watermarks, if available. (e.g., WebKit's placeholder attribute.)
		useNative: true
	},
	
	// Hide one or more watermarks by specifying any selector type
	// i.e., DOM element, string selector, jQuery matched set, etc.
	hide: function (selector) {
		$(selector).filter(selWatermarkDefined).each(
			function () {
				$.watermark._hide($(this));
			}
		);
	},
	
	// Internal use only.
	_hide: function ($input, focus) {
	
		if ($input.val() == $input.data(dataText)) {
			$input.val("");
			
			// Password type?
			if ($input.data(dataPassword)) {
				
				if ($input.attr("type") === "text") {
					var $pwd = $input.data(dataPassword), 
						$wrap = $input.parent();
						
					$wrap[0].removeChild($input[0]); // Can't use jQuery methods, because they destroy data
					$wrap[0].appendChild($pwd[0]);
					$input = $pwd;
				}
			}
			
			if ($input.data(dataMaxLen)) {
				$input.attr("maxLength", $input.data(dataMaxLen));
				$input.removeData(dataMaxLen);
			}
		
			if (focus) {
				$input.attr("autocomplete", "off");  // Avoid NS_ERROR_XPC_JS_THREW_STRING error in Firefox
				window.setTimeout(
					function () {
						$input.select();  // Fix missing cursor in IE
					}
				, 0);
			}
		}
		
		$input.removeClass($input.data(dataClass));
	},
	
	// Display one or more watermarks by specifying any selector type
	// i.e., DOM element, string selector, jQuery matched set, etc.
	// If conditions are not right for displaying a watermark, ensures that watermark is not shown.
	show: function (selector) {
		$(selector).filter(selWatermarkDefined).each(
			function () {
				$.watermark._show($(this));
			}
		);
	},
	
	// Internal use only.
	_show: function ($input) {
		var val = $input.val(),
			text = $input.data(dataText),
			type = $input.attr("type");

		if (((val.length == 0) || (val == text)) && (!$input.data(dataFocus))) {
			pageDirty = true;
		
			// Password type?
			if ($input.data(dataPassword)) {
				
				if (type === "password") {
					var $wm = $input.data(dataPassword),
						$wrap = $input.parent();
						
					$wrap[0].removeChild($input[0]); // Can't use jQuery methods, because they destroy data
					$wrap[0].appendChild($wm[0]);
					$input = $wm;
					$input.attr("maxLength", text.length);
				}
			}
		
			// Ensure maxLength big enough to hold watermark (input of type="text" or type="search" only)
			if ((type === "text") || (type === "search")) {
				var maxLen = $input.attr("maxLength");
				
				if ((maxLen > 0) && (text.length > maxLen)) {
					$input.data(dataMaxLen, maxLen);
					$input.attr("maxLength", text.length);
				}
			}
            
			$input.addClass($input.data(dataClass));
			$input.val(text);
		}
		else {
			$.watermark._hide($input);
		}
	},
	
	// Hides all watermarks on the current page.
	hideAll: function () {
		if (pageDirty) {
			$.watermark.hide(selWatermarkAble);
			pageDirty = false;
		}
	},
	
	// Displays all watermarks on the current page.
	showAll: function () {
		$.watermark.show(selWatermarkAble);
	}
};

$.fn.watermark = function (text, options) {
	///	<summary>
	///		Set watermark text and class name on all input elements of type="text/password/search" and
	/// 	textareas within the matched set. If className is not specified in options, the default is
	/// 	"watermark". Within the matched set, only input elements with type="text/password/search"
	/// 	and textareas are affected; all other elements are ignored.
	///	</summary>
	///	<returns type="jQuery">
	///		Returns the original jQuery matched set (not just the input and texarea elements).
	/// </returns>
	///	<param name="text" type="String">
	///		Text to display as a watermark when the input or textarea element has an empty value and does not
	/// 	have focus. The first time watermark() is called on an element, if this argument is empty (or not
	/// 	a String type), then the watermark will have the net effect of only changing the class name when
	/// 	the input or textarea element's value is empty and it does not have focus.
	///	</param>
	///	<param name="options" type="Object" optional="true">
	///		Provides the ability to override the default watermark options ($.watermark.options). For backward
	/// 	compatibility, if a string value is supplied, it is used as the class name that overrides the class
	/// 	name in $.watermark.options.className. Properties include:
	/// 		className: When the watermark is visible, the element will be styled using this class name.
	/// 		useNative (Boolean or Function): Specifies if native browser support for watermarks will supersede
	/// 			plugin functionality. If useNative is a function, the return value from the function will
	/// 			determine if native support is used. The function is passed one argument -- a jQuery object
	/// 			containing the element being tested as the only element in its matched set -- and the DOM
	/// 			element being tested is the object on which the function is invoked (the value of "this").
	///	</param>
	/// <remarks>
	///		The effect of changing the text and class name on an input element is called a watermark because
	///		typically light gray text is used to provide a hint as to what type of input is required. However,
	///		the appearance of the watermark can be something completely different: simply change the CSS style
	///		pertaining to the supplied class name.
	///		
	///		The first time watermark() is called on an element, the watermark text and class name are initialized,
	///		and the focus and blur events are hooked in order to control the display of the watermark.  Also, as
	/// 	of version 3.0, drag and drop events are hooked to guard against dropped text being appended to the
	/// 	watermark.  If native watermark support is provided by the browser, it is detected and used, unless
	/// 	the useNative option is set to false.
	///		
	///		Subsequently, watermark() can be called again on an element in order to change the watermark text
	///		and/or class name, and it can also be called without any arguments in order to refresh the display.
	///		
	///		For example, after changing the value of the input or textarea element programmatically, watermark()
	/// 	should be called without any arguments to refresh the display, because the change event is only
	/// 	triggered by user actions, not by programmatic changes to an input or textarea element's value.
	/// 	
	/// 	The one exception to programmatic updates is for password input elements:  you are strongly cautioned
	/// 	against changing the value of a password input element programmatically (after the page loads).
	/// 	The reason is that some fairly hairy code is required behind the scenes to make the watermarks bypass
	/// 	IE security and switch back and forth between clear text (for watermarks) and obscured text (for
	/// 	passwords).  It is *possible* to make programmatic changes, but it must be done in a certain way, and
	/// 	overall it is not recommended.
	/// </remarks>
	
	var hasText = (typeof(text) === "string"), hasClass;
	
	if (typeof(options) === "object") {
		hasClass = (typeof(options.className) === "string");
		options = $.extend({}, $.watermark.options, options);
	}
	else if (typeof(options) === "string") {
		hasClass = true;
		options = $.extend({}, $.watermark.options, {className: options});
	}
	else {
		options = $.watermark.options;
	}
	
	if (typeof(options.useNative) !== "function") {
		options.useNative = options.useNative? function () { return true; } : function () { return false; };
	}
	
	return this.each(
		function () {
			var $input = $(this);
			
			if (!$input.is(selWatermarkAble)) {
				return;
			}
			
			// Watermark already initialized?
			if ($input.data(dataFlag)) {
			
				// If re-defining text or class, first remove existing watermark, then make changes
				if (hasText || hasClass) {
					$.watermark._hide($input);
			
					if (hasText) {
						$input.data(dataText, text);
					}
					
					if (hasClass) {
						$input.data(dataClass, options.className);
					}
				}
			}
			else {
			
				// Detect and use native browser support, if enabled in options
				if (options.useNative.call(this, $input)) {
					
					// Placeholder attribute (WebKit)
					// Big thanks to Opera for the wacky test required
					if ((("" + $input.css("-webkit-appearance")).replace("undefined", "") !== "") && ($input.attr("tagName") !== "TEXTAREA")) {
						
						// className is not set because WebKit doesn't appear to have
						// a separate class name property for placeholders (watermarks).
						if (hasText) {
							$input.attr("placeholder", text);
						}
						
						// Only set data flag for non-native watermarks (purposely commented-out)
						// $input.data(dataFlag, 1);
						return;
					}
				}
				
				$input.data(dataText, hasText? text : "");
				$input.data(dataClass, options.className);
				$input.data(dataFlag, 1); // Flag indicates watermark was initialized
				
				// Special processing for password type
				if ($input.attr("type") === "password") {
					var $wrap = $input.wrap("<span>").parent();
					var $wm = $($wrap.html().replace(/type=["']?password["']?/i, 'type="text"'));
					
					$wm.data(dataText, $input.data(dataText));
					$wm.data(dataClass, $input.data(dataClass));
					$wm.data(dataFlag, 1);
					$wm.attr("maxLength", text.length);
					
					$wm.focus(
						function () {
							$.watermark._hide($wm, true);
						}
					).bind("dragenter",
						function () {
							$.watermark._hide($wm);
						}
					).bind("dragend",
						function () {
							window.setTimeout(function () { $wm.blur(); }, 1);
						}
					);
					$input.blur(
						function () {
							$.watermark._show($input);
						}
					).bind("dragleave",
						function () {
							$.watermark._show($input);
						}
					);
					
					$wm.data(dataPassword, $input);
					$input.data(dataPassword, $wm);
				}
				else {
					
					$input.focus(
						function () {
							$input.data(dataFocus, 1);
							$.watermark._hide($input, true);
						}
					).blur(
						function () {
							$input.data(dataFocus, 0);
							$.watermark._show($input);
						}
					).bind("dragenter",
						function () {
							$.watermark._hide($input);
						}
					).bind("dragleave",
						function () {
							$.watermark._show($input);
						}
					).bind("dragend",
						function () {
							window.setTimeout(function () { $.watermark._show($input); }, 1);
						}
					).bind("drop",
						// Firefox makes this lovely function necessary because the dropped text
						// is merged with the watermark before the drop event is called.
						function (evt) {
							var dropText = evt.originalEvent.dataTransfer.getData("Text");
							
							if ($input.val().replace(dropText, "") === $input.data(dataText)) {
								$input.val(dropText);
							}
							
							$input.focus();
						}
					);
				}
				
				// In order to reliably clear all watermarks before form submission,
				// we need to replace the form's submit function with our own
				// function.  Otherwise watermarks won't be cleared when the form
				// is submitted programmatically.
				if (this.form) {
					var form = this.form,
						$form = $(form);
					
					if (!$form.data(dataFormSubmit)) {
						$form.submit($.watermark.hideAll);
						
						// form.submit exists for all browsers except Google Chrome
						// (see "else" below for explanation)
						if (form.submit) {
							$form.data(dataFormSubmit, form.submit);
							
							form.submit = (function (f, $f) {
								return function () {
									var nativeSubmit = $f.data(dataFormSubmit);
									
									$.watermark.hideAll();
									
									if (nativeSubmit.apply) {
										nativeSubmit.apply(f, Array.prototype.slice.call(arguments));
									}
									else {
										nativeSubmit();
									}
								};
							})(form, $form);
						}
						else {
							$form.data(dataFormSubmit, 1);
							
							// This strangeness is due to the fact that Google Chrome's
							// form.submit function is not visible to JavaScript (identifies
							// as "undefined").  I had to invent a solution here because hours
							// of Googling (ironically) for an answer did not turn up anything
							// useful.  Within my own form.submit function I delete the form's
							// submit function, and then call the non-existent function --
							// which, in the world of Google Chrome, still exists.
							form.submit = (function (f) {
								return function () {
									$.watermark.hideAll();
									delete f.submit;
									f.submit();
								};
							})(form);
						}
					}
				}
			}
			
			$.watermark._show($input);
		}
	).end();
};

// Hijack any functions found in the triggerFns list
if (triggerFns.length) {

	// Wait until DOM is ready before searching
	$(function () {
		var i, name, fn;
	
		for (i=triggerFns.length-1; i>=0; i--) {
			name = triggerFns[i];
			fn = window[name];
			
			if (typeof(fn) === "function") {
				window[name] = (function (origFn) {
					return function () {
						$.watermark.hideAll();
						origFn.apply(null, Array.prototype.slice.call(arguments));
					};
				})(fn);
			}
		}
	});
}

})(jQuery);
/* ------------------------------------------------------------------------
	Class: prettyLoader
	Use: A unified solution for AJAX loader
	Author: Stephane Caron (http://www.no-margin-for-errors.com)
	Version: 1.0.1
------------------------------------------------------------------------- */

(function ($) {
    $.prettyLoader = { version: '1.0.1' };

    $.prettyLoader = function (settings) {
        settings = jQuery.extend({
            animation_speed: 'fast', /* fast/normal/slow/integer */
            bind_to_ajax: true, /* true/false */
            delay: false, /* false OR time in milliseconds (ms) */
            loader: '/Content/img/loading.gif', /* Path to your loader gif */
            offset_top: 13, /* integer */
            offset_left: 10 /* integer */
        }, settings);

        scrollPos = _getScroll();

        imgLoader = new Image();
        imgLoader.onerror = function () {
            alert('Preloader image cannot be loaded. Make sure the path is correct in the settings and that the image is reachable.');
        };
        imgLoader.src = settings.loader;

        if (settings.bind_to_ajax) {
            jQuery(document).ajaxStart(function (e) {
                if (typeof tradelr.hideLoader != "undefined" && !tradelr.hideLoader) {
                    $.prettyLoader.show();
                }
            })
            .ajaxStop(function () {
                $.prettyLoader.hide();
            });
        }

        $.prettyLoader.positionLoader = function (e) {
            e = e ? e : window.event;

            // Set the cursor pos only if the even is returned by the browser.
            cur_x = (e.clientX) ? e.clientX : 0;
            cur_y = (e.clientY) ? e.clientY : 0;

            left_pos = cur_x + settings.offset_left + scrollPos['scrollLeft'];
            top_pos = cur_y + settings.offset_top + scrollPos['scrollTop'];

            $('.prettyLoader').css({
                'top': top_pos,
                'left': left_pos
            });
        };

        $.prettyLoader.show = function (delay) {
            if ($('.prettyLoader').size() > 0) {
                 return;
            }

            // Get the scroll position
            scrollPos = _getScroll();

            // Build the loader container
            $('<div></div>')
				.addClass('prettyLoader')
				.addClass('prettyLoader_' + settings.theme)
				.appendTo('body')
				.hide();

            // No png for IE6...sadly :(
            if ($.browser.msie && $.browser.version == 6)
                $('.prettyLoader').addClass('pl_ie6');

            // Build the loader image
            $('<img />')
				.attr('src', settings.loader)
				.appendTo('.prettyLoader');

            // Show it!
            $('.prettyLoader').fadeIn(settings.animation_speed);

            $(document).bind('click', $.prettyLoader.positionLoader);
            $(document).bind('mousemove', $.prettyLoader.positionLoader);
            $(window).scroll(function () { scrollPos = _getScroll(); $(document).triggerHandler('mousemove'); });

            delay = (delay) ? delay : settings.delay;

            if (delay) {
                setTimeout(function () { $.prettyLoader.hide(); }, delay);
            }
        };

        $.prettyLoader.hide = function () {
            $(document).unbind('click', $.prettyLoader.positionLoader);
            $(document).unbind('mousemove', $.prettyLoader.positionLoader);
            $(window).unbind('scroll');

            $('.prettyLoader').fadeOut(settings.animation_speed, function () {
                $(this).remove();
            });
        };

        function _getScroll() {
            if (self.pageYOffset) {
                return { scrollTop: self.pageYOffset, scrollLeft: self.pageXOffset };
            } else if (document.documentElement && document.documentElement.scrollTop) { // Explorer 6 Strict
                return { scrollTop: document.documentElement.scrollTop, scrollLeft: document.documentElement.scrollLeft };
            } else if (document.body) {// all other Explorers
                return { scrollTop: document.body.scrollTop, scrollLeft: document.body.scrollLeft };
            };
        };

        return this;
    };

})(jQuery);