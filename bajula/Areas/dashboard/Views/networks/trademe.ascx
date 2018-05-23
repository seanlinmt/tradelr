<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<tradelr.Models.networks.viewmodels.TrademeNetworkViewModel>" %>
<div class="section_header">
    <img src="/Content/img/social/icons/trademe_16.png" /> Trademe
</div>
<div class="form_group">
<div id="connected" class="hidden">
    <p><%= Model.trademeProfileUrl %></p>
    <div class="form_entry">
    <div class="form_label">
        <label for="email">
            Last Sync</label>
            <span class="tip">Synchronisation is automatically carried out every hour</span>
    </div>
    <div id="trademe_lastsync">
        <span title="<%= Model.lastSync %>"></span> <button type="button" class="small green ajax" id="button_trademe_sync">sync now!</button>
    </div>
</div>
<div class="mt50">
<button id="buttonlogout">disconnect from trademe</button>
</div>
</div>
<div id="notconnected" class="hidden">
    <div class="info">Don't have a TradeMe account? <a href="http://www.trademe.co.nz" target="_blank">Create a free account now</a>
    <p><strong>Membership is restricted to New Zealanders!</strong></p>
    </div>
    <p>By linking your Tradelr account with TradeMe, you will be able to do the following without leaving your dashboard</p>
    <ol class="ml20 mb20">
        <li>List your products on TradeMe</li>
        <li>Automatically synchronise active and completed orders</li>
        <li>Automatically update your inventory so that you do not oversell</li>
        <li>Relist products on TradeMe as long as the product is in stock</li>
        <li>Store contact information of your buyers</li>
        <li>Notify buyers when their order has been shipped</li>
        <li>Post feedback</li>
        <li>Cancel active listings</li>
    </ol>
        <div class="mt50">
<button id="buttonlogin" class="green">connect with trademe</button>
</div>
</div>
</div>
<script type="text/javascript">
    $('#button_trademe_sync').click(function () {
        $.post('/dashboard/trademe/sync', function (json_result) {
            if (json_result.success) {
                $('span', '#trademe_lastsync').attr('title', json_result.data);
                $('span', '#trademe_lastsync').prettyDate();
            } else {
                $.jGrowl(json_result.message);
            }
        });
    });
    
    $('#buttonlogin').click(function () {
        $(this).attr('disabled', true);
        window.location = '/dashboard/trademe/getToken';
    });

    $('#buttonlogout').click(function () {
        $.post('/dashboard/trademe/clearToken', null, function (json_result) {
            if (json_result.success && json_result.data) {
                $('#connected').fadeOut(function () {
                    $('#notconnected').fadeIn();
                });
            }
            else {
                $.jGrowl('Unable to disconnect from trademe');
            }
        }, 'json');
    });

    
    $(document).ready(function () {
        $.post('/dashboard/networks/haveToken', { type: 'trademe' }, function (json_result) {
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
