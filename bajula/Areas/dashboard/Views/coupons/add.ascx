<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<tradelr.Models.coupons.Coupon>" %>
<%@ Import Namespace="tradelr.Models.coupons" %>
<form id="couponAddForm" method="post">
<h3 id="headingAdd">
    create coupon</h3>
<div class="form_entry">
    <div class="form_label">
        <label for="name" class="required">
            Description</label>
            <div class="w450px">
            <span class="tip_inline">Example: Winter sale 2010 discount coupon</span>
            <div class="charsleft fr">
            <span id="descr-charsleft"></span>
        </div>
            </div>
    </div>
    <input type="text" id="description" name="description" value="<%= Model.description %>" class="w450px" />
</div>
<div class="fl mr10">
    <div class="form_entry w200px">
        <div class="form_label">
            <label for="name" class="required">
                Coupon code</label>
                <div>
                <span class="tip_inline">Example: WINTERSALE, C3gb3K
            </span>
            <div class="charsleft inline-block pl10">
            <span id="code-charsleft"></span>
        </div>
                </div>
                
        </div>
        <%= Html.TextBox("code", Model.code)%>
    </div>
</div>
<div class="fl">
    <div class="form_entry">
        <div class="form_label">
            <label for="name" class="required">
                Coupon value</label><span class="tip">discount value or percentage</span>
        </div>
        <div>
        <div class="w170px fl">
        <input type="text" name="value" id="value" value="<%= Model.value %>" class="w150px" />
        </div>
        <div class="fl">
        <select id="valuetype" name="valuetype" class="w100px">
        <option value="<%= Model.currency.code %>">
                <%= Model.currency.code %></option>
            <option value="%">%</option>
        </select>
        </div>
        <div class="clear"></div>
        </div>
    </div>
</div>
<div class="clear">
</div>
<div class="form_entry">
    <div class="form_label">
        <label>
            Maximum number of uses</label>
    </div>
    <ul>
        <li>
            <input type="radio" id="unlimited_type" name="duration_type" value="<%= DurationType.UNLIMITED %>" 
            <%= string.IsNullOrEmpty(Model.maxImpressions)?"checked='checked'":"" %> /> <label for="unlimited_type">unlimited</label>
            </li>
        <li>
            <input type="radio" id="impression_type" name="duration_type" value="<%= DurationType.IMPRESSION %>"
                <%= !string.IsNullOrEmpty(Model.maxImpressions)?"checked='checked'":"" %> />
            <label for="impression_type">up to</label>
            <input type="text" name="maxImpressions" id="maxImpressions" value="<%= Model.maxImpressions %>" class="w100px" />
            <label for="impression_type">impressions</label></li>
    </ul>
</div>
<div class="form_entry mt20">
    <ul class="normal">
        <li>
            <%= Html.CheckBox("hasDuration", Model.hasDuration)%> 
            <label for="hasDuration">coupon valid from</label>
            <input type="text" name="start_date" id="start_date" value="<%= Model.start_date %>" class="w100px" />
            <label class="m0">to</label>
            <input type="text" name="end_date" id="end_date" value="<%= Model.end_date %>" class="w100px" />
        </li>
        <li>
            <%= Html.CheckBox("minimumPurchaseOnly", Model.minimumPurchaseOnly)%>
            <label for="minimumPurchaseOnly">
                minimum purchase of</label>
                <input type="text" name="minimumPurchase" id="minimumPurchase" value="<%= Model.minimumPurchase %>" class="w100px ar" />
                 <%= Model.currency.code %></li>
    </ul>
</div>
<div class="pt5">
    <button class="green" type="button" id="buttonSave">
        save</button>
    <button type="button" id="buttonCancel">
        cancel</button>
</div>
<%= Html.Hidden("id", Model.id) %>
</form>
<script type="text/javascript">
    $(document).ready(function () {
        if ($('#id', '#couponAddForm').val() != '') {
            $('#couponAddForm').attr('action', '/dashboard/coupons/update');
            $('#headingAdd', '#couponAddForm').html('edit coupon');
        }
        else {
            $('#couponAddForm').attr('action', '/dashboard/coupons/create');
        }

        // prettify
        inputSelectors_init();
        $('#start_date', '#couponAddForm').watermark('start date');
        $('#end_date', '#couponAddForm').watermark('end date');

        $('#value, #minimumPurchase', '#couponAddForm').numeric({ allow: '.' });
        $('#maxImpressions', '#couponAddForm').numeric();

        $('#description').limit('100', '#descr-charsleft');
        $('#code').limit('50', '#code-charsleft');

        $('#start_date', '#couponAddForm').datepicker({
            dateFormat: 'D, d M yy',
            onSelect: function (dateText, inst) {
                var start_date = Date.parse(dateText);
                var end_date = Date.parse($('#end_date', '#couponAddForm').val());
                if (end_date == null) {
                    return true;
                }
                if (!end_date.isAfter(start_date)) {
                    $.jGrowl('Invalid start date selected');
                    $(this).val('');
                    return false;
                }
            }
        });
        $('#end_date', '#couponAddForm').datepicker({
            dateFormat: 'D, d M yy',
            onSelect: function (dateText, inst) {
                var end_date = Date.parse(dateText);
                var start_date = Date.parse($('#start_date', '#couponAddForm').val());
                if (start_date == null) {
                    return true;
                }
                if (!end_date.isAfter(start_date)) {
                    $.jGrowl('Invalid end date selected');
                    $(this).val('');
                    return false;
                }
            }
        });
    });

    $('#buttonSave', '#couponAddForm').click(function () {
        $(this).buttonDisable();
        $('#couponAddForm').trigger('submit');
    });

    $('#buttonCancel', '#couponAddForm').click(function () {
        dialogBox_close();
        return false;
    });

    $('#couponAddForm').submit(function () {
        var ok = $('#couponAddForm').validate({
            invalidHandler: function (form, validator) {
                $(validator.invalidElements()[0]).focus();
            },
            focusInvalid: false,
            rules: {
                description: {
                    required: true
                },
                code: {
                    required: true
                },
                value: {
                    required: true
                }
            }
        }).form();
        if (!ok) {
            $('#buttonSave', '#couponAddForm').buttonEnable();
            return false;
        }
        
        var serialized = $(this).serialize();
        
        // post form
        $.ajax({
            type: "POST",
            url: $(this).attr('action'),
            data: serialized,
            dataType: "json",
            success: function (json_data) {
                if (json_data.success) {
                    $.jGrowl('Coupon saved');

                    // when in marketing view
                    reloadCouponsGrid();

                    // when in store settings view
                    $('#facebookCoupon').val($('#code', '#couponAddForm').val());

                    dialogBox_close();
                }
                else {
                    $.jGrowl(json_data.message);
                }
                $('#buttonSave', '#couponAddForm').buttonEnable();
                return false;
            }
        });
        return false;
    });
</script>
