<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<tradelr.Areas.dashboard.Models.store.general.GeneralSettings>" %>
<form id="settingsForm" autocomplete="off" action="<%= Url.Action("Settings","Store") %>"
method="post">
<div class="form_group">
<div class="form_entry">
            <div class="form_label">
                <label for="domainName">
                    Store Visibility</label>
            </div>
            <select name="storeEnabled" class="w500px">
            <option value="True" <%= Model.store_enabled?"selected='selected'":"" %>>Online shop is PUBLIC and can be viewed by anyone</option>
            <option value="False" <%= !Model.store_enabled?"selected='selected'":"" %>>Online shop is PRIVATE and can only be viewed by users with login and password</option>
            </select>
        </div>
    
    <div class="form_entry">
        <div class="form_label">
            <label for="storeName">
                Store Name</label>
        </div>
        <%=Html.TextBox("storeName", Model.storeName, 
            new Dictionary<string, object>(){
                {"class","w500px"}
            })%>
    </div>
    <div class="form_entry">
        <div class="form_label">
            <label for="motd">
                Store Message</label>
                <div class="charsleft fr">
                                    <span id="motd-charsleft"></span>
                                </div>
        </div>
        <%=Html.TextArea("motd", Model.motd)%>
    </div>
</div>

<div class="section_header">
    Facebook Store</div>
<div class="form_group">
    <div class="info">
        Reveal the following coupon code when your Facebook Page is liked</div>
    <div class="form_entry">
        <div class="form_label">
            <label for="facebookCoupon">
                Coupon Code</label>
        </div>
        <%=Html.TextBox("facebookCoupon", Model.facebookCoupon)%>
    </div>
</div>
<div class="buttonRow_bottom">
<div class="mr10">
    <button id="buttonSaveSettings" type="button" class="green ajax">
        <img src="/Content/img/save.png" alt='' />
        update settings</button>
</div>
</div>
</form>
<script type="text/javascript">
    $(document).ready(function() {
        $('#buttonSaveSettings', '#settingsForm').click(function () {
            $(this).trigger('submit');
        });
        $('#settingsForm').trackUnsavedChanges('#buttonSaveSettings');

        // init facebook coupon selector
        $('#facebookCoupon').click(function () {
            dialogBox_open('/dashboard/coupons/select', 600);
            return false;
        });

        $('#motd').limit('1000', '#motd-charsleft');
        $('#storeName').limit('200');
    });

    $('#settingsForm').submit(function () {
        var action = $(this).attr("action");
        var serialized = $(this).serialize();

        $('#buttonSaveSettings', '#settingsForm').buttonDisable();
        
        // post form
        $.ajax({
            type: "POST",
            url: action,
            dataType: "json",
            data: serialized,
            success: function (json_data) {
                if (json_data.success) {
                    $.jGrowl('Settings successfully updated');
                    scrollToTop();
                }
                else {
                    $.jGrowl(json_data.message, { sticky: true });
                }
                return false;
            }
        });
        return false;
    });
</script>