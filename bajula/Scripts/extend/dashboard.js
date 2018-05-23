var tradelr = tradelr || { };
tradelr.dashboard = { };
tradelr.dashboard.ebay = { };
tradelr.dashboard.ebay.shipping_init = function (siteEl) {
    $(siteEl).change(function () {
        var id = $(this).val();
        if (id == '') {
            return;
        }

        $('#ebay_profile_selector').show();
        $('#ebayshippingprofile, #ebay_shipping_profile').html('');

        // update shipping profile
        $.post('/dashboard/shipping/ebaysiteprofile/' + id, function (json_result) {
            if (json_result.success) {
                for (var i = 0; i < json_result.data.length; i++) {
                    var entry = json_result.data[i];
                    var option = $("<option>").attr("value", entry.value).text(entry.name);
                    $('#ebayshippingprofile').append(option);
                }
                $('#ebayshippingprofile').appendable('/dashboard/shipping/ebayprofileadd', 'Create New Profile');
            } else {
                $.jGrowl(json_result.message);
            }
        });

        
    });

    $('#ebayshippingprofile_edit').click(function () {
        var selected = $('#ebayshippingprofile').val();
        if (selected != '') {
            loadEbayProfileSection(selected);
        }
    });

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

    $('#ebayshippingprofile').change(function () {
        var selected = $(this).val();
        if (selected != '' && selected > 0) {
            loadEbayProfileSection(selected);
        }
        else {
            $('#ebay_shipping_profile').html('');
        }
    });

    // profile stuff
    $('#ebay_add_domestic').live('click', function () {
        $.post('/dashboard/shipping/ebayservices',
            { isInternational: false, siteid: $(siteEl).val() },
            function (json_result) {
                if (json_result.success) {
                    $('tbody', '#ebay_domestic_rules').prepend(json_result.data);
                } else {
                    $.jGrowl(json_result.message);
                }

            });
        return false;
    });

    $('#ebay_add_international').live('click', function () {
        $.post('/dashboard/shipping/ebayservices',
            {
                isInternational: true,
                siteid: $(siteEl).val()
            },
            function (json_result) {
                if (json_result.success) {
                    $('tbody', '#ebay_international_rules').prepend(json_result.data);
                }
                else {
                    $.jGrowl(json_result.message);
                }
            });
        return false;
    });

    $('#ebay_shipping_profile').delegate('.buttonEbayAddLocation', 'click', function () {
        var list = $(this).next('#shipToLocation_list');
        var select = $(this).prev('#shipToLocation');
        var selected = $(select).val();
        var option = $("<li>")
            .attr('id', selected)
            .text($("option:selected", select).text());
        $(list).append(option);
    });

    $('#ebay_shipping_profile').delegate('.action_cancel', 'click', function () {
        $(this).closest('tr').fadeOut(function () {
            $(this).remove();
        });
    });

    $('#ebay_shipping_profile').delegate('.action_ok', 'click', function () {
        var row = $(this).closest('tr');
        var service = $('#service_name', row).val();
        var cost = $('#cost', row).val();
        if (cost == '') {
            $.jGrowl('Please enter shipping cost');
            return;
        }

        var shipToDestination = [];
        $('#shipToLocation_list li', row).each(function () {
            shipToDestination.push($(this).attr('id'));
        });

        $.post('/dashboard/shipping/ebayservicessave',
            {
                profileid: $('#ebayshippingprofile').val(),
                serviceid: service,
                cost: cost,
                destinations: shipToDestination.join(",")
            },
            function (json_result) {
                if (json_result.success) {
                    $(row).replaceWith(json_result.data);
                } else {
                    $.jGrowl(json_result.message);
                }
            });
    });

    $('#ebay_shipping_profile').delegate('.hover_del', 'click', function () {
        var ok = window.confirm("Are you sure?");
        if (!ok) {
            return;
        }
        var row = $(this).closest('tr');
        var id = $(row).attr('alt');

        $.post('/dashboard/shipping/ebayrulesdelete/' + id, function (json_result) {
            if (json_result.success) {
                $(row).closest('tr').fadeOut(function () {
                    $(this).remove();
                });
            } else {
                $.jGrowl(json_result.message);
            }
        });
    });
};
