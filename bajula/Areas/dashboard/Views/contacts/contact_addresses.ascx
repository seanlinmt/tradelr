<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<tradelr.Models.address.ContactAddressesViewModel>" %>
<div id="address_billing" class="mr40 mb20 fl">
<div class="section_header">Billing address</div>
        <div class="form_entry">
            <div class="form_label">
                <label for="first_name">
                    First Name</label>
            </div>
            <%= Html.TextBox("billing_first_name", Model.billing.firstName)%>
        </div>
        <div class="form_entry">
            <div class="form_label">
                <label for="last_name">
                    Last Name</label>
            </div>
            <%= Html.TextBox("billing_last_name", Model.billing.lastName)%>
        </div>
        <div class="form_entry">
            <div class="form_label">
                <label for="company">
                    Company</label>
            </div>
            <%= Html.TextBox("billing_company", Model.billing.companyName)%>
        </div>
        <div class="form_entry">
            <div class="form_label">
                <label for="address">
                    Your Street Address</label></div>
            <%= Html.TextArea("billing_address", Model.billing.streetAddress, new Dictionary<string, object>() { { "class", "w100p" } })%>
        </div>
        <div class="form_entry">
            <div class="form_label">
                <label for="city">
                    City</label>
            </div>
            <%= Html.TextBox("billing_city", Model.billing.city)%>
            <%= Html.Hidden("billing_citySelected")%>
        </div>
        <div class="form_entry">
            <div class="form_label">
                <label for="postcode">
                    Postal/Zip Code</label>
            </div>
            <%= Html.TextBox("billing_postcode", Model.billing.postcode)%>
        </div>
        <% Html.RenderPartial("~/Views/contacts/userLocation.ascx", "#address_billing"); %>
        <div class="form_entry">
            <div class="form_label">
                <label for="phone">
                    Contact Phone</label>
            </div>
            <%= Html.TextBox("billing_phone", Model.billing.phone)%>
        </div>
        <span id="countryval" class="hidden"><%= Model.billing.countryid%></span> 
        <span id="stateval" class="hidden"><%= Model.billing.state%></span>
        <% if (!Model.hideSameShippingCheckBox) {%>
        <div class="same_shipping_checkbox">
               <%= Html.CheckBox("ship_same_billing", Model.sameBillingAndShipping)%>
        <label for="ship_same_billing">
            ship items to the above address</label>
            </div>
           <%} %>
        
    </div>
    <div id="address_shipping" class="fl <%= Model.sameBillingAndShipping && !Model.hideSameShippingCheckBox?"hidden":"" %>" >
    <div class="section_header">Shipping address</div>
        <div class="form_entry">
            <div class="form_label">
                <label for="first_name">
                    First Name</label>
            </div>
            <%= Html.TextBox("shipping_first_name", Model.shipping.firstName)%>
        </div>
        <div class="form_entry">
            <div class="form_label">
                <label for="last_name">
                    Last Name</label>
            </div>
            <%= Html.TextBox("shipping_last_name", Model.shipping.lastName)%>
        </div>
        <div class="form_entry">
            <div class="form_label">
                <label for="company">
                    Company</label>
            </div>
            <%= Html.TextBox("shipping_company", Model.shipping.companyName)%>
        </div>
        <div class="form_entry">
            <div class="form_label">
                <label for="address">
                    Your Street Address</label></div>
            <%= Html.TextArea("shipping_address", Model.shipping.streetAddress, new Dictionary<string, object>() { { "class", "w100p" } })%>
        </div>
        <div class="form_entry">
            <div class="form_label">
                <label for="city">
                    City</label>
            </div>
            <%= Html.TextBox("shipping_city", Model.shipping.city)%>
            <%= Html.Hidden("shipping_citySelected")%>
        </div>
        <div class="form_entry">
            <div class="form_label">
                <label for="postcode">
                    Postal/Zip Code</label>
            </div>
            <%= Html.TextBox("shipping_postcode", Model.shipping.postcode)%>
        </div>
        <% Html.RenderPartial("~/Views/contacts/userLocation.ascx", "#address_shipping"); %>
        <div class="form_entry">
            <div class="form_label">
                <label for="phone">
                    Contact Phone</label>
            </div>
            <%= Html.TextBox("shipping_phone", Model.shipping.phone)%>
        </div>
        <span id="countryval" class="hidden"><%= Model.shipping.countryid%></span> 
        <span id="stateval" class="hidden"><%= Model.shipping.state%></span>
    </div>
    <div class="clear">
    </div>
<script type="text/javascript">
    $(document).ready(function () {
        $('#ship_same_billing').change(function () {
            $('#address_shipping').toggle();
        });

        $('#billing_city, #shipping_city').autocomplete('/city/find', {
            dataType: "json",
            parse: function (data) {
                var rows = new Array();
                if (data != null && data.length != null) {
                    for (var i = 0; i < data.length; i++) {
                        rows[i] = { data: data[i], value: data[i].title, result: data[i].title };
                    }
                }
                return rows;
            },
            autoFill: true,
            formatItem: function (row, i, max) {
                return row.title;
            }
        });

        $('#billing_city, #shipping_city').bind('keyup', function () {
            if ($(this).attr('id') == '#billing_city') {
                $("#billing_citySelected").val('');
            }
            else {
                $("#shipping_citySelected").val('');
            }
        });

        $('#billing_city, #shipping_city').result(function (event, data, formatted) {
            if (data) {
                if ($(this).attr('id') == '#billing_city') {
                    $("#billing_citySelected").val(data.id);
                }
                else {
                    $("#shipping_citySelected").val(data.id);
                }
            }
        });
    });
</script>