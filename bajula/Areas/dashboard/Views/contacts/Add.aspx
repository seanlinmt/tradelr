<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<tradelr.Areas.dashboard.Models.contact.ContactViewModel>" %>

<%@ Import Namespace="tradelr.Libraries.Extensions" %>
<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Contacts
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="content_area_2col mt10">
        <div id="content_main">
            <div id="added" class="boxSuccess hidden">
                <h3>
                    Your contact has been saved</h3>
                <p id="companyNameMessage">
                </p>
                <p>
                    You can now do the following</p>
                <ul>
                    <li><button type="button" onclick="window.location = '/dashboard/invoices/add';" class="small">create new invoice</button></li>
                    <li><button type="button" onclick="window.location = '/dashboard/orders/add';" class="small">create new order</button></li>
                </ul>
                <div class="clear">
                </div>
            </div>
            <div id="addDiv" class="hidden">
                <h3 id="headingAdd">
                    new contact
                </h3>
            </div>
            <div id="editDiv" class="hidden">
                <h3 id="headingProfile" class="fl">
                    edit contact
                </h3>
                <div class="mb10 mt10 fr">
                        <button class="small green" type="button" id="buttonViewContact">
                            view</button>
                        <button class="small red" type="button" id="buttonDeleteContact">
                            delete</button>
                    </div>
                    <div class="clear"></div>
            </div>
            <form id="contactAddForm" autocomplete="off" action="/dashboard/contacts/create" method="post" class="hidden">

            <div id="contact_tabs" class="tabs_clear">
            <ul class="hidden">
                <li><a href="#basic_tab">profile information</a></li>
                <% if (Model.editMode) {%>
                       <li><a href="#address_tab">billing and shipping</a></li>
                   <%} %>
            </ul>
            <div id="basic_tab" class="hidden">
            <%
                Html.RenderPartial("userInfo", Model.contact); %>
                <div class="section_header">
                Organization Information</div>
            <div class="form_group">
                <div id="existingCompanyDiv" class="hidden fl mt10 w250px">
                    <div class="form_entry">
                        <div class="form_label">
                            <label for="companyName">
                                Existing Company</label>
                        </div>
                        <%= Html.DropDownList("existingOrg", Model.organisationList)%>
                    </div>
                </div>
                    <%
                        Html.RenderPartial("organisationInfo", Model); %>
            </div>
            <div class="section_header">
                Notes</div>
            <div class="form_group">
                <div class="form_entry">
                    <%= Html.TextArea("notes", Model.contact.notes)%>
                    <div class="charsleft">
                        <span id="notes-charsleft"></span>
                    </div>
                </div>
                <div class="form_entry">
                <%= Html.CheckBox("sendemail") %><label for="sendemail">email contact with above note</label>
                </div>
            </div>
            <%
                Html.RenderPartial("userPermission", Model.contact.permissions); %>
            </div>
            <% if (Model.editMode) {%>
            <div id="address_tab" class="hidden">
            <div class="form_group">
            <%
                        Html.RenderPartial("contact_addresses", Model.addresses); %>
            </div>
            </div>
            <%} %>
            </div>
            <div class="buttonRow_bottom">
                <span class="mr10">
                    <button id="buttonAdd" type="button" class="large green ajax">
                        <img src="/Content/img/save.png" alt='' />
                        save</button>
                </span>
            </div>
            <%= Html.Hidden("id", Model.contact.id)%>
            <%= Html.Hidden("orgid", Model.contact.orgid)%>
            <%= Html.Hidden("organisationPhotoID")%>
            </form>
        </div>
        <div id="content_side">
            <div class="top">
            </div>
            <div class="middle">
                <div class="content_side_box">
                <ul>
                <li><a class="icon_add" href="/dashboard/contacts/import">import contacts</a></li>
                <li><a class="icon_add" href="#" onclick="javascript:dialogBox_open('/dashboard/contacts/networklink');return false;">add network contact</a></li>
                </ul>
                </div>
                <div id="contact_network" class="content_side_box hidden">
                
                </div>
                <div id="contact_tasks" class="content_side_box hidden">
                    <div class="header">
                        Contact Tasks</div>
                    <div id="contactActions" class="content">
                        <ul>
                            <li><span class="linkButton" id="buttonInvoice">create invoice</span></li>
                            <li><span class="linkButton" id="buttonOrder">create order</span></li>
                        </ul>
                    </div>
                    
                </div>
            </div>
            <div class="bottom">
            </div>
        </div>
        <div class="clear">
        </div>
    </div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="AdditionalJS" runat="server">
