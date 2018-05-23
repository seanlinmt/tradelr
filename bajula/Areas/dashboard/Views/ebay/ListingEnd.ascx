<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<tradelr.Areas.dashboard.Models.product.ebay.EbayEndListingViewModel>" %>
<form id="ebayEndListingForm" action="/dashboard/ebay/listingend/<%= Model.itemid %>" method="POST">
    <h3>End Ebay Listing</h3>
        <div class="form_entry">
            <div class="form_label">
                <label for="categoryTitle">
                    reason for ending listing</label>
            </div>
            <%= Html.DropDownList("reason")%>
        </div>
        <div class="pad5">
            <button type="submit" class="green">
                end listing</button>&nbsp;
            <button onclick="dialogBox_close();" type="button">
                cancel</button>
        </div>
</form>
<script type="text/javascript">
    $('#ebayEndListingForm').submit(function () {
        var action = $(this).attr("action");
        var serialized = $(this).serialize();

        // post form
        $.post(action, serialized, function (json_result) {
            if (json_result.success) {
                $('#ebay_listing_status').text(json_result.data);
                $('#ebay_button_end_listing').hide();
                dialogBox_close();
            } else {
                $.jGrowl(json_result.message);
            }
        });

        return false;
    });
</script>