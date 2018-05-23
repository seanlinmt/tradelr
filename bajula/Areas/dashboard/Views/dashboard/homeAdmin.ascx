<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<tradelr.Models.home.HomeViewData>" %>
<%@ Import Namespace="tradelr.Libraries.Extensions" %>
<div id="content_main">
    <ul class="hidden">
        <li><a href="#getStarted">get started</a></li>
        <li><a href="#activity_list"><img src="/Content/img/streamsource/tradelr.png" /> tradelr</a></li>
        <li><a href="#facebook_list"><img src="/Content/img/streamsource/facebook.png" /> facebook</a></li>
    </ul>
    <div id="getStarted" class="hidden">
        <a class="first" href="/dashboard/product/add">Add a Product <span>Add a product to your inventory and track what you have in stock</span> </a>
        <a href="/dashboard/contacts/add">Add
                a Contact <span>Add a product supplier or customer to your list of contacts</span>
            </a>
        <a href="/dashboard/invoices/add">Create a Sales Invoice <span>Invoice your client by email
                or print it for your own record</span> </a>
        <a href="/dashboard/orders/add">Create a Purchase Order
                    <span>Email your supplier a purchase order or print it for your own record</span>
                </a>
                <a href="/dashboard/store/settings">Configure Your Online Store
                    <span>Configure payment providers, shipping costs rules and your Facebook store</span>
                </a>
    </div>
    <div id="activity_list" class="">
        <%
            Html.RenderPartial("statistics", Model.stats);
            Html.RenderPartial("activities", Model.activities.tradelr); %>
    </div>
    <div id="facebook_list" class="hidden">
   <% Html.RenderAction("facebook"); %>
    </div>
</div>
<div id="content_side">
    <div class="top">
    </div>
    <div class="middle">
        <div id="quicklinks" class="content_side_box">
            <div class="header">
                Shortcuts</div>
            <div class="content">
                <ul>
                    <li><a href="/dashboard/product/add">Add Product</a></li>
                    <li><a href="/dashboard/contacts/add">Add Contact</a></li>
                    <li><a href="/dashboard/invoices/add">Create Sales Invoice</a></li>
                    <li><a href="/dashboard/orders/add">Create Purchase Order</a></li>
                    </ul>
                    <ul class="mt10">
                    <li><a href="/dashboard/networks">Import Products</a></li>
                </ul>
            </div>
        </div>
    </div>
    <div class="bottom">
    </div>
</div>
<div class="clear">
</div>
<script type="text/javascript">
    // need to do it here because .live() doesn't seem to bind to context properly
    $('.profilelink, .userlink').live('click', function (event) {
        if ($(this).parents('#facebook_list').length != 0) {
            // facebook
            var id = $(this).attr('alt');
            popup_open('/fb/profile/' + $.trim(id), event.clientX, event.clientY);
        }
        else {
            // normal contact
            window.location = $(this).attr('alt');
        }
        return false;
    });

    function popup_open(url, posx, posy) {
        dialogBox_close();
        $.post(url, null, function (json_result) {
            if (json_result.success) {
                var data = $(json_result.data);
                var title = $(json_result.title);
                $('body').append("<div id='ajax_dialog'><div id='ajax_content'></div></div>");
                $('#ajax_content').html(data);
                $('#ajax_dialog').dialog({
                    autoOpen: false,
                    closeOnEscape: true,
                    draggable: true,
                    modal: false,
                    resizable: false,
                    overlay: { background: "white" },
                    position: [posx, posy],
                    title: title,
                    width: 360,
                    height: 'auto',
                    zIndex: 1000
                });
                $('#ajax_dialog').dialog("open");
            }
            else {
                $.jGrowl(json_result.message);
            }
        }, 'json');
    }

    $(document).ready(function () {

        $('#content_main').tabs({
            selected: parseInt($('#panelIndex').text()),
            select: function (event, ui) {
                var index = parseInt(ui.index, 10);
                $.post('/settings/panelActivity?tabindex=' + index);
            },
            show: function (event, ui) {
                var index = parseInt(ui.index, 10);
                switch (index) {
                    case 1: // tradelr
                        break;
                    case 2: // facebook
                        if (typeof (tradelr.facebook.pollForFacebookPosts) == "function") {
                            tradelr.facebook.pollForFacebookPosts(true);
                        }
                        break;
                }
            }
        });

        $('#navhome').addClass('navselected');

        function accountInfo(subdomain, order, invoice) {
            this.subdomain = subdomain;
            this.order = order;
            this.invoice = invoice;
        }
    });
</script>