<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Login.Master" Inherits="System.Web.Mvc.ViewPage<tradelr.Models.account.viewmodel.AccountVerify>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    Email Confirmation
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
<div id="banner">
        <div class="content">
            <h1 title="Company Name"><%= Model.domainName%></h1>
        </div>
    </div>
    <div class="banner_main">
    <div class="content">
    <h3>Email Confirmation</h3>
    <% if (Model.isValidCode)
           {%>
            <p class="larger font_darkgrey"><img src='/Content/img/loading_circle.gif' alt="" /> Your email address has been verified! Please give us a few seconds while we prepare your dashboard.</p>
        <% }
           else
           { %>
            <p class="larger font_darkgrey">The confirmation code is invalid.</p>
        <%} %>
        </div>
    </div>
    <div class="clear">
        </div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="AdditionalJS" runat="server">
<script type="text/javascript">
    $(document).ready(function() {
        setTimeout(function() { window.location = '/dashboard'; }, 5000);
    });
    </script>
</asp:Content>
