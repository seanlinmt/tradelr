<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<tradelr.Models.networks.viewmodels.EbayNetworkViewModel>" %>
<div class="section_header">
    <img src="/Content/img/social/icons/ebay_16.png" /> eBay
</div>
<div class="form_group">
<div id="connected" class="hidden">
<p><%= Model.EbayProfileUrl %></p>
<div class="form_entry">
    <div class="form_label">
        <label for="email">
            Last Sync</label>
            <span class="tip">Synchronisation is automatically carried out every hour</span>
    </div>
    <div id="ebay_lastsync">
        <span title="<%= Model.lastSync %>"></span> <button type="button" class="small green ajax" id="button_ebay_sync">sync now!</button>
    </div>
</div>
<div class="mt50">
<button id="buttonlogout" type="button">disconnect from ebay</button>
</div>
</div>
<div id="notconnected" class="hidden">
    <div class="info">Don't have an eBay account? <a href="http://www.ebay.com" target="_blank">Create a free account now</a></div>
    <p>By linking your Tradelr account with eBay, you will be able to do the following without leaving your dashboard</p>
    <ol class="ml20 mb20">
        <li>List your products on eBay</li>
        <li>Automatically synchronise active and completed orders</li>
        <li>Automatically update your inventory so that you do not oversell</li>
        <li>Relist products on ebay as long as the product is in stock</li>
        <li>Store contact information of your buyers</li>
        <li>Notify buyers when their order has been shipped</li>
        <li>Post feedback</li>
        <li>Cancel active listings</li>
    </ol>
    <div class="mt50">
<button id="buttonlogin" class="green" type="button">connect with eBay</button>
</div>
</div>
</div>
<script type="text/javascript">
    $('#button_ebay_sync').click(function () {
        $.post('/dashboard/ebay/sync', function (json_result) {
            if (json_result.success) {
                $('span', '#ebay_lastsync').attr('title', json_result.data);
                $('span', '#ebay_lastsync').prettyDate();
            } else {
                $.jGrowl(json_result.message);
            }
        });
    });
    
    $('#buttonlogin').click(function () {
        $(this).attr('disabled', true);
        window.location = '/dashboard/ebay/getToken';
    });

    $('#buttonlogout').click(function () {
        $.post('/dashboard/ebay/clearToken', null, function (json_result) {
            if (json_result.success && json_result.data) {
                $('#connected').fadeOut(function () {
                    $('#notconnected').fadeIn();
                });
            }
            else {
                $.jGrowl('Unable to disconnect from eBay');
            }
        }, 'json');
    });

    $(document).ready(function () {
        $('span', '#ebay_lastsync').prettyDate();
        $.post('/dashboard/networks/haveToken', { type: 'ebay' }, function (json_result) {
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
