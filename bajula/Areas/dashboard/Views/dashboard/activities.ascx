<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<tradelr.Models.activity.TradelrActivity>" %>
<%@ Import Namespace="tradelr.Libraries.Extensions"%>
<div class="content_filter">
<div id="activityTypeList" class="filter">
    <h4>
        activity type</h4>
    <div class="sideboxEntry selected">
        All</div>
    <%= Html.FilterBoxList("activityTypeList", Model.filterList)%>
</div>
</div>
<div class="main_columnright">
<div class="activity_content">
<% Html.RenderPartial("activityList", Model.activities); %>
</div>
<div class="activity_more">
</div>
</div>
<span id="filterBy" class="hidden"></span>
<script type="text/javascript">
    var activityPage = 1;
    var gettingMoreActivities = false;
    var noMoreActivities = false;
    $(document).ready(function () {
        initActivityList();

        // bind side filter click event
        $('#activityTypeList').find('.sideboxEntry').live("click", function () {
            // remove select class and highlight current one
            $('#activityTypeList').find('.sideboxEntry').removeClass('selected');
            $(this).addClass('selected');

            // reload activities
            if ($(this).attr('fid') != undefined) {
                setFilterByField($(this).attr('fid'));
            }
            else {
                setFilterByField('');
            }
            // reset page count
            activityPage = 1;
            noMoreActivities = false;
            reloadActivityList(getFilterByField());
        });

        // start activity polling timer
        setInterval(function () {
            // don't poll if tab is not visible
            if ($('#activity_list').is(':hidden')) {
                return;
            }

            // get since_id = largest id = first one
            var since_idString = $('.activity_row:first', '#activity_list').attr('id');
            var since_id;

            // when there's no activities
            if (since_idString == undefined) {
                since_id = 0;
            }
            else {
                since_id = since_idString.substring(since_idString.indexOf('_') + 1);
            }

            $.ajaxManager.post('/dashboard/activity/list', { since_id: since_id, type: getFilterByField() }, function (json_result) {
                if (json_result.success) {
                    var activityString = $.trim(json_result.data);
                    if (activityString != "") {
                        $('.activity_content', '#activity_list').prepend(activityString);
                        $('.time span', '.activity_row').prettyDate();
                    }
                }
                else {
                    if (json_result.data == tradelr.returncode.NOTLOGGEDIN) {
                        window.location = '/login';
                    }
                }
            }, 'json', true, true);
        }, 60000);

    });

    // get more activities
    $(document).scroll(function () {
        if (noMoreActivities) {
            return false;
        }
        if (!$('.activity_more', '#activity_list').is(':visible')) {
            return false;
        }

        var fold = $(window).height() + $(window).scrollTop();
        if ($('.activity_more', '#activity_list').offset().top - fold > 0) {
            return false;
        }

        // don't get if we're currently getting one or there's there's no tweets yet
        if ($('.activity_content', '#activity_list').children().length == 0 || gettingMoreActivities) {
            return false;
        }

        gettingMoreActivities = true;
        $.jGrowl('Getting more activities');

        // max_id = last entry
        var max_idString = $('.activity_row:last', '#activity_list').attr('id');
        var max_id;
        if (max_idString == undefined) {
            max_id = 0;
        }
        else {
            max_id = max_idString.substring(max_idString.indexOf('_') + 1);
        }

        $.ajaxManager.post('/dashboard/activity/list', { max_id: max_id, type: getFilterByField(), page: activityPage }, function (json_result) {
            if (json_result.success) {
                var activityString = $.trim(json_result.data);
                if (activityString == "") {
                    noMoreActivities = true;
                    $.jGrowl('No more activities');
                }
                else {
                    $('.activity_content', '#activity_list').append(activityString);
                    $('.time span', '.activity_row').prettyDate();
                }
            }
            else {
                if (json_result.data == tradelr.returncode.NOTLOGGEDIN) {
                    window.location = '/login';
                }
            }
            gettingMoreActivities = false;
        }, 'json', false, true);
        activityPage++;
        return false;
    });

    function initActivityList() {
        // handle deletes
        $('.del', '.activity_row').click(function () {
            var idStr = $(this).parent('.activity_row').attr('id');
            var id = idStr.substring(idStr.indexOf('_') + 1);
            var ok = window.confirm('Sure you want to delete this activity? There is NO undo!');
            if (!ok) {
                return false;
            }
            var row = $(this).parent('.activity_row');
            $.post('/activity/destroy',
                'id=' + id,
                function (json_data) {
                    $(row).fadeOut(function () {
                        $(row).remove();
                    });
                });
        });

        // handles date
        $('.time span', '.activity_row').prettyDate();
    }

    function reloadActivityList(filterBy) {
        $.post('/dashboard/activity/list?type=' + filterBy, null, function (json_result) {
            if (json_result.success) {
                var content = $.trim(json_result.data);
                $('.activity_content', '#activity_list').html(content);
                initActivityList();
            }
            else {
                if (json_result.data == tradelr.returncode.NOTLOGGEDIN) {
                    window.location = '/login';
                }
            }
        }, 'json', false, true);
    }
</script>

