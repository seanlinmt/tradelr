<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/NotLoggedIn.Master"
    Inherits="System.Web.Mvc.ViewPage" %>

<%@ Import Namespace="tradelr.Libraries.Extensions" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div id="banner">
        <div class="content">
            <h1>
                Plans and Sign Up</h1>
        </div>
    </div>
    <div class="banner_main">
        <p class="ac pt20 pb10" style="font-size: 22px;">
            30 days free trial on all plans. 
            No obligation, no credit card or sales call required. </p>
        <div class="content pt50">
            <% Html.RenderPartial("~/Views/pricing/plans.ascx", null); %>
        </div>
    </div>
    <div class="banner_main bt bg_lightgrey">
        <div class="content">
            <ul class="pricing_faq">
                <li><h3>Do  you take a percentage of each sale or invoice?</h3>
                    <p>
                        Nope. Your money is all yours. There are no listing or transaction fees.</p>
                </li>
                <li><h3>Am I tied to a long term contract?</h3>
                    <p>
                        No, you pay from month to month and you're free to cancel your account at any time.</p>
                </li>
                <li><h3>Who controls my data? Can I still access my data if I close my account?</h3>
                    <p>
                        Your data belongs to you and you control it. Even after your trial expires or if you decide
                        to close your account, you can still access your data.</p>
                </li>
            </ul>
            <ul class="pricing_faq ml50">
                <li><h3>Can I upgrade or downgrade my account?</h3>
                    <p>
                        Yes, you can do this at any time.</p>
                </li>
                <li><h3>Any deals for non-profits?</h3>
                    <p>
                        Tell us how you're making the world a better place and we'll set you up.</p>
                </li>
            </ul>
            <div class="clear"></div>
        </div>
    </div>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="head" runat="server">
    Plans, Pricing &amp; Sign Up
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="AdditionalJS" runat="server">
    <script type="text/javascript">
        $(document).ready(function () {
            // manually highlight third column
            $('#pricing tr').find('td:eq(2)').addClass('highlight');
            $('#pricing tr:first').find('td:eq(2)').addClass('pro');
            $('#pricing tr:last').find('td:eq(2)').addClass('proend');
        });
    </script>
    
</asp:Content>
