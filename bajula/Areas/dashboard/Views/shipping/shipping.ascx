<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<IEnumerable<SelectListItem>>" %>
<div id="shipping_settings">
    <div class="form_group fl">
    <div class="form_entry">
        <div class="form_label">
            <label for="shippingprofile">
                Tradelr Shipping Profile</label>
        </div>
        <%= Html.DropDownList("shippingprofile", Model)%>
        <button id="buttonEditShipping" class="small" onclick="shippingprofile_load();" type="button">edit profile</button>
    </div>
    </div>
    <div id="shipping_profile">
    
    </div>
</div>
<script type="text/javascript">
    function shippingprofile_load() {
        var selected = $('#shippingprofile').val();
        if (selected != '') {
            loadProfileSection(selected);
        }
    };

    function initProfileSections() {
        if ($('#flatrateshipping').is(':checked')) {
            $('#flatrate', '#shipping_settings').show();
        }
        if ($('#calculatedshipping').is(':checked')) {
            $('#calculated', '#shipping_settings').show();
        }
        if ($('#calculated_manual').is(':checked')) {
            $('#manual', '#shipping_settings').show();
        }
    };

    function loadProfileSection(id) {
        $.get('/dashboard/shipping/profile/' + id, function (result) {
            $('#shipping_profile').html(result);
            initProfileSections();
            dialogBox_close(); // closes from add profile
        });
    };

    $('#shippingprofile').bind('change', function () {
        var selected = $(this).val();
        if (selected != '' && selected > 0) {
            loadProfileSection(selected);
            $('.buttonRow_bottom', '#shippingForm').show();
            $('#buttonEditShipping', '#shipping_settings').attr('disabled', false);
        }
        else {
            $('#shipping_profile').html('');
            $('.buttonRow_bottom', '#shippingForm').hide();
            $('#buttonEditShipping', '#shipping_settings').attr('disabled', true);
        }
    });

    // handle shipping method section
    $('#shipping_destination_add').die().live('click', function () {
        var profileid = $('#shippingprofile').val();
        dialogBoxTitle_open('/dashboard/shipping/add/' + profileid, "<h3 class='font_white' id='headingAdd'>shipping cost rule</h3>", 440);
        return false;
    });

    $('#flatrateshipping', '#shipping_settings').die().live('click', function () {
        var profileid = $('#shippingprofile').val();
        $.post('/dashboard/shipping/profiletype/', { id: profileid, type: "flatrate" });
        $('#calculated', '#shipping_settings').hide();
        $('#flatrate', '#shipping_settings').show();

    });

    $('#calculatedshipping', '#shipping_settings').die().live('click', function () {
        $('#flatrate', '#shipping_settings').hide();
        $('#calculated', '#shipping_settings').show();
    });

    $('#calculated_manual', '#shipping_settings').die().live('click', function () {
        var profileid = $('#shippingprofile').val();
        $.post('/dashboard/shipping/profiletype/', { id: profileid, type: "calculated" });
        $('#shipwire', '#shipping_settings').hide();
        $('#manual', '#shipping_settings').show();
    });

    $('#calculated_shipwire', '#shipping_settings').die().live('click', function () {
        var profileid = $('#shippingprofile').val();
        $.post('/dashboard/shipping/profiletype/', { id: profileid, type: "shipwire" });
        $('#manual', '#shipping_settings').hide();
        $('#shipwire', '#shipping_settings').show();
    });

    $('.hover_edit', '#shipping_countries').die().live('click', function () {
        var row = $(this).parents('tr:first');
        var id = $(row).attr('id');
        dialogBoxTitle_open('/dashboard/shipping/edit/' + id, "<h3 class='font_white headingEdit'>shipping cost rule</h3>", 440);
    });

    $('.hover_del', '#shipping_countries').die().live('click', function () {
        var hideHeader = function () {
            if ($('#shipping_countries').children().length == 0) {
                $('#shipping_header').fadeOut('fast', function () {
                    $(this).remove();
                });
            }
        };

        var row = $(this).parents('tr:first');
        var id = $(row).attr('id');
        $.post('/dashboard/shipping/delete/' + id, { profileid: $('#shippingprofile').val() }, function (json_result) {
            if (json_result.success) {
                // is this the last rule?
                var siblingscount = $(row).siblings().length;
                var hasstateTable = $(row).parents('table:first').next('.shipping_states').children().length != 0;

                if (siblingscount == 0 && !hasstateTable) {
                    // first detect if we are a state rule
                    // check if we're the only state left
                    // check if there's a table above us. If there is, check if anything inside
                    if ($(row).parents('.shipping_states:first').length != 0 &&
                        $(row).parents('.shipping_states:first').children().length == 1 &&
                        $(row).parents('.shipping_states:first').prev('table').find('tr').length == 0) {
                        $(row).parents('.shipping_states:first').parent().fadeOut('fast', function () {
                            $(this).remove();
                            hideHeader();
                        });
                    }
                    else {
                        // delete the li
                        $(row).parents('li:first').fadeOut('fast', function () {
                            $(this).remove();
                            hideHeader();
                        });
                    }
                }
                else {
                    $(row).slideUp('fast', function () {
                        $(this).remove();
                        hideHeader();
                    });
                }
            }
            else {
                $.jGrowl(json_result.message);
            }
        }, 'json');
    });
</script>
