<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<tradelr.Models.networks.viewmodels.NetworkViewModel>" %>
<div class="section_header">
    <img src="/Content/img/social/icons/wordpress_16.png" /> Wordpress
</div>
<div class="form_group">
<div id="connected" class="hidden">
<p><img src="/Content/img/tick.png" /> Connected to Wordpress</p>
<button id="buttonreenter">re-enter credentials</button><button id="buttonlogout">disconnect from wordpress</button>
</div>
<div id="notconnected" class="hidden">
<button id="buttonlogin" class="green">connect with your wordpress blog</button>
</div>
</div>
<script type="text/javascript">

    $('#buttonlogin,#buttonreenter').click(function () {
        $(this).attr('disabled', true);
        dialogBox_open('/wordpress/credentials');
    });

    $('#buttonlogout').click(function () {
        $.post('/wordpress/clear', null, function (json_result) {
            if (json_result.success && json_result.data) {
                $('#connected').fadeOut(function () {
                    $('#notconnected').fadeIn();
                });
            }
            else {
                $.jGrowl('Unable to remove Wordpress credentials');
            }
        }, 'json');
    });

    $(document).ready(function () {
        $.post('/wordpress/connected', null, function (json_result) {
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
