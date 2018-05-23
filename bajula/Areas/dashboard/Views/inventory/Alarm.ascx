<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<tradelr.Models.inventory.InventoryLocationItem>" %>
<h3><img src="/Content/img/inventory/alarm_32.png" /> Low Inventory Alarm</h3>
<form id="alarmForm" action="/dashboard/inventory/alarm" method="post">
<span class="tip">If your inventory level reaches this level, an email notification will be sent to you.</span>
<div class="form_entry">
    <div class="form_label">
        <label for="email">
            Alarm Level</label>
    </div>
    <%= Html.TextBox("quantity", Model.alarmLevel) %>
</div>
<div class="mt10">
    <button id="buttonSave" type="button" class="ajax green">
        save</button>
    <button id="buttonCancel" type="button">
        cancel</button>
</div>
<%= Html.Hidden("id", Model.id) %>
</form>
<script type="text/javascript">
    $(document).ready(function () {
        $('#buttonSave', '#alarmForm').click(function () {
            $(this).buttonDisable();
            $(this).trigger('submit');
        });
        $('#buttonCancel', '#alarmForm').click(function () {
            dialogBox_close();
        });

        $('#alarmForm').submit(function () {
            var action = $(this).attr("action");
            var serialized = $(this).serialize();
            var ok = $('#alarmForm').validate({
                invalidHandler: function (form, validator) {
                    $(validator.invalidElements()[0]).focus();
                },
                focusInvalid: false,
                rules: {
                    quantity: {
                        required: true
                    }
                }
            }).form();
            if (!ok) {
                $('#buttonSave', '#alarmForm').buttonEnable();
                return false;
            }
            // post form
            $.ajax({
                type: "POST",
                url: action,
                data: serialized,
                dataType: 'json',
                success: function (json_data) {
                    if (json_data.success) {
                        $.jGrowl(json_data.message);
                        var item = $('.itemid:contains(' + $('#id', '#alarmForm').val() + ')', '#inventory').parents('.content_row');
                        $('.alarmlevel', item).html($('#quantity', '#alarmForm').val());
                        dialogBox_close();
                    }
                    else {
                        $.jGrowl(json_data.message);
                    }
                    $('#buttonSave', '#alarmForm').buttonEnable();
                    return false;
                }
            });
            return false;
        });
        inputSelectors_init();
    });
</script>
