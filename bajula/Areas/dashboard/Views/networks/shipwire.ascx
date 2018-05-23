<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<tradelr.Models.networks.viewmodels.NetworkViewModel>" %>
<div class="section_header">
    <img src="/Content/img/social/icons/shipwire_16.png" /> Shipwire
</div>
<div class="form_group">
<div id="connected" class="hidden">
<p><img src="/Content/img/tick.png" /> Connected to Shipwire</p>
<button id="buttonreenter">re-enter credentials</button><button id="buttonlogout">disconnect from shipwire</button>
</div>
<div id="notconnected" class="hidden">
<p>Additional product listing and shipping options will be enabled once you have connected to your Shipwire account.</p>
<button id="buttonlogin" class="green">connect with shipwire</button>
<p class="icon_help mt50">Don't have a Shipwire account? <a href="http://www.shipwire.com/pp/o.php?id=5191" target="_blank">Create one here</a></p>
</div>
</div>
<script type="text/javascript">

    $('#buttonlogin,#buttonreenter').click(function () {
        dialogBox_open('/dashboard/shipwire/credentials');
    });

    $('#buttonlogout').click(function () {
        $.post('/dashboard/shipwire/clear', null, function (json_result) {
            if (json_result.success && json_result.data) {
                $('#connected').fadeOut(function () {
                    $('#notconnected').fadeIn();
                });
            }
            else {
                $.jGrowl('Unable to remove Shipwire credentials');
            }
        }, 'json');
    });

    $(document).ready(function () {
        $.post('/dashboard/shipwire/connected', null, function (json_result) {
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
