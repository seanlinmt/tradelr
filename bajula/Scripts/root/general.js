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
    Array.prototype.binarySearch = function (find, comparator) {
        var low = 0, high = this.length - 1, i, comparison;
        while (low <= high) {
            i = Math.floor((low + high) / 2);
            comparison = comparator(this[i], find);
            if (comparison < 0) { low = i + 1; continue; };
            if (comparison > 0) { high = i - 1; continue; };
            return i;
        }
        return -1;
    };
    
    Array.prototype.search = function (find, comparator) {
        for (var i = 0; i < this.length; i++) {
            var comparison = comparator(this[i], find);
            if (comparison) {
                return i;
            }
        }
        return -1;
    };
    
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
            
            $('body')
                .append("<div id='ajax_dialog'><div id='ajax_content'></div></div>")
                .css({ 'overflow': 'hidden' });
            $('#ajax_content').html(result);
            $('#ajax_dialog').dialog({
                autoOpen: false,
                close: function () {
                    dialogBox_close();
                },
                closeOnEscape: true,
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
            $('body')
                .append("<div id='ajax_dialog'><div id='ajax_content'></div></div>")
                .css({ 'overflow': 'hidden' });
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
