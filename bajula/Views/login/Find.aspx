<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/NotLoggedIn.Master"
    Inherits="System.Web.Mvc.ViewPage" %>

<%@ Import Namespace="tradelr.Libraries.Extensions" %>
<asp:Content ID="Content2" ContentPlaceHolderID="head" runat="server">
 Find Your tradelr Account
</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div id="banner">
        <div class="content">
            <h1>
                Find Your Account</h1>
        </div>
    </div>
    <div class="banner_main">
    <div class="content h500px">
    <div class="fl w400px">
        <form id="findForm" class="form_content form_larger" action="/login/find" method="post">
        <h3 id="heading_search">Find Account Login Page</h3>
        <div class="result_before">
        <p>
            Enter your email address below to find the login page for your account.</p>
        <div class="form_entry">
            <div class="form_label">
                <label>
                    Email Address</label></div>
            <input type="text" name="email" id="email" />
        </div>
        
            <button id="buttonSearch" type="button" class="green ajax">
                search</button>
        </div>
        <div class="result_after hidden">
        <p>The following accounts were found matching the email address submitted. 
        Click on the link(s) to access the login page. </p>
        </div>
        </form>
    </div>
    <div class="panel_sideInfo" style="margin-top:20px;">
            <h3>
                Don't have an account yet?</h3>
            <p>
                You will need to sign up for an account first.</p>
            <button id="buttonRegister" >
                sign up now</button>
                </div>
     <div class="clear"></div></div>
     </div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="AdditionalJS" runat="server">
    <script type="text/javascript">
        $(document).ready(function () {
            $('#buttonRegister').click(function () {
                window.location = '/pricing';
            });

            $('#buttonSearch').click(function () {
                $(this).buttonDisable();
                $('#findForm').trigger('submit');
            });

            $('#email').focus().addClass('curFocus');

            $('#findForm').submit(function () {
                var action = $(this).attr("action");

                var ok = $('#findForm').validate({
                    invalidHandler: function (form, validator) {
                        $(validator.invalidElements()[0]).focus();
                    },
                    focusInvalid: false,
                    rules: {
                        email: {
                            required: true,
                            email: true
                        }
                    }
                }).form();
                if (!ok) {
                    $('#buttonSearch').buttonEnable();
                    return false;
                }
                var serialized = $(this).serialize();
                // post form
                $.ajax({
                    type: "POST",
                    url: action,
                    data: serialized,
                    dataType: 'json',
                    success: function (json_data) {
                        if (json_data.success) {
                            var data = json_data.data;
                            $.each(data, function (i) {
                                $('.result_after').append("<p>" + data[i] + "</p>");
                            });
                            if (data.length == 0) {
                                $.jGrowl("Could not find your login page. <a href='/pricing'>Have you registered an account?</a>");
                                $('#buttonSearch').buttonEnable();
                                return false;
                            }
                            $('.result_before').fadeOut('normal', function () {
                                $('.result_after').fadeIn();
                            });
                        }
                        else {
                            $.jGrowl(json_data.message);
                        }
                        $('#buttonSearch').buttonEnable();
                        return false;
                    }
                });
                return false;
            });

            // input highlighters
            inputSelectors_init();
        });


    </script>
</asp:Content>
