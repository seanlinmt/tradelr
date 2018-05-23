<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<tradelr.Areas.dashboard.Models.account.AccountViewModel>" %>
<%@ Import Namespace="tradelr.Common.Constants" %>
<%@ Import Namespace="tradelr.Models.users" %>
<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Account
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="content_area_2col">
        <div id="content_main">
            <ul class="hidden">
                <li><a href="#account_profile">profile</a></li>
                <li><a href="#address_tab">billing and shipping</a></li>
                <% if (Model.contact.permissions.HasPermission(UserPermission.NETWORK_SETTINGS))
                   {%>
                <li><a href="#account_payment">payment methods</a></li>
                <%
                   }%>
                <% if (Model.role.HasFlag(UserRole.CREATOR))
                   {%>
                <li><a href="#account_domain">custom domain</a></li>
                <li><a href="#account_affiliate">affiliates</a></li>
                <%
                   }%>
            </ul>
            <div id="account_profile" class="hidden">
                <form id="profileEditForm" action="<%= Url.Action("UpdateProfile","Account") %>" method="post" autocomplete="off">
                <% Html.RenderPartial("profile", Model);%>
                <div class="buttonRow_bottom">
                    <span class="mr10">
                        <button id="buttonUpdateProfile" type="button" class="large ajax green">
                            <img src="/Content/img/save.png" alt='' />
                            update profile</button>
                    </span>
                </div>
                </form>
            </div>
            <div id="address_tab" class="hidden">
                <form id="addressEditForm" action="<%= Url.Action("UpdateAddresses","Account") %>"
                method="post" autocomplete="off">
                <div class="form_group">
                    <div class="info">
                        Keep your billing and shipping addresses up to date.</div>
                    <% Html.RenderPartial("~/Areas/dashboard/Views/contacts/contact_addresses.ascx", Model.addresses); %>
                </div>
                <div class="buttonRow_bottom">
                    <span class="mr10">
                        <button id="buttonUpdateAddress" type="button" class="large ajax green">
                            <img src="/Content/img/save.png" alt='' />
                            update billing &amp; shipping addresses</button>
                    </span>
                </div>
                </form>
            </div>
            <%
                if (Model.contact.permissions.HasPermission(UserPermission.NETWORK_SETTINGS))
                {%>
            <div id="account_payment" class="hidden">
                <% Html.RenderPartial("payment", Model);%>
            </div>
            <%
                }%>
            <% if (Model.role.HasFlag(UserRole.CREATOR))
               {%>
            <div id="account_domain" class="hidden">
                <div class="form_group">
                    <div class="fl">
                        <div class="form_entry">
                            <div class="form_label">
                                <label for="domainName">
                                    Custom Domain Name</label>
                                <span class="tip">For example: mydomain.com <a target="_blank" href="http://www.tradelr.com/help/dashboard/account">need help?</a></span>
                            </div>
                            <%= Html.TextBox("domainName", Model.customDomain)%>
                            <button id="buttonUpdateDomain" type="button" class="ajax small">
                                update domain</button>
                        </div>
                    </div>
                    <div class="fr">
                        <div class="info w200px">
                            <div>
                                Don't have your own domain yet?</div>
                            <a href="#" onclick="$('#orderDomainForm').show();return false;">Get your own domain
                                here</a>
                        </div>
                        <% if (!string.IsNullOrEmpty(Model.customDomain))
                           { %>
                        <div class="info w200px">
                            <div>
                                Need protection from eavesdroppers for your custom domain name?</div>
                            <a href="#" onclick="$('#orderSSLForm').show();return false;">Get GeoTrust SSL for your
                                domain name</a>
                        </div>
                        <% } %>
                    </div>
                    <div class="clear">
                    </div>
                </div>
                <% Html.RenderPartial("DomainTools", Model.TLDExtensionList); %>
                <% Html.RenderPartial("SSLTools"); %>
            </div>
            <div id="account_affiliate">
                <p>Your affiliate ID is <strong><%= Model.affiliateID %></strong></p>
                <p><%= Model.affiliateTo %></p>
            </div>
            <%
                   }%>
        </div>
        <div id="content_side">
            <div class="top">
            </div>
            <div class="middle">
                <% if (Model.isInTrialPeriod)
                   { %>
                <div class="content_side_box bg_blue">
                    <div class="header font_black">
                        Free Trial</div>
                    <div class="content">
                        <ul class="custom">
                            <li>expires <strong><%= Model.trialExpiryDate %></strong></li>
                        </ul>
                    </div>
                </div>
                <% }%>
                  <% if (!Model.notSubscribed && !Model.isInTrialPeriod)
                     {%>
                <div class="content_side_box bg_beige">
                    <div class="header">
                        Plan Expired</div>
                    <div class="content">
                        <ul class="custom">
                            <li>You are currently not subscribed to any plan. Some features will be disabled. 
                            You should <a href="/dashboard/account/plan">subscribe to a plan now</a>.</li>
                        </ul>
                    </div>
                </div>
                <% } %>
                <div class="content_side_box">
                    <div class="header">
                        Account</div>
                    <div class="content">
                        <ul class="custom">
                            <li class="custom-pass"><a id="changePassword" href="#">Change password</a></li>
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
        $(document).ready(function () {

            $('#navaccount').addClass('navselected');
            var $tabs = $('#content_main').tabs({
                select: function (event, ui) {
                }
            });

            ////////// bind image uploaders
            bindUserUploader();
            bindOrgUploader();

            //// side bar actions
            $('#changePassword').click(function () {
                dialogBox_open("/dashboard/account/password");
                return false;
            });

            $('#offlineAccess').click(function () {
                if (DEBUG) {
                    if (!$.browser.webkit) {
                        $.jGrowl('This feature is only available with Google Chrome or Apple Safari');
                        return false;
                    }
                    $.post('/settings/offlineaccess', null, function (json_result) {
                        if (json_result.success) {
                            var text = $('#offlineAccess').text();
                            if (text.indexOf('Disable') == -1) {
                                $('#offlineAccess').text('Disable Offline Access');
                                $.jGrowl('Offline Access Enabled');
                            }
                            else {
                                $('#offlineAccess').text('Enable Offline Access');
                                $.jGrowl('Offline Access Disabled');
                            }
                        }
                        else {
                            $.jGrowl(json_result.message);
                        }
                        return false;
                    }, 'json');
                }
                else {
                    $.jGrowl('This feature is coming soon');
                    $.post('/monitor', { feature: "offline access" });
                }
                return false;
            });

            ///////////// domain
            $('#buttonUpdateDomain').click(function () {
                $(this).buttonDisable();
                $.post('/dashboard/account/updatedomain', { domainName: $('#domainName').val() }, function (json_result) {
                    if (json_result.success) {
                        window.location = json_result.data;
                    }
                    else {
                        $.jGrowl(json_result.message);
                    }
                });
            });

            ///////// addresses
            $('#buttonUpdateAddress').click(function () {
                $(this).buttonDisable();
                $('#addressEditForm').trigger('submit');
            });
            $('#addressEditForm').trackUnsavedChanges('#buttonUpdateAddress');

            $('#addressEditForm').submit(function () {
                var action = $(this).attr("action");
                var serialized = $(this).serialize();
                $.post(action, serialized, function (json_data) {
                    if (json_data.success) {
                        $.jGrowl('Settings successfully updated');
                        scrollToTop();
                    }
                    else {
                        $.jGrowl(json_data.message, { sticky: true });
                    }
                });
                return false;
            });

            /////////////// profile
            $('#buttonUpdateProfile').click(function () {
                $(this).buttonDisable();
                $('#profileEditForm').trigger('submit');
            });
            $('#profileEditForm').trackUnsavedChanges('#buttonUpdateProfile');

            $('#profileEditForm').submit(function () {
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

                var serialized = $(this).serialize();
                // post form
                $.ajax({
                    type: "POST",
                    url: action,
                    dataType: "json",
                    data: serialized,
                    success: function (json_data) {
                        if (json_data.success) {
                            $.jGrowl('Settings successfully updated');
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

            // input highlighters
            inputSelectors_init();
            init_autogrow('.content_area_2col');
        });
    </script>
</asp:Content>
