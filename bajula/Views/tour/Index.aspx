<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/NotLoggedIn.Master"
    Inherits="System.Web.Mvc.ViewPage<tradelr.Models.tour.TourViewData>" %>
<%@ Import Namespace="tradelr.Libraries.Extensions" %>
<%@ Import Namespace="tradelr.Library.Constants" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div id="banner">
        <div class="content">
            <h1>
                Feature Tour</h1>
        </div>
    </div>
    <div class="banner_main tour">
        <div class="content pt20">
            <div id="tour_nav" class="fl">
            <ul id="tour_items">
            <li id="tour_transactions" <%= Model.control==TradelrControls.tour_transactions?"class='selected'":"" %>><span><a href="/tour/transactions">Track Transactions</a></span></li>
            <li id="tour_inventory" <%= Model.control==TradelrControls.tour_inventory?"class='selected'":"" %>><span><a href="/tour/inventory">Manage Products</a></span></li>
            <li id="tour_contacts" <%= Model.control==TradelrControls.tour_contacts?"class='selected'":"" %>><span><a href="/tour/contacts">Organize Contacts</a></span></li>
            <li id="tour_engage" <%= Model.control==TradelrControls.tour_engage?"class='selected'":"" %>><span><a href="/tour/engage">Engage Customers</a></span></li>
            <li id="tour_store" <%= Model.control==TradelrControls.tour_store?"class='selected'":"" %>><span><a href="/tour/store">Web Store</a></span></li>
            <li id="tour_web" <%= Model.control==TradelrControls.tour_web?"class='selected'":"" %>><span><a href="/tour/web">Web-based</a></span></li>
            </ul>
            </div>
            <div id="tour_content_top" class="ml200"></div>
            <div id="tour_content" class="ml200">
                <% Html.RenderControl(Model.control); %>
            </div>
            <div id="tour_content_bottom" class="ml200"></div>
        </div>
    </div>
    <div id="banner_bottom">
    <a href="<%=GeneralConstants.HTTP_HOST%>/pricing"><img src="/Content/img/frontpg/signup_free.png" alt="sign up free" /></a> 
    30 days free trial
    </div>
    <div class="clear"></div>
    <script type="text/javascript">
        var pos = $('#tour_nav').offset();
        $(window).scroll(function () {
            if (self.pageYOffset > pos.top) {
                $('#tour_nav').css({ position: 'fixed', top: 0 });
            }
            else {
                $('#tour_nav').css({ position: 'static', top: 'auto' });
            }
        });
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="head" runat="server">
    <%= Model.title %>
</asp:Content>

