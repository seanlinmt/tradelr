<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<tradelr.Models.BaseViewModel>" %>
<%@ Import Namespace="tradelr.Library.Constants" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	Signing Out
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
<div class="h300px mt50">
<p class="larger"><img class="at" src="/Content/img/loading.gif" alt="" /></p>
<div id="fb-root"></div>
</div>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="AdditionalJS" runat="server">
<script type="text/javascript" src="http://connect.facebook.net/en_US/all.js"></script>
    <script type="text/javascript">
        $(document).ready(function () {
            try {

                FB.init({ appId: '<%= GeneralConstants.FACEBOOK_API_KEY %>', status: true, cookie: true, xfbml: false });

                FB.getLoginStatus(function (response) {
                    if (response.session) {
                        FB.logout(function (response) {
                            // user is now logged out
                            window.location = '/';
                        });
                    } else {
                        // no user session available, someone you dont know
                        window.location = '/';
                    }
                });
            }
            catch (e) {
                window.location = '/';
            }
            setTimeout(function () {
                window.location = '/';
            }, 5000);
        });
    </script>
</asp:Content>
