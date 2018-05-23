<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<tradelr.Areas.dashboard.Models.product.trademe.TrademeProductViewModel>" %>
<%@ Import Namespace="tradelr.Library.Constants" %>
<div id="trademe_product_settings">
    <% if (Model.isPosted)
       { %>
    <div>
        <% if(Model.isActive){%>
               <button type="button" class="ajax orange small" id="trademe_button_end_listing">end listing</button>
          <% } else { %>
               <button type="button" class="ajax green small" id="trademe_button_relist">relist item</button>
           <%}%>
        <%= Html.Hidden("trademe_relist","False") %>
    </div>
    <div class="info_box">
        <table class="simple">
            <tr>
                <th>
                    Listing ID
                </th>
                <td>
                    <a href="<%= Model.ViewLocation %>" target="_blank">
                        <%= Model.ListingID %></a>
                </td>
                <th>
                    Start Date
                </th>
                <td>
                    <%= Model.StartDate %>
                </td>
            </tr>
            <tr>
                <th>
                    Listing Status
                </th>
                <td>
                    <%= Model.isActive?"Active":"Completed" %>
                </td>
                <th>
                    End Date
                </th>
                <td>
                    <%= Model.EndDate %>
                </td>
            </tr>
            <tr>
                <th>
                    Fees Charged
                </th>
                <td>
                    <%= Model.ListingFees %>
                </td>
                <td>
                </td>
                <td>
                </td>
            </tr>
        </table>
    </div>
    <% } %>
    <div class="pb10 bb">
        <ul>
            <li><%= Html.CheckBox("trademe_autorelist", Model.autorelist) %>
        <label for="trademe_autorelist">
            auto relist if product is in stock</label></li>
            <li><%= Html.CheckBox("trademe_isnew", Model.isBrandNew) %>
        <label for="trademe_isnew">
            item is brand new</label></li>
            <li><%= Html.CheckBox("trademe_authenticatedonly", Model.onlyAuthenticated) %>
        <label for="trademe_authenticatedonly">
            only allow bids from authenticated members</label></li>
            <li><%= Html.CheckBox("trademe_safetrader", Model.safetrader) %>
        <label for="trademe_safetrader">
            support SafeTrader</label></li>
        </ul>
        
    </div>
    <div id="trademe_auction_options" class="fl">
        <div class="fl">
            <div class="form_entry">
            <div class="form_label">
                <label for="trademe_startprice">
                    Start Price</label>
            </div>
            <%= Html.TextBox("trademe_startprice", Model.StartPrice)%>
        </div>
        </div>
        <div class="fl ml10">
            <div class="form_entry">
            <div class="form_label">
                <label for="trademe_reserveprice">
                    Reserve Price </label>
            </div>
            <%= Html.TextBox("trademe_reserveprice", Model.ReservePrice)%>
        </div>
        </div>
        <div class="fl ml10">
            <div class="form_entry">
            <div class="form_label">
                <label for="trademe_buynowprice">
                    Buy Now Price</label>
            </div>
            <%= Html.TextBox("trademe_buynowprice", Model.BuynowPrice)%>
        </div>
        </div>
    </div>
    <div class="clear"></div>
    <div class="form_entry">
            <div class="form_label">
                <label>Additional Promotions</label>
            </div>
            <ul class="list_fl">
            <li><%= Html.CheckBox("trademe_bold", Model.isBold) %>
        <label for="trademe_bold">
            bold title (95c)</label></li>
            <li class="ml20"><%= Html.CheckBox("trademe_gallery", Model.hasGallery) %>
        <label for="trademe_gallery">
            gallery feature (55c)</label></li>
            <li class="ml20"><%= Html.CheckBox("trademe_featured", Model.isFeatured) %>
        <label for="trademe_featured">
            category feature ($3.45)</label></li>
            <li class="ml20"><%= Html.CheckBox("trademe_homepage", Model.isHomepageFeatured) %>
        <label for="trademe_homepage">
            homepage feature ($39)</label></li>
        </ul>
        <div class="clear"></div>
        </div>
    <div id="trademe_category_selector" class="fl">
        <div class="form_entry">
            <div class="form_label">
                <label for="trademe_category">
                    TradeMe Category</label>
            </div>
            <ol id="trademe_category_list" class="normal">
                <% var catcount = Model.categories.Count;
                   for (int i = catcount; i-- > 0; )
                   {%>
                <li>
                    <%= Html.DropDownList("trademe_cat_" + (Model.categories.Count - 1 - i), Model.categories[i])%></li>
                <% } %>
            </ol>
        </div>
        <p>
            <span id="trademe_category_selected" class="ok_post <%= Model.isPosted?"":"hidden" %>">
                category selected</span></p>
    </div>
    <div class="fr" style="width: 560px">
        <div class="fl">
            <div class="form_entry">
                <div class="form_label">
                    <label for="trademe_duration">
                        Duration</label>
                </div>
                <%= Html.DropDownList("trademe_duration", Model.durations, new Dictionary<string, object>() { { "disabled", "disabled" } })%>
            </div>
        </div>
        <div class="fl ml10">
            <div class="form_entry">
                <div class="form_label">
                    <label for="trademe_quantity">
                        Quantity</label>
                </div>
                <%= Html.TextBox("trademe_quantity", Model.quantity)%>
            </div>
        </div>
        <div class="clear">
        </div>
        <div class="form_entry">
                <div class="form_label">
                    <label for="trademe_duration">
                        Shipping</label>
                </div>
                <%= Html.DropDownList("trademe_pickup", Model.GetPickups())%>
                <ul id="trademe_shippingoptions">
                    <li>
                <input id="trademe_shippingfree" type="radio" name="trademe_shippingcosts" value="<%= GeneralConstants.FREE %>" <%= Model.freeShipping?"checked='checked'":"" %>  />
                <label for="trademe_shippingfree">free shipping within New Zealand</label></li>
                <li>
                <input id="trademe_shippingcosts" type="radio" name="trademe_shippingcosts" value="" />
                <label for="trademe_shippingcosts">specify shipping costs</label>
            <table class="header_bold">
                <thead>
                    <tr><td>Cost</td><td>Details <span class="tip_inline"> e.g. Overnight courier within Auckland</span></td><td></td></tr>
                </thead>
                <tbody>
                    <% foreach (var shippingCost in Model.shippingCosts)
                       {%>
                       <tr><td>$ <input class='w75px' type='text' name='trademe_scost' value='<%= shippingCost.cost %>' /></td><td><input class='w400px' type='text' name='trademe_sdesc' value='<%= shippingCost.description %>' /></td><td><span class='hover_del'></span></td></tr>
                          <% } %>
                </tbody>
                <tfoot><tr><td colspan="3"><a class="icon_add" href="#" id="trademe_add_shipping_option">add shipping option</a></td></tr></tfoot>
            </table>
            </li>
            </ul>
            </div>
    </div>
