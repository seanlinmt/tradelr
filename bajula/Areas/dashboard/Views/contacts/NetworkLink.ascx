<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>
<h3 class="headingNetwork"> Network Link Request</h3>
<form id="networklinkForm" action="/comtacts/networklink" method="post">
<span class="tip">Enter the sitename or primary email address of the tradelr account you want to link with.</span>
<div class="form_entry">
    <div class="form_label">
        <label for="email">
            Network ID
            </label>
    </div>
    <%= Html.TextBox("id") %>
</div>
<div id="result" class="mt10"></div>
<div class="mt10">
<button id="buttonSave" type="button" class="green ajax hidden">
        send request</button>
    <button id="buttonCancel" type="button">
        close</button>
</div>
</form>
<script type="text/javascript">
    $(document).ready(function () {
        $('#buttonSave', '#networklinkForm').click(function () {
            $(this).buttonDisable();
            $.post('/dashboard/contacts/requestsend', { id: $('#result .profile', '#networklinkForm').attr('alt') }, function (json_result) {
                if (json_result.success) {
                    dialogBox_close();
                    $.jGrowl('Request sent successfully');
                }
                else {
                    $(this).buttonEnable();
                    $.jGrowl(json_result.message);
                }
            }, 'json');
        });
        $('#buttonCancel', '#networklinkForm').click(function () {
            dialogBox_close();
        });

        var timer_networklink;
        $('#id', '#networklinkForm').bind('keyup', function () {
            if (timer_networklink !== undefined) {
                clearTimeout(timer_networklink);
            }

            var id = $(this).val();
            if (id == '') {
                return false;
            }

            timer_networklink = setTimeout(function () {
                $.ajax({
                    type: "POST",
                    url: '/dashboard/contacts/networkfind/',
                    data: { id: id },
                    dataType: "json",
                    success: function (json_data) {
                        if (json_data.success) {
                            var response = json_data.data;
                            switch (response) {
                                case tradelr.returncode.ISLINKED:
                                    $('#result', '#networklinkForm').html('<span class="error_post">You are already connected to <strong>' + id + '</strong></span>');
                                    $('#buttonSave', '#networklinkForm').hide();
                                    break;
                                case tradelr.returncode.NOTFOUND:
                                    $('#result', '#networklinkForm').html('<span class="error_post">Could not find match for given Network ID</span>');
                                    $('#buttonSave', '#networklinkForm').hide();
                                    break;
                                default:
                                    var resp = ["<div class='profile' alt='" + response.id + "'>",
                                                "<div class='fl'>",
                                                    response.profileThumbnail,
                                                "</div>",
                                                "<div class='fl ml10 mt6'>",
                                                "<div class='name'>" + response.fullName + "</div>",
                                                "<div class='org'>" + response.companyName + ", " + response.countryName + "</div>",
                                                "</div><div class='clear'></div>",
                                            "</div>"
                                            ];
                                    $('#result', '#networklinkForm').html(resp.join(''));
                                    $('#buttonSave', '#networklinkForm').show();

                                    break;
                            }
                        }
                        else {
                            $.jGrowl(json_data.message);
                        }
                        $('#buttonFindContacts').buttonEnable();
                        return false;
                    }
                });
            }, 500);
        });

        $('#networklinkForm').submit(function () {
            var action = $(this).attr("action");
            var serialized = $(this).serialize();
            var ok = $('#networklinkForm').validate({
                invalidHandler: function (form, validator) {
                    $(validator.invalidElements()[0]).focus();
                },
                focusInvalid: false,
                rules: {
                    id: {
                        required: true
                    }
                }
            }).form();
            if (!ok) {
                $('#buttonSave', '#networklinkForm').buttonEnable();
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
                    $('#buttonSave', '#networklinkForm').buttonEnable();
                    return false;
                }
            });
            return false;
        });
        inputSelectors_init();
    });
</script>
