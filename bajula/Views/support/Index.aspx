<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/NotLoggedIn.Master"
    Inherits="System.Web.Mvc.ViewPage" %>
<%@ Import Namespace="tradelr.Libraries.Extensions" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div id="banner">
        <div class="content">
            <h1>
                Contact Us / Support</h1>
        </div>
    </div>
    <div class="banner_main">
    <div class="content tour">
    <div class="w600px">
    <p>Contact us in any language.</p>
        <p>Let us know how we can serve you better or how we can improve. Response can be anywhere between a few minutes to up to 24 hours.</p>
        </div>
        <div class="form_content w600px">
            <% Html.RenderControl(TradelrControls.contactUs); %>
        </div>
    </div>
    </div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="AdditionalJS" runat="server">
    <%= Html.RegisterViewJS() %>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="head" runat="server">
    Contact Us
</asp:Content>
