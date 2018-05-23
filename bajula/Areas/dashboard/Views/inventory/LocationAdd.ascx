<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>
<form id="locationAddForm" action="/dashboard/inventory/locationCreate" method="post">
<h3 id="headingAdd">
    inventory location</h3>
    <p>Enter a name for your inventory location</p>
<div class="form_entry">
    <div class="form_label">
        <label for="locationName">
            Location Name</label>
    </div>
    <input id="locationName" name="locationName" type="text" />
</div>
<div class="pt10">
    <button type="submit" class="green">
        add</button>&nbsp;
    <button id="cancelButton">
        cancel</button>
</div>
</form>
<script type="text/javascript">
    $(document).ready(function () {
        $('#cancelButton', '#locationAddForm').bind("click", function () {
            dialogBox_close();
            return false;
        });
        $('#locationName').alphanumeric({ allow: ' .' });

        $('#locationAddForm').submit(function () {
            var action = $(this).attr("action");
            var serialized = $(this).serialize();

            // post form
            $.ajax({
                type: "POST",
                url: action,
                dataType: "json",
                data: serialized,
                success: function (json_data) {
                    if (json_data.success) {
                        var data = json_data.data;
                        // add entry to location list (product view)
                        if ($('#inventoryLocationAdd').length != 0) {
                            $('#inventoryLocationAdd').before('<div class="inventoryLocation"></div>');
                            $('.inventoryLocation:last').load('/dashboard/inventory/newloc/' + data.id + '?name=' + escape(data.title));
                        }

                        // add entry to side filter list (inventory view)
                        var entry = "<div class='sideboxEntry' style='display:none' fid='" + data.id + "'>" + data.title + "<div class='del'></div></div>";
                        $('#locationList > div:last').after(entry);
                        $('#locationList > div:last').slideDown('slow');
                        dialogBox_close();
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


