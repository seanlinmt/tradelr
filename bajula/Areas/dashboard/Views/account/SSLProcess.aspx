<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Login.Master" Inherits="System.Web.Mvc.ViewPage<tradelr.Areas.dashboard.Models.account.SSLRegistrationViewModel>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    SSL Certificate Registration
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div id="banner">
        <div class="content">
            <h1>
                <%= Model.orgName %></h1>
        </div>
    </div>
    <div class="banner_main">
        <div class="content">
            <h3>
                SSL Certificate Registration</h3>
            <h4>Please do not close this window.</h4>
            <p class="larger font_darkgrey">
                <img src='/Content/img/loading_circle.gif' alt="" /> Give us a minute while we confirm your payment. We will email you again once your certificate is ready.</p>
        </div>
    </div>
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="AdditionalJS" runat="server">
<script type="text/javascript">
    $(document).ready(function () {
        ssl_registration_poll();
    });
    var ssl_registration_poll = function () {
        $.post("/dashboard/account/sslprocesspoll", { plan: '<%= Model.ssl_plan %>' }, function (json_result) {
            if (json_result.success) {
                window.location = json_result.data;
            }
            else {
                setTimeout("ssl_registration_poll()", 5000);
            }
        });
    }; 
</script>
</asp:Content>
