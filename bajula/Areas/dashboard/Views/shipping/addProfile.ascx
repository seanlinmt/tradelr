<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>
<%@ Import Namespace="tradelr.Libraries.Extensions" %>
<form id="profileAddForm" action="/dashboard/shipping/createprofile" method="post">
<h3 id="headingAdd">
    shipping profile</h3>
<div class="form_entry">
    <div class="form_label">
        <label for="profileName">
            Profile Name</label>
    </div>
    <input id="profileName" name="profileName" type="text" />
</div>
<div class="pt10">
    <button id="buttonSave" type="button" class="green">
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
                $('#buttonSave').buttonEnable();
                return false;
            }

            // post form
            $.ajax({
                type: "POST",
                url: action,
                dataType: "json",
                data: serialized,
                success: function (json_data) {
                    if (json_data.success) {
                        var data = json_data.data;
                        $('#shippingprofile').insertOption(data.title, data.id);
                        loadProfileSection(data.id);
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


