<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<System.Collections.Generic.IEnumerable<SelectListItem>>" %>
<form id="profileAddForm" action="/dashboard/shipping/ebayProfileCreate" method="post">
<h3 id="headingAdd">
    ebay shipping profile</h3>
<div class="form_entry">
    <div class="form_label">
        <label for="siteid">
            eBay Site</label>
    </div>
    <%= Html.DropDownList("siteid", Model) %>
</div>
<div class="form_entry">
    <div class="form_label">
        <label for="profileName">
            Profile Name</label>
    </div>
    <input id="profileName" name="profileName" type="text" />
</div>
<div class="pt10">
    <button id="buttonSave" type="button" class="green ajax">
        add</button>
    <button id="buttonCancel">
        cancel</button>
</div>
</form>
<script type="text/javascript">
    $(document).ready(function () {
        $('#buttonCancel', '#profileAddForm').bind("click", function () {
            dialogBox_close();
            return false;
        });

        $('#buttonSave', '#profileAddForm').bind("click", function () {
            $('#profileAddForm').trigger('submit');
            return false;
        });

        $('#profileAddForm').submit(function () {
            var action = $(this).attr("action");
            var serialized = $(this).serialize();

            var ok = $('#profileAddForm').validate({
                invalidHandler: function (form, validator) {
                    $(validator.invalidElements()[0]).focus();
                },
                focusInvalid: false,
                rules: {
                    profileName: {
                        required: true
                    }
                }
            }).form();
            if (!ok) {
                return false;
            }

            $('#buttonSave').buttonDisable();

            // post form
            $.ajax({
                type: "POST",
                url: action,
                dataType: "json",
                data: serialized,
                success: function (json_data) {
                    if (json_data.success) {
                        var data = json_data.data;
                        $('#ebayshippingprofile').insertOption(data.text, data.value);
                        loadEbayProfileSection(data.value);
                    }
                    else {
                        $.jGrowl(json_data.message);
                    }
                }
            });
            return false;
        });

        // input highlighters
        inputSelectors_init();

    });
</script>


