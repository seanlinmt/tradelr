<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<tradelr.Models.activity.FacebookActivityViewModel>" %>
<%@ Import Namespace="tradelr.Libraries.Extensions"%>
<% if(Model.requireAdditionalPermission)
   { %>
<div>
<p>Additional permissions are required</p>
<div class="mt10 ac">
<button type='button' class='green' onclick='tradelr.facebook.requestPermission();'>grant additional permissions</button>
</div>
</div>
<% }else if(Model.requireToken){ %>
<div>
<p>You need to link your account with facebook first</p>
<div class="mt10 ac">
<button type='button' class='green' onclick='window.location = "/dashboard/networks#facebook"'>connect with facebook</button>
</div>
</div>
<%}else{%>
<div class="content_filter">
<div id="keywordSearch" class="filter">
<input id="facebooksearchbox" name="facebooksearchbox" type="text" class="w150px" />
<div class="mt4">
<select id="fbsearch_type" class="w150px">
<option value="post">posts</option>
<option value="user">users</option>
<option value="page">pages</option>
</select>
</div>
<div id="fbsearch_user" class="mt4">
<input type="text" class="w150px" />
<input type="hidden" id="fb_friendid" value="" />
</div>
</div>
<div id="facebookPageList" class="filter">
<h4>pages</h4>
<div class="sideboxEntry" fid="">home</div>
<%= Html.FilterBoxList("pageList", Model.pageList)%>
</div>
</div>
<div class="main_columnright">
<div id="sharearea" class="bl pl4">
<textarea rows="2" id="mypost" style="width:560px"></textarea>
<div style="height:35px" class="relative mt4">
<div class="inline-block right absolute"><button id="sendFbPost" type="button" class="mr0">share</button></div>
</div>
</div>
<div class="activity_content">

</div>
<div class="activity_more">
</div>
</div>
<span id="facebook_idfilter" class="hidden"></span>
<%} %>
<script type="text/javascript">
    var tradelr = tradelr || {};
    tradelr.facebook = {};

    var fb_timer;
    var fb_timer_running = false; // use to determine if we are in search mode
    var gettingMoreFbPosts = false;
    var fb_pageoffset = 0;


    // handle side facebook pages
    $('.sideboxEntry', '#facebookPageList').live("click", function () {
        $('#facebookPageList').find('.sideboxEntry').removeClass('selected');
        $(this).addClass('selected');
        var id = $(this).attr('fid');
        $('#facebook_idfilter').html(id);
        tradelr.facebook.reloadFacebookActivityList();
    });

    // handle search
    $('#fbsearch_type').live('change', function () {
        var selected = $(this).val();
        if (selected == 'post') {
            $('#fbsearch_user').fadeIn();
        }
        else {
            $('#fbsearch_user').fadeOut();
        }
    });

    // top facebook share
    $('#sendFbPost', '#facebook_list').live('click', function () {
        var post = $('#mypost').val();
        if (post == '') {
            return false;
        }

        $.post('/fb/send', { data: post, pageid: $('#facebook_idfilter').html() }, function (json_result) {
            if (json_result.success) {
                $('#mypost').val('');
                tradelr.facebook.pollForFacebookPosts(true);
            }
            else {
                $.jGrowl(json_result.message);
            }
        }, 'json');
        return false;
    });

    // handle comment box
    $('#mycomment', '#facebook_list').live('focus', function () {
        $(this).next().show();
    }).live('blur', function () {
        if (facebook_noblur) {
            return false;
        }
        $(this).val('');
        $(this).watermark('write a comment');
        $(this).next().hide();
    });

    // need to prevent blur
    var facebook_noblur = false;
    $('#comment_button', '#facebook_list')
    .live('mouseover', function () { facebook_noblur = true; })
    .live('mouseout', function () { facebook_noblur = false; });

    // handle comment
    $('.comment', '#facebook_list').live("click", function () {
        var row = $(this).parents('.activity_row');
        if ($('.comment_box', row).length != 0) {
            $('#mycomment', row).focus();
            return false;
        }
        var commentbox = ["<div class='comment_box'>",
                        "<textarea rows='1' id='mycomment'></textarea>",
                        "<div class='mt4 ar hidden'><button type='button' id='comment_button' class='small'>comment</button></div>",
                      "</div>"];
        $(row).append(commentbox.join(''));
        inputSelectors_init();
        init_autogrow('.comment_box', 30);
        $('#mycomment', row).focus();
        return false;
    });

    // handle show more comment
    $('.showMoreComments', '#facebook_list').live("click", function () {
        var row = $(this).parents('.activity_row');
        var idStr = $(row).attr('id');
        var id = idStr.substring(idStr.indexOf('_') + 1);

        $.post('/fb/getcomments', { postid: id }, function (json_result) {
            if (json_result.success) {
                $('.showMoreComments', row).fadeOut('fast', function () {
                    $(this).remove();
                    var comments = json_result.data;
                    // skip last two, start backwards
                    for (var i = comments.length - 3; i >= 0; i--) {
                        $('.comments', row).prepend(tradelr.facebook.renderCommentRow(comments[i]));
                    }
                    $('.time span', row).prettyDate();
                });
            }
            else {
                $.jGrowl(json_result.message);
            }
        }, 'json');
        return false;
    });

    // handle post comment
    $('#comment_button', '#facebook_list').live('click', function () {
        var row = $(this).parents('.activity_row');
        var idStr = $(row).attr('id');
        var id = idStr.substring(idStr.indexOf('_') + 1);
        var post = $(row).find('#mycomment').val();
        if (post == '') {
            return false;
        }

        $.post('/fb/sendcomment', { data: post, postid: id }, function (json_result) {
            if (json_result.success) {
                $('.comment_box', row).remove();
                $('.comments', row).append(tradelr.facebook.renderCommentRow(json_result.data));
                $('.time span', row).prettyDate();
            }
            else {
                $.jGrowl(json_result.message);
            }
        }, 'json');
        return false;
    });

    tradelr.facebook.renderCommentRow = function (activityComment) {
        var comment = ['<li>',
        '<div class="comment_photo">',
        '<a class="userlink" href="#" alt="', activityComment.contactLink, '">',
        activityComment.profile_url, '</a>',
        '</div>',
        '<div class="comment_content">',
        '<div>',
        '<span class="commenter">', activityComment.commenter, '</span> ', activityComment.message,
        '</div>',
        '<div class="time">',
        '<span title="', activityComment.created, '"></span>',
        '</div>',
        '</div>',
        '<div class="clear"></div>',
        '</li>'];
        return comment.join('');
    };

    tradelr.facebook.facebook_getPostParams = function () {
        // get since_id = largest id = first one
        var sinceString = $('.activity_row:first .time', '#facebook_list').children('span');
        var since = null;
        var until = null;

        // when there's activities
        if (sinceString.length != 0) {
            since = Date.parse($(sinceString).attr('title')).valueOf() / 1000 - (tradelr.date.getTimezoneOffset() * 60);
        }
        else {
            until = tradelr.date.getLocalTime();
        }

        return {
            id: $('#facebook_idfilter').html(),
            since: since,
            until: until
        };
    };

    tradelr.facebook.initPosts = function () {
        // handles date
        $('.activity_row .time span', '#facebook_list').prettyDate();
        $('.activity_content', '#facebook_list').wrapUrls();
    };

    tradelr.facebook.reloadFacebookActivityList = function () {
        $('.activity_content', '#facebook_list').html('');

        $.ajaxManager.post('/fb/list', tradelr.facebook.facebook_getPostParams(), function (json_result) {
            if (json_result.success) {
                var content = $.trim(json_result.data);
                $('.activity_content', '#facebook_list').html(content);
                tradelr.facebook.initPosts();
            }
            else {
                if (json_result.data == tradelr.returncode.NOTLOGGEDIN) {
                    window.location = '/login';
                }
            }
        }, 'json', false);
        tradelr.facebook.startFacebookTimer();
    };

    tradelr.facebook.pollForFacebookPosts = function (showImmediately) {
        // don't poll if tab is not visible
        if ($('#facebook_list').is(':hidden')) {
            return;
        }

        $.ajaxManager.post('/fb/list', tradelr.facebook.facebook_getPostParams(), function (json_result) {
            if (json_result.success) {
                if (json_result.data == tradelr.returncode.NOTOKEN) {
                    $('.activity_content', '#facebook_list').html(tradelr.facebook.getRequestPermissionHtml());
                    clearInterval(fb_timer);
                }
                else {
                    var activityString = $.trim(json_result.data);
                    if (activityString != "") {
                        var rows = $(activityString);
                        $(rows).wrapUrls();
                        $('.activity_content', '#facebook_list').prepend(rows);
                        $('.time span', '.activity_row').prettyDate();
                    }
                }
            }
            else {
                if (json_result.data == tradelr.returncode.NOTLOGGEDIN) {
                    window.location = '/login';
                }
            }
        }, 'json', !showImmediately);
    };

    $.fn.extend({
        wrapUrls: function () {
            var content = $(this).find('.inline');
            $.each(content, function () {
                var text = $(this).html();
                var modifiedtext = replaceURLWithHTMLLinks(text);
                $(this).html(modifiedtext);
            });
        }
    });

    tradelr.facebook.startFacebookTimer = function () {
        fb_timer = setInterval(function () { tradelr.facebook.pollForFacebookPosts(false); }, 60000);
        fb_timer_running = true;
    };

    tradelr.facebook.stopFacebookTimer = function () {
        clearInterval(fb_timer);
        fb_timer_running = false;
    };

    $(document).ready(function () {
        tradelr.facebook.initFacebook();

        // start activity polling timer
        tradelr.facebook.startFacebookTimer();

        // searchbox
        $('#facebooksearchbox').keyup(function (e) {
            var searchbox = this;
            if (!isEnterKey(e)) {
                return false;
            }

            var searchtype = $('#fbsearch_type').val();
            var fb_friendid = $('#fb_friendid').val();
            if (searchtype == "post" && fb_friendid == '' && $('#fbsearch_user input[type="text"]').val() != '') {
                // unknown friend name
                $.jGrowl('Please specify a valid friend');
                return false;
            }

            fb_pageoffset = 0;
            $(this).attr('disabled', true);
            tradelr.facebook.stopFacebookTimer();
            $('.activity_content', '#facebook_list').html('');
            $('#facebookPageList').find('.sideboxEntry').removeClass('selected');

            var params = {
                q: $('#facebooksearchbox').val(),
                type: searchtype,
                friendid: fb_friendid
            };
            $.ajaxManager.post('/fb/search', params,
            function (json_result) {
                if (json_result.success) {
                    if (json_result.data == tradelr.returncode.NOTOKEN) {
                        $('.activity_content', '#facebook_list').html(tradelr.facebook.getRequestPermissionHtml());
                        clearInterval(fb_timer);
                    }
                    else {
                        var content = $.trim(json_result.data);
                        $('.activity_content', '#facebook_list').html(content);
                        switch (searchtype) {
                            case "page":
                                $.each($('.activity_row', '#facebook_list'), function (i, val) {
                                    (function () {
                                        var row = val;
                                        var idStr = $(row).attr('id');
                                        var id = idStr.substring(idStr.indexOf('_') + 1);
                                        tradelr.ajax.jsonp('http://graph.facebook.com/' + id, function (result) {
                                            $('.caption', row).html(result.mission);
                                            $('.overview', row).html(result.company_overview);
                                            $('.fan_count', row).html(result.fan_count + " fans");
                                            $('.pagelink', row).attr('href', result.link);
                                            $(row).wrapUrls();
                                        });
                                    })();
                                });
                                break;
                            default:
                                tradelr.facebook.initPosts();
                                break;
                        }
                    }
                }
                else {
                    if (json_result.data == tradelr.returncode.NOTLOGGEDIN) {
                        window.location = '/login';
                    }
                }
                $(searchbox).attr('disabled', false);
            }, 'json', false);
        }).watermark('search');

        $('#fbsearch_user input[type="text"]').autocomplete('/fb/friends', {
            dataType: "json",
            parse: function (data) {
                var rows = new Array();
                if (data != null && data.length != null) {
                    for (var i = 0; i < data.length; i++) {
                        rows[i] = { data: data[i], value: data[i].name, result: data[i].name };
                    }
                }
                return rows;
            },
            autoFill: true,
            formatItem: function (row, i, max) {
                return row.name;
            }
        });

        $('#fbsearch_user input[type="text"]').bind('keyup', function () {
            $("#fb_friendid").val('');
        });

        $('#fbsearch_user input[type="text"]').result(function (event, data, formatted) {
            if (data) {
                $("#fb_friendid").val(data.id);
            }
        });

        // get more tweets
        $(document).scroll(function () {
            if (!$('.activity_more', '#facebook_list').is(':visible')) {
                return false;
            }

            var fold = $(window).height() + $(window).scrollTop();
            if ($('.activity_more', '#facebook_list').offset().top - fold > 0) {
                return false;
            }

            // don't get if we're currently getting one or there's there's no posts yet
            if ($('.activity_content', '#facebook_list').children().length == 0 || gettingMoreFbPosts) {
                return false;
            }

            gettingMoreFbPosts = true;
            if (!fb_timer_running) {
                // we are in search mode
                $.jGrowl('retrieving more results');
                var params = {
                    q: $('#facebooksearchbox').val(),
                    type: $('#fbsearch_type').val(),
                    friendid: $('#fb_friendid').val(),
                    offset: fb_pageoffset + 25
                };
                $.ajaxManager.post('/fb/search', params,
            function (json_result) {
                if (json_result.success) {
                    if (json_result.data == tradelr.returncode.NOTOKEN) {
                        $('.activity_content', '#facebook_list').html(tradelr.facebook.getRequestPermissionHtml());
                        clearInterval(fb_timer);
                    }
                    else {
                        var content = $.trim(json_result.data);
                        $('.activity_content', '#facebook_list').append(content);
                        switch ($('#fbsearch_type').val()) {
                            case "page":
                                $.each($('.activity_row', '#facebook_list'), function (i, val) {
                                    // do rows that has not been filled yet
                                    if ($('.fan_count', val).html().indexOf('fans') == -1) {
                                        (function () {
                                            var row = val;
                                            var idStr = $(row).attr('id');
                                            var id = idStr.substring(idStr.indexOf('_') + 1);
                                            tradelr.ajax.jsonp('http://graph.facebook.com/' + id, function (result) {
                                                $('.caption', row).html(result.mission);
                                                $('.overview', row).html(result.company_overview);
                                                $('.fan_count', row).html(result.fan_count + " fans");
                                                $('.pagelink', row).attr('href', result.link);
                                                $(row).wrapUrls();
                                            });
                                        })();
                                    }
                                });
                                break;
                            default:
                                tradelr.facebook.initPosts();
                                break;
                        }
                    }
                }
                else {
                    if (json_result.data == tradelr.returncode.NOTLOGGEDIN) {
                        window.location = '/login';
                    }
                }
                gettingMoreFbPosts = false;
            }, 'json', false);
            }
            else {
                $.jGrowl('retrieving more posts');

                // max_id = last entry
                var untilString = $('.activity_row:last .time', '#facebook_list').children('span');
                var until = null;
                if (untilString.length != 0) {
                    until = Date.parse($(untilString).attr('title')).valueOf() / 1000 - (tradelr.date.getTimezoneOffset() * 60);
                }

                $.ajaxManager.post('/fb/list', { until: until }, function (json_result) {
                    if (json_result.success) {
                        if (json_result.data == tradelr.returncode.NOTOKEN) {
                            $('.activity_content', '#facebook_list').html(tradelr.facebook.getRequestPermissionHtml());
                            clearInterval(fb_timer);
                        }
                        else {
                            var activityString = $.trim(json_result.data);
                            if (activityString == "") {
                                $.jGrowl('No more posts');
                            }
                            else {
                                var rows = $(activityString);
                                $(rows).wrapUrls();
                                $('.activity_content', '#facebook_list').append(rows);
                                $('.time span', '.activity_row').prettyDate();
                            }
                        }
                    }
                    else {
                        if (json_result.data == tradelr.returncode.NOTLOGGEDIN) {
                            window.location = '/login';
                        }
                    }
                    gettingMoreFbPosts = false;
                }, 'json', false);
            }

            return false;
        });
    });

    tradelr.facebook.initFacebook = function () {
        inputSelectors_init();

        $('#fbsearch_user > input[type="text"]').watermark('search only this friend');
        init_autogrow('#sharearea', 30);
    };

    tradelr.facebook.getRequestPermissionHtml = function () {
        return "<button type='button' class='green' onclick='tradelr.facebook.requestPermission();'>grant additional permissions</button>";
    };

    tradelr.facebook.requestPermission = function () {
        window.location = '/fb/permissions?scope=read_stream,publish_stream,manage_pages';
    };
</script>

