<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<tradelr.Areas.dashboard.Models.store.policies.PolicySettings>" %>
<form id="policiesForm" autocomplete="off" action="<%= Url.Action("Policies","Store") %>" method="post">
<div class="section_header">
    Payment Policy</div>
<div class="form_group">
    <div class="form_entry">
        <div class="form_label">
            <div class="clear">
            </div>
            <span class="tip fl">Payment terms for sent out invoices</span>
            <div class="charsleft fr pr10">
                <span id="paymentTerms-charsleft"></span>
            </div>
        </div>
        <%= Html.TextArea("paymentTerms", Model.paymentTerms)%>
    </div>
</div>
<div class="section_header">
    Refund Policy
</div>
<div class="form_group">
    <div class="form_entry">
        <div class="form_label">
            <div class="clear">
            </div>
            <span class="tip fl">Example: Sold goods are not returnable</span>
            <div class="charsleft fr pr10">
                <span id="returnPolicy-charsleft"></span>
            </div>
        </div>
        <%= Html.TextArea("returnPolicy", Model.refundPolicy)%>
    </div>
</div>
<div class="buttonRow_bottom">
<div class="mr10">
    <button id="buttonSavePolicies" type="button" class="green ajax">
        <img src="/Content/img/save.png" alt='' />
        update policies</button>
</div>
</div>
</form>
<script type="text/javascript">
    $(document).ready(function () {
        $('#buttonSavePolicies', '#policiesForm').click(function () {
            $(this).trigger('submit');
        });
        $('#policiesForm').trackUnsavedChanges('#buttonSavePolicies');
    });

    $('#policiesForm').submit(function () {
        var action = $(this).attr("action");
        var serialized = $(this).serialize();
        $(this).buttonDisable();

        // post form
        $.ajax({
            type: "POST",
            url: action,
            dataType: "json",
            data: serialized,
            success: function (json_data) {
                if (json_data.success) {
                    $.jGrowl('Policies successfully saved');
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