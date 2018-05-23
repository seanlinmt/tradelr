$(document).ready(function () {
    

    $('#buttonAdd').click(function () {
        $(this).buttonDisable();
        $('#staffAddForm').trigger('submit');
    });

    $('#buttonCancel').click(function () {
        dialogBox_close();
    });

    // handle submit
    $('#staffAddForm').submit(function () {
        // update photos ids before we serialise
        var profilePhotoID = $('.thumbnail > img', '#profile_image').attr('alt');
        $('#profilePhotoID').val(profilePhotoID);

        var action = $(this).attr("action");

        var ok = $('#staffAddForm').validate({
            invalidHandler: function (form, validator) {
                $(validator.invalidElements()[0]).focus();
            },
            focusInvalid: false,
            rules: {
                email: {
                    required: true
                }
            }
        }).form();
        if (!ok) {
            $('#buttonAdd').buttonEnable();
            return false;
        }
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
                    reloadStaffGrid();
                    dialogBox_close();
                }
                else {
                    $.jGrowl(json_data.message);
                }
                $('#buttonAdd').buttonEnable();
                return false;
            }
        });
        return false;
    });

    inputSelectors_init();
});