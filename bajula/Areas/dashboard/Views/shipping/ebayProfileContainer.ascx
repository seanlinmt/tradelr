<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>
<div id="ebay_shipping_settings">
<div id="ebay_profile_selector" class="form_entry hidden">
    <div class="form_label">
        <label for="ebayshippingprofile">
            eBay Shipping Profile</label>
    </div>
    <select id="ebayshippingprofile" name="ebayshippingprofile"></select>
    <button id="ebayshippingprofile_edit" class="small" type="button">edit profile</button>
</div>
<div id="ebay_shipping_profile">
    
</div>
</div>
<script type="text/javascript">
    function loadEbayProfileSection(id) {
        $.post('/dashboard/shipping/ebayprofile/' + id, function (json_result) {
            if (json_result.success) {
                $('#ebay_shipping_profile', '#ebay_shipping_settings').html(json_result.data);
                dialogBox_close(); // closes from add profile
            }
            else {
                $.jGrowl(json_result.message);
            }
        });
    };
</script>