</div>
<script type="text/javascript">
    $(document).ready(function () {
        $('#trademe_pickup').trigger('change');
        $('#trademe_shippingoptions').trigger('click');
        $("input[name='trademe_sdesc']").limit(50);
    });     // end ready

    function handleTrademeShippingOptions(el) {
        if ($('#trademe_shippingfree').is(":checked")) {
            $("input[type='text']", el).attr('disabled', true);
            $('#trademe_add_shipping_option').hide();
        }
        else {
            $("input[type='text']", el).attr('disabled', false);
            $('#trademe_add_shipping_option').show();
        }
    }

    $('#trademe_shippingoptions').click(function () {
        handleTrademeShippingOptions(this);
    });

    $('#trademe_pickup').change(function () {
        var selected = $(this).val();
        if (selected == 2) {
            // must pickup
            $('input', '#trademe_shippingoptions').attr('disabled', true);
            $('#trademe_add_shipping_option').hide();
        }
        else {
            $('input', '#trademe_shippingoptions').attr('disabled', false);
            $('#trademe_add_shipping_option').show();
        }
    });

    $('#trademe_add_shipping_option').click(function () {
        $('table > tbody', '#trademe_shippingoptions').append("<tr><td>$ <input class='w75px' type='text' name='trademe_scost' /></td><td><input class='w400px' type='text' name='trademe_sdesc' /></td><td><span class='hover_del'></span></td></tr>");
        inputSelectors_init('#trademe_shippingoptions');
        $("input[name='trademe_sdesc']").limit(50);
        return false;
    });

    $('#trademe_duration', '#trademe_product_settings').each(function () {
        if ($(this).children().length != 0) {
            $(this).attr("disabled", false);
        }
    });

    $('#trademe_button_relist').click(function () {
        $('#trademe_relist').val('True');
        $(this).trigger('submit');
    });

    $('#trademe_button_end_listing').click(function () {
        dialogBox_open("/dashboard/trademe/listingend/<%= Model.ListingID %>", 600);
    });

    $('select', '#trademe_category_list').live('change', function () {
        var selected = $(this).val();
        if (selected == '') {
            return;
        }

        var list = $(this).parent();
        var index = $('select', '#trademe_category_list').index(this);
        var isLeaf = false;

        $.get('/dashboard/trademe/categoryselector', { parentid: selected }, function (json_result) {
            var appendOptions = function (selector, datalists) {
                if (datalists.categories != null) {
                    for (var i = 0; i < datalists.categories.length; i++) {
                        var entry = datalists.categories[i];
                        $('<option></option>').attr("value", entry.id).text(entry.name).appendTo(selector);
                    }
                }
                else {
                    isLeaf = true;
                }

                // duration
                if (datalists.durations != null) {
                    var duration_selector = $('#trademe_duration', '#trademe_product_settings');
                    if (datalists.durations.length > 0) {
                        $(duration_selector).html('').attr('disabled', false);
                    }
                    $.each(datalists.durations, function () {
                        $('<option></option>').attr("value", this.id).text(this.name).appendTo(duration_selector);
                    });
                }
            }; // appendOptions

            if (json_result.success) {
                // empty select
                $(list).nextAll('li').each(function () {
                    $(this).remove();
                });

                var select = $("<select name='trademe_cat_" + (index + 1) + "'></select>");

                $("<option value=''>select ...</option>").appendTo(select);

                appendOptions(select, json_result.data);

                // only append if there are entries as we have reached a leaf
                if ($(select).children().length > 1) {
                    var newlist = $("<li></li>").append(select);
                    $(newlist).insertAfter(list);
                }

                if (isLeaf) {
                    $('#trademe_category_selected').show();
                } else {
                    $('#trademe_category_selected').hide();
                    $('#trademe_duration', '#trademe_product_settings').html('').attr('disabled', true);
                }
            } else {
                $.jGrowl(json_result.message);
            }
        });
    });
</script>
