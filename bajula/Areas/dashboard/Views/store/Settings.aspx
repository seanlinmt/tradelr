<%@ Page Title="" Language="C#" Inherits="System.Web.Mvc.ViewPage<tradelr.Areas.dashboard.Models.store.StoreSettingsViewModel>"
    MasterPageFile="~/Views/Shared/Site.Master" %>
<%@ Import Namespace="tradelr.Libraries.Helpers" %>
<%@ Import Namespace="tradelr.Library" %>
<%@ Import Namespace="tradelr.Library.Constants" %>
<asp:Content runat="server" ID="Title" ContentPlaceHolderID="TitleContent">
    Store Settings
</asp:Content>
<asp:Content runat="server" ID="Head" ContentPlaceHolderID="HeadContent">
</asp:Content>
<asp:Content runat="server" ID="Main" ContentPlaceHolderID="MainContent">
    <div class="content_area_2col">
        <div id="content_main">
            <h3 id="headingSettings" class="hidden mt10 fl">
                store settings</h3>
            <div id="settings_tabs">
                <ul class="hidden">
                    <li><a href="#basic">settings</a></li>
                    <li><a href="#information">policies</a></li>
                    <li><a href="#links">navigation</a></li>
                    <li><a href="#pages">pages & blogs</a></li>
                </ul>
                <div id="basic" class="hidden">
                <% Html.RenderPartial("general", Model.general); %>
                </div>
                <div id="information" class="hidden">
                <% Html.RenderPartial("policies", Model.policies); %>
                </div>
                <div id="links" class="hidden">
                <% Html.RenderAction("index", "links", new { area = "Dashboard"}); %>
                </div>
                <div id="pages" class="hidden">
                <% Html.RenderAction("index", "pages", new { area = "Dashboard" }); %>
                </div>
            </div>
        </div>
        <div id="content_side">
            <div class="top">
            </div>
            <div class="middle">
                <div class="content_side_box">
                    <ul>
                        <li><a class="icon_continue" href="/dashboard/account#account_payment">payment gateways</a></li>
                        <li><a class="custom-store" href="/dashboard/themes">store themes</a></li>
                        <li><a class="custom-map" id="googleMapLocationAdd" href="#">location map</a></li>
                        <li><a class="link_facebook" href="http://apps.facebook.com/tradelr">facebook store</a></li>
                    </ul>
                </div>
            </div>
            <div class="bottom">
            </div>
        </div>
        <div class="clear"></div>
    </div>
</asp:Content>
<asp:Content runat="server" ID="AdditionalJs" ContentPlaceHolderID="AdditionalJS">
    <script type="text/javascript" src="//www.google.com/jsapi?key=KEY"></script>
<% if (!GeneralConstants.DEBUG) {%>
<script type="text/javascript" src="/Scripts/tinymce/tiny_mce.js"></script>
<% } else { %>
<script type="text/javascript" src="/Scripts/tinymce/tiny_mce_src.js"></script>
<% } %>
<script src="/jsapi/uploader" type="text/javascript"></script>
<script type="text/javascript">
    $(document).ready(function () {
        $('#navstore').addClass('navselected');
        var $tabs = tradelr.tabs.init('#settings_tabs');

        if (window.location.hash != '') {
            try {
                $tabs.tabs('select', window.location.hash);
            } catch(e) {
            } 
        }

        $('#returnPolicy').limit('1000', '#returnPolicy-charsleft');
        $('#paymentTerms').limit('1000', '#paymentTerms-charsleft');

        // init google maps
        $('#googleMapLocationAdd').click(function () {
            dialogBox_open('/google/map/<%= Model.general.orgid %>', 800);
            return false;
        });

        // input highlighters
        inputSelectors_init();
        init_autogrow();
    });
</script>
</asp:Content>
