<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<tradelr.Models.address.CheckoutAddressViewModel>" %>
<div id="addresses" class="hidden">
<form id="addressesForm" autocomplete="off" method="POST" action="/checkout/order/<%= Model.cartid %>/update_addresses">
    <% Html.RenderPartial("~/Areas/dashboard/Views/contacts/contact_addresses.ascx", Model); %>
    <div class="pad10">
    <button class="green" type="button" id="buttonNext">
        continue to next step</button>
</div>
</form>
</div>
<script type="text/javascript">
    $(document).ready(function () {
        if (!$('#addresses').is(':visible')) {
            $('#addresses').slideDown();
        }

        init_autogrow("#addresses");

        var depends = function () {
            return !$("#ship_same_billing").is(':checked');
        };

        $('#buttonNext').click(function () {
            $(this).trigger('submit');
        });

        $('#addressesForm').submit(function () {
            var serialized = $(this).serialize();
            var action = $(this).attr("action");

            var ok = $(this).validate({
                invalidHandler: function (form, validator) {
                    $(validator.invalidElements()[0]).focus();
                },
                focusInvalid: false,
                rules: {
                    billing_first_name: "required",
                    billing_last_name: "required",
                    billing_company: "required",
                    billing_address: "required",
                    billing_city: "required",
                    billing_postcode: "required",
                    country: "required",
                    shipping_first_name: depends,
                    shipping_last_name: depends,
                    shipping_company: depends,
                    shipping_address: depends,
                    shipping_city: depends,
                    shipping_postcode: depends
                }
            }).form();

            if (!ok) {
                return false;
            }

            $(this).buttonDisable();

            $.ajax({
                type: "POST",
                url: action,
                data: serialized,
                success: function (result) {
                    if (typeof (result) == 'string') {
                        $('#addresses').slideUp(function () {
                            $('#addresses').after(result);
                        });
                    }
                }
            });
            return false;
        });
    });
</script>