<script type="text/javascript">
    function showProperHeaders() {
        if ($('#id').val() !== '') {
            //////////////////////////// edit contact
            //$('.boxHelp').hide();
            $('#editDiv').show();
            $('#contact_tasks').show();
            $('#addDiv').hide();
            $('#tipAdd').hide();
            $('#contactAddForm').show();
            $('#contactAddForm').attr('action', '/dashboard/contacts/update');
            $('#existingCompanyDiv').hide();
            $(window).trackUnsavedChanges('#buttonAdd');
        }
        else {
            ////////////// add contact
            $('#contact_tasks').hide();
            $('#addDiv').show();
            $('#existingCompanyDiv').show();
            $('#tipAdd').show();
            $('#editDiv').hide();

            // no find first
            $('#contactAddForm').show();
            $(window).trackUnsavedChanges('#buttonAdd');
        }
    }

    $(document).ready(function () {
        var companyNameFilled = false;
        // show appropriate information for form edit
        showProperHeaders();

        $('#navcontact').addClass('navselected_white');

        $('#contact_tabs').tabs({
            select: function (event, ui) {
                
            }
        });

        // check to see if email already exist
        var timer;
        $('#email').bind('keyup', function () {
            var textbox = this;
            var emailAddr = $(this).val();
            if (!validateEmail(emailAddr)) {
                return false;
            }

            if (timer !== undefined) {
                clearTimeout(timer);
            }

            timer = setTimeout(function () {
                $.ajax({
                    type: "POST",
                    url: '/dashboard/contacts/find/',
                    data: { email: emailAddr, id: $('#id', '#contactAddForm').val() },
                    dataType: "json",
                    success: function (json_data) {
                        if (json_data.success) {
                            var response = json_data.data;
                            switch (response) {
                                case tradelr.returncode.INUSE:
                                    $(textbox).next('#emailCheckResponse').html("<span class='error_post'>email in use</span>");
                                    break;
                                case tradelr.returncode.ISLINKED:
                                case tradelr.returncode.NOTFOUND:
                                    $(textbox).next('#emailCheckResponse').html('');
                                    break;
                                default:
                                    $(textbox).next('#emailCheckResponse').html('');
                                    var resp = ["<p class='font_darkorange'><img src='/Content/img/headings/heading_network_16.png' /> Network Contact Found</p>",
                                            "<div class='profile' alt='" + response.id + "'>",
                                                "<div class='fl'>",
                                                    response.profileThumbnail,
                                                "</div>",
                                                "<div class='fl ml10 mt6'>",
                                                "<div class='name'>" + response.fullName + "</div>",
                                                "<div class='org'>" + response.companyName + ", " + response.countryName + "</div>",
                                                "</div><div class='clear'></div>",
                                            "</div>",
                                            "<button id='sendRequest' class='small'>request network link</button>"
                                            ];
                                    $('#contact_network').html(resp.join('')).slideDown();
                                    $('#sendRequest', '#contact_network').click(function () {
                                        $.post('/dashboard/contacts/requestsend', { id: $('.profile', '#contact_network').attr('alt') }, function (json_result) {
                                            if (json_result.success) {
                                                $('#contact_network').slideUp();
                                                $.jGrowl('Request sent successfully');
                                            }
                                            else {
                                                $.jGrowl(json_result.message);
                                            }
                                        }, 'json');
                                    });
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

        // add contact button
        $('#buttonAdd').click(function () {
            $(this).buttonDisable();
            $('#contactAddForm').trigger('submit');
        });

        $('#buttonInvoice').click(function () {
            window.location = '/dashboard/invoices/add?contact=' + $('#orgid').val();
        });

        $('#buttonOrder').click(function () {
            window.location = '/dashboard/orders/add?contact=' + $('#orgid').val();
        });

        // delete user
        $('#buttonDeleteContact').click(function () {
            var ok = window.confirm('Are you sure? All activities, messages and messages belonging to this contact will also be deleted.');
            if (!ok) {
                return false;
            }
            var id = $('#id').val();
            if (id == '') {
                $.jGrowl('Contact has not been added. Contact not deleted.');
                return false;
            }

            $.ajax({
                type: "POST",
                url: '/dashboard/contacts/delete/' + id,
                dataType: "json",
                success: function (json_data) {
                    if (json_data.success) {
                        $.jGrowl('Contact successfully deleted');
                        window.location = '/dashboard/contacts';
                    }
                    else {
                        if (json_data.data == tradelr.returncode.NOPERMISSION) {
                            $.jGrowl('You do not have permission to delete contacts');
                        }
                        else {
                            $.jGrowl(json_data.message);
                        }
                    }
                    return false;
                }
            });
            return false;
        });

        $('#buttonViewContact').click(function () {
            window.location = '/dashboard/contacts/' + $('#id').val();
        });

        bindOrgUploader();

        $('#existingOrg').bind('change', function () {
            var selectedVal = $(this).val();
            if (selectedVal == '') {
                return false;
            }
            $.ajax({
                type: "POST",
                url: '/org/list/' + selectedVal,
                dataType: "json",
                success: function (json_data) {
                    if (json_data.success) {
                        var org = json_data.data;
                        $('#companyName').val(org.companyName);
                        $('#address').val(org.address);
                        $('#city').val(org.city);
                        $('#coPhone').val(org.coPhone);
                        $('#postcode').val(org.postcode);
                        $('#fax').val(org.fax);
                        $('#countryval').html(org.country);
                        $('#stateval').html(org.state);
                        $('#orgid').val(org.id);
                        user_location_init();
                    }
                    else {
                        $.jGrowl(json_data.message);
                    }
                    return false;
                }
            });
            return false;
        });

        $('#notes').limit('1000', '#notes-charsleft');

        $('#contactAddForm').submit(function () {
            // update photos ids before we serialise
            var profilePhotoID = $('.thumbnail > img', '#profile_image').attr('alt');
            var orgPhotoID = $('.thumbnail > img', '#company_image').attr('alt');
            $('#profilePhotoID').val(profilePhotoID);
            $('#organisationPhotoID').val(orgPhotoID);

            var action = $(this).attr("action");

            // if company name is not specified, use name, if no name then use email
            if ($('#companyName').val() === '') {
                if ($('#firstName').val() !== '') {
                    if ($('#lastName').val() !== '') {
                        $('#companyName').val($('#lastName').val() + ', ' + $('#firstName').val());
                        companyNameFilled = true;
                    }
                    else {
                        $('#companyName').val($('#firstName').val());
                        companyNameFilled = true;
                    }
                }
                else { // use email since it's not empty
                    $('#companyName').val($('#email').val());
                    companyNameFilled = true;
                }
            }

            // handle permissions
            if ($('#permissions')) {
                var permission = $('#permissions').val();
                $('#permissions').val(parseUserPermissions(permission));
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
                        if ($('#id').val() !== '') {
                            $.jGrowl('Contact successfully updated');
                        }
                        else {
                            $('#id').val(data.id);
                            if (companyNameFilled) {
                                $('#companyNameMessage').html('The name of the company has been updated as <strong>' + $('#companyName').val() + '</strong>.');
                            }
                            $.jGrowl('Contact successfully saved');
                            $('#added').fadeIn();
                            // update headers
                            $('#id').val(data.uid);
                            $('#orgid').val(data.oid);
                            showProperHeaders();
                        }
                        scrollToTop();
                    }
                    else {
                        $('#companyName').val('');
                        $.jGrowl(json_data.message);
                    }
                    $('#buttonAdd').buttonEnable();
                    return false;
                }
            });
            return false;
        });

        // input highlighters
        inputSelectors_init();
        init_autogrow();
    });
</script>
</asp:Content>
