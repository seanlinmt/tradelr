<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<tradelr.Models.transactions.viewmodel.EbayOrderShipViewModel>" %>
<h3 id="headingShip" class="bb">
    Ship eBay Order</h3>
<form id="shippedForm" action="/dashboard/orders/shipebay/<%= Model.orderID %>" method="post">
<div class="form_entry">
    <div class="form_label">
        <label for="shippingService">
            Shipping Service Used</label>
    </div>
    <%= Html.TextBox("shippingService", Model.shippingService) %>
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
        <label for="trackingno">
            Leave Feedback</label>
    </div>
    <%= Html.TextBox("feedbackComment", "", new Dictionary<string, object>(){{"class","w400px"}}) %>
</div>
<div class="pt10">
    <button id="buttonSave" type="button" class="ajax green">
        ship order</button>&nbsp;
    <button id="buttonCancel" type="button">
        cancel</button>
</div>
</form>
<script type="text/javascript">
    $(document).ready(function () {
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
