<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>
<div class="section_header">
    <img src="/Content/img/social/icons/tumblr_16.png" /> Tumblr
</div>
<div class="form_group">
<div id="connected" class="hidden">
<p><img src="/Content/img/tick.png" /> Connected to Tumblr</p>
<button id="buttonreenter">re-enter credentials</button><button id="buttonlogout">disconnect from tumblr</button>
</div>
<div id="notconnected" class="hidden">
<p class="icon_help"> Product will be posted to your Tumblr blog. A Tumblr account will be required.</p>
<button id="buttonlogin" class="green">connect with your tumblr blog</button>
</div>
</div>
<script type="text/javascript">

    $('#buttonlogin,#buttonreenter').click(function () {
        $(this).attr('disabled', true);
        dialogBox_open('/tumblr/credentials');
    });

    $('#buttonlogout').click(function () {
        $.post('/tumblr/clear', null, function (json_result) {
            if (json_result.success && json_result.data) {
                $('#connected').fadeOut(function () {
                    $('#notconnected').fadeIn();
                });
            }
            else {
                $.jGrowl('Unable to remove tumblr credentials');
            }
        }, 'json');
    });

    $(document).ready(function () {
        $.post('/tumblr/connected', null, function (json_result) {
            if (json_result.success) {
                if (json_result.data) {
                    $('#connected').show();
                }
                else {
                    $('#notconnected').show();
                }
            }
            else {
                $.jGrowl(json_result.message);
            }
        }, 'json');

    });
</script>
