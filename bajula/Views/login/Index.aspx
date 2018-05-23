<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Login.Master" Inherits="System.Web.Mvc.ViewPage<tradelr.Models.login.LoginViewModel>" %>

<%@ Import Namespace="tradelr.Libraries.Extensions" %>
<asp:Content ID="Content2" ContentPlaceHolderID="head" runat="server">
    <%= Model.loginPageName %>
</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div id="banner">
        <div class="content pl10 pr10">
            <h1 title="Company Name">
                <%= Model.loginPageName %></h1>
        </div>
    </div>
    <div class="banner_main pl10 pr10">
        <% if (Model.showRegistrationHelp)
           { %>
        <div class="content">
            <div class="info" style="font-size: 20px; line-height: 1.6; background-position: 10px 10px;">
                Before you can login, we will need to verify your email account. A verification
                email is being sent to you now. Your login information can also be found in this email.
                <br />
                <br />
                Please don't forget to check your junk mail folder if you have not received it after
                10 minutes.</div>
        </div>
        <%  } %>
        <% Html.RenderControl(TradelrControls.signIn); %>
        <div class="fl">
            <h3 class="mt20">
                Or sign in using the following</h3>
            <p>
                <span id="signin-facebook" class="pointer"></span>
            </p>
        </div>
    </div>
    <div class="clear">
    </div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="AdditionalJS" runat="server">
    <script type="text/javascript">
        $(document).ready(function () {
            $('#signin-facebook').click(function () {
                var redirectUrl = querySt("redirect");
                if (redirectUrl == undefined || redirectUrl == '') {
                    window.location = "/fb/login";
                }
                else {
                    window.location = "/fb/login?redirect=" + redirectUrl;
                }

            });
        });
    </script>
</asp:Content>
