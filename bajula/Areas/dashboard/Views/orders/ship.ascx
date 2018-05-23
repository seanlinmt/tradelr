<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<tradelr.Models.transactions.viewmodel.OrderShipViewModel>" %>
<h3 id="headingShip">
    Ship Order</h3>
<form id="shippedForm" action="/dashboard/orders/ship/<%= Model.orderID %>" method="post">
<% if (Model.useShipwire)
   {%>
   <div class="info">When using Shipwire, the status of this order will only change to SHIPPED when Shipwire actually ships the order.</div>
<div class="form_entry">
    <input type="checkbox" name="shipwire" id="shipwire" />
    <label for="shipwire">Submit order to <span class="font_shipwire">Shipwire</span> for processing. </label>
</div>
<%  } %>
<div id="manualtracking" class="relative bt mt10">
<div class="form_entry">
    <div class="form_label">
        <label for="shippingService">
            Shipping Service Used</label>
            <span class="tip">Example: USPS, DHL, FedEx</span>
    </div>
    <%= Html.TextBox("shippingService") %>
</div>
<div class="form_entry">
    <div class="form_label">
        <label for="trackingno">
            Shipment Tracking Number</label>
    </div>
    <%= Html.TextBox("trackingno") %>
</div>
<div class="form_entry">
    <div class="form_label">
        <label for="trackingAddress">
            Tracking Information</label>
            <span class="tip">Site where buyer can track the package. Example: http://www.ups.com/tracking/tracking.html </span>
    </div>
    <%= Html.TextBox("trackingAddress") %>
</div>
</div>


<div class="pt10">
    <button id="buttonSave" type="button" class="ajax green">
        ship order</button>&nbsp;
    <button id="buttonCancel" type="button">
        cancel</button>
</div>
</form>
<script type="text/javascript">
    $('#shipwire').live('click', function () {
        if ($('#shipwire').is(':checked')) {
            $('#manualtracking').slideUp();
        }
        else {
            $('#manualtracking').slideDown();
        }
    });
    $(document).ready(function () {
        if ($('#shipwire').is(':checked')) {
            $('#manualtracking').slideUp();
        }
        else {
            $('#manualtracking').slideDown();
        }

        $('#buttonSave', '#shippedForm').click(function () {
            $(this).trigger('submit');
        });

        $('#buttonCancel', '#shippedForm').click(function () {
            dialogBox_close();
        });

        $('#shippedForm').submit(function () {
            var action = $(this).attr("action");
            var serialized = $(this).serialize();

            // post form
            $.ajax({
                type: "POST",
                url: action,
                data: serialized,
                success: function (json_result) {
                    if (json_result.success) {
                        // flash message
                        scrollToTop();
                        window.location.reload();
                    }
                    else {
                        $.jGrowl(json_result.message);
                    }

                }
            }, 'json');
            return false;
        });
        inputSelectors_init();
    });
</script>
