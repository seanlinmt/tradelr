<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<tradelr.Models.networks.viewmodels.NetworkViewModel>" %>
<%@ Import Namespace="tradelr.Libraries.Extensions" %>
<div class="section_header">
    <img src="/Content/img/social/icons/blogger_16.png" /> Blogger
</div>
<div class="form_group">
<div id="notconfigured" <%= string.IsNullOrEmpty(Model.bloggerSessionKey)?"":"class='hidden'" %>>
<a href="javascript:void(0)" class="nounderline" id="linkRequest">
<button id="buttonRequest" type="button" class="green" >request permission</button>
</a>
</div>
<div id="configured" <%= !string.IsNullOrEmpty(Model.bloggerSessionKey)?"":"class='hidden'" %>>
<%
    Html.RenderPartial("~/Areas/dashboard/Views/blogger/display.ascx", Model.blogList); %>
<button id="buttonReset" type="button">reset</button>
</div>
<div id="configureblog" class="hidden">
</div>
</div>
<script type="text/javascript">
    $('#linkRequest').click(function () {
        win1 = window.open('<%= Model.requestUrl %>', '', 'width=650px,height=500px,toolbar=0');
        check();
        $('#buttonRequest').attr('disabled', true);
    });

    function check() {
        if (win1.closed) {
            $.post('/dashboard/blogger/haveToken', null, function (json_result) {
                if (json_result.success && json_result.data) {
                    $.post('/dashboard/blogger/list', null, function (json_result) {
                        if (json_result.success) {
                            $('#configureblog').html(json_result.data);
                            $('#notconfigured').fadeOut(function () {
                                $('#configureblog').fadeIn();
                            });
                        }
                        else {
                            $.jGrowl(json_result.message);
                        }
                    }, 'json');
                }
                $('#buttonRequest').attr('disabled', false);
            }, 'json');
        } else {
            setTimeout("check()", 1); 
        }
    }

    $('#blogSave').live('click', function () {
        var checkboxes = $('#configureblog .blockSelectable');
        var blogs = [];
        $.each(checkboxes, function (i, val) {
            if ($(this).hasClass('selected')) {
                var blog = new Object();
                blog.name = $.trim($(this).find('h4 > a').text());
                blog.blogHref = $.trim($(this).find('h4 > a').attr('href'))
                blog.blogUri = $.trim($(this).find('.address').text());
                blogs.push(blog);
            }
        });
        $(this).buttonDisable();
        var encoded = $.toJSON(blogs);

        // post form
        $.ajax({
            contentType: 'application/json',
            type: "POST",
            url: '/dashboard/blogger/saveList',
            dataType: "json",
            data: encoded,
            success: function (json_data) {
                if (json_data.success) {
                    $('#configured').prepend(json_data.data);
                    $('#configureblog').fadeOut(function () {
                        $('#configured').fadeIn();
                    });
                    $.jGrowl('Settings saved!');
                }
                else {
                    $.jGrowl(json_data.message);
                }
                $('#googleSave').buttonEnable();
            }
        }); // ajax
    });

    $('#buttonReset').click(function () {
        $.post('/dashboard/blogger/clear');
        $('#configured').find('div').remove();
        $('#configured').fadeOut(function () {
            $('#notconfigured').fadeIn();
        });
    });
</script>