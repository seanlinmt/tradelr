<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<tradelr.Areas.dashboard.Models.product.ebay.EbayProductViewModel>" %>
<%@ Import Namespace="Ebay.Enums" %>
<div id="ebay_product_settings">
    <% if (Model.isPosted)
       { %>
    <div>
        <% if(Model.isActive){%>
               <button type="button" class="ajax orange small" id="ebay_button_end_listing">end listing</button>
          <% } else { %>
               <button type="button" class="ajax green small" id="ebay_button_relist">relist item</button>
           <%}%>
        <%= Html.Hidden("ebay_relist","False") %>
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
            <li><%= Html.CheckBox("ebay_autorelist", Model.autorelist) %>
        <label for="ebay_autorelist">
            auto relist if product is in stock</label></li>
            <li><%= Html.CheckBox("ebay_includeAddress", Model.includeAddress) %>
        <label for="ebay_includeAddress">
            include company contact information in eBay listing</label></li>
        </ul>
        
    </div>
    <div class="fl">
        <div class="form_entry">
            <div class="form_label">
                <label for="ebay_listingtype">
                    Listing Type</label>
            </div>
            <%= Html.DropDownList("ebay_listingtype", Model.TypeList)%>
        </div>
    </div>
    <div id="ebay_auction_options" class="fl hidden">
        <div class="fl ml10">
            <div class="form_entry">
            <div class="form_label">
                <label for="ebay_startprice">
                    Start Price</label>
            </div>
            <%= Html.TextBox("ebay_startprice", Model.StartPrice)%>
        </div>
        </div>
        <div class="fl ml10">
            <div class="form_entry">
            <div class="form_label">
                <label for="ebay_reserveprice">
                    Reserve Price</label>
            </div>
            <%= Html.TextBox("ebay_reserveprice", Model.ReservePrice)%>
        </div>
        </div>
        <div class="fl ml10">
            <div class="form_entry">
            <div class="form_label">
                <label for="ebay_buynowprice">
                    Buy Now Price</label>
            </div>
            <%= Html.TextBox("ebay_buynowprice", Model.BuynowPrice)%>
        </div>
        </div>
    </div>
    <div class="clear"></div>
    <div id="ebay_category_selector" class="fl">
        <div class="form_entry">
            <div class="form_label">
                <label for="ebay_site">
                    eBay Site</label>
            </div>
            <%= Html.DropDownList("ebay_site", Model.GetSites())%>
        </div>
        <div class="form_entry">
            <div class="form_label">
                <label for="ebay_category">
                    eBay Category</label>
            </div>
            <ol id="ebay_category_list" class="normal">
                <% var catcount = Model.categories.Count;
                   for (int i = catcount; i-- > 0; )
                   {%>
                <li>
                    <%= Html.DropDownList("ebay_cat_" + (Model.categories.Count - 1 - i), Model.categories[i])%></li>
                <% } %>
            </ol>
        </div>
        <p>
            <span id="ebay_category_selected" class="ok_post <%= Model.isPosted?"":"hidden" %>">
                category selected</span></p>
    </div>
    <div class="fr" style="width: 560px">
        <div class="fl">
            <div class="form_entry">
                <div class="form_label">
                    <label for="ebay_condition">
                        Condition</label>
                </div>
                <%= Html.DropDownList("ebay_condition", Model.conditions, new Dictionary<string, object>(){{"disabled","disabled"}}) %>
            </div>
            <div class="form_entry">
                <div class="form_label">
                    <label for="ebay_duration">
                        Duration</label>
                </div>
                <%= Html.DropDownList("ebay_duration", Model.durations, new Dictionary<string, object>() { { "disabled", "disabled" } })%>
            </div>
            <div class="form_entry">
                <div class="form_label">
                    <label for="ebay_quantity">
                        Quantity</label>
                </div>
                <%= Html.TextBox("ebay_quantity", Model.quantity)%>
            </div>
        </div>
        <div class="ml10 fl">
            <div class="form_entry">
                <div class="form_label">
                    <label for="ebay_return_policy">
                        Return Policy</label>
                </div>
                <%= Html.DropDownList("ebay_return_policy", Model.GetReturnPolicy()) %>
            </div>
            <div id="return_policy_options" class="hidden">
                <div class="form_entry">
                    <div class="form_label">
                        <label for="ebay_return_within">
                            Returns Must Be Within</label>
                    </div>
                    <%= Html.DropDownList("ebay_return_within", Model.GetReturnWithin()) %>
                </div>
                <div class="form_entry">
                    <div class="form_label">
                        <label for="ebay_refund_policy">
                            Refund Policy</label>
                    </div>
                    <%= Html.DropDownList("ebay_refund_policy", Model.GetRefundPolicy()) %>
                </div>
            </div>
        </div>
        <div class="ml10 fl">
            <div class="form_entry">
                <div class="form_label">
                    <label for="ebay_return_policy">
                        Handling Time</label>
                </div>
                <%= Html.DropDownList("ebay_handling_time", Model.dispatchTimes) %>
            </div>
        </div>
        <div class="clear">
        </div>
        <% Html.RenderPartial("~/Areas/dashboard/Views/shipping/ebayProfileContainer.ascx"); %>
    </div>
</div>
<script type="text/javascript">
    $(document).ready(function () {
        tradelr.dashboard.ebay.shipping_init('#ebay_site');
        $('#ebay_site,#ebay_listingtype').trigger('change');
        $('#ebay_condition,#ebay_duration', '#ebay_product_settings').each(function () {
            if ($(this).children().length != 0) {
                $(this).attr("disabled", false);
            }
        });

        $('#ebay_button_relist').click(function () {
            $('#ebay_relist').val('True');
            $(this).trigger('submit');
        });

        $('#ebay_button_end_listing').click(function () {
            dialogBox_open("/dashboard/ebay/listingend/<%= Model.ListingID %>", 600);
        });

        // bind extra category change handling
        $('#ebay_site').bind('change.category', function () {
            var id = $(this).val();
            var rootlist = $('li:first', '#ebay_category_list');
            $(rootlist).nextAll('li').each(function () {
                $(this).remove();
            });
            var select = $('select', rootlist);
            $(select).html('');

            // update root category
            $.get('/dashboard/ebay/categoryselector', { site: id, level: 1, type: $('#ebay_listingtype').val() }, function (json_result) {
                if (json_result.success) {
                    $("<option value=''>select ...</option>").appendTo(select);
                    $.each(json_result.data.categories, function () {
                        $('<option></option>').attr("value", this.id).text(this.name).appendTo(select);
                    });
                } else {
                    $.jGrowl(json_result.message);
                }
            });
        });

        // listing type change
        $('#ebay_listingtype').bind('change', function () {
            var value = $(this).val();
            if (value == "<%= ListingType.Chinese %>") {
                $('#ebay_auction_options').show();
            } else {
                $('#ebay_auction_options').hide();
            }
        });
    });   // end ready

    $('#ebay_return_policy', '#ebay_product_settings').change(function () {
        if ($('#ebay_return_policy', '#ebay_product_settings').val() == "ReturnsNotAccepted") {
            $('#return_policy_options', '#ebay_product_settings').hide();
        }
        else {
            $('#return_policy_options', '#ebay_product_settings').show();
        }
    });
    $('#ebay_return_policy', '#ebay_product_settings').trigger('change');

    $('select', '#ebay_category_list').live('change', function () {
        var selected = $(this).val();
        if (selected == '') {
            return;
        }

        var site = $('#ebay_site').val();
        var list = $(this).parent();
        var index = $('select', '#ebay_category_list').index(this);
        var isLeaf = false;

        $.get('/dashboard/ebay/categoryselector', { site: site, parentid: selected, type: $('#ebay_listingtype').val() }, function (json_result) {
            var appendOptions = function (selector, category_condition_object) {
                if (category_condition_object.categories != null) {
                    // don't use foreach else it get parallelized
                    for (var i = 0; i < category_condition_object.categories.length; i++) {
                        var entry = category_condition_object.categories[i];
                        $('<option></option>').attr("value", entry.id).text(entry.name).appendTo(selector);
                    }
                }
                else {
                    isLeaf = true;
                }

                // condition
                if (category_condition_object.conditions != null) {
                    var condition_selector = $('#ebay_condition', '#ebay_product_settings');
                    if (category_condition_object.conditions.length > 0) {
                        $(condition_selector).html('').attr('disabled', false);
                    }
                    $.each(category_condition_object.conditions, function () {
                        $('<option></option>').attr("value", this.id).text(this.name).appendTo(condition_selector);
                    });
                }

                // duration
                if (category_condition_object.durations != null) {
                    var duration_selector = $('#ebay_duration', '#ebay_product_settings');
                    if (category_condition_object.durations.length > 0) {
                        $(duration_selector).html('').attr('disabled', false);
                    }
                    $.each(category_condition_object.durations, function () {
                        $('<option></option>').attr("value", this.id).text(this.name).appendTo(duration_selector);
                    });
                }
            }; // appendOptions

            if (json_result.success) {
                // empty select
                $(list).nextAll('li').each(function () {
                    $(this).remove();
                });

                var select = $("<select name='ebay_cat_" + (index + 1) + "'></select>");

                $("<option value=''>select ...</option>").appendTo(select);

                appendOptions(select, json_result.data);

                // only append if there are entries as we have reached a leaf
                if ($(select).children().length > 1) {
                    var newlist = $("<li></li>").append(select);
                    $(newlist).insertAfter(list);
                }

                if (isLeaf) {
                    $('#ebay_category_selected').show();
                } else {
                    $('#ebay_category_selected').hide();
                    $('#ebay_condition,#ebay_duration', '#ebay_product_settings').html('').attr('disabled', true);
                }
            } else {
                $.jGrowl(json_result.message);
            }
        });
    });
</script>
