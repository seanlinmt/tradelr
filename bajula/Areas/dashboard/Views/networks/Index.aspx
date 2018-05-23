<%@ Page Title="" Language="C#" Inherits="System.Web.Mvc.ViewPage<tradelr.Models.BaseViewModel>"
    MasterPageFile="~/Views/Shared/Site.Master" %>

<asp:Content runat="server" ID="Content" ContentPlaceHolderID="TitleContent">
    Networks
</asp:Content>
<asp:Content runat="server" ID="Content1" ContentPlaceHolderID="MainContent">
    <div class="pt10">
        <h3 id="headingAutopost">
            network links</h3>
        <div class="mt10">
            <div class="info"><a id="networkContact" href="#">Contact us</a>
            if you want to link to a network not listed below.</div>
            <ul class="networks_list">
                        <li id="blogger" class="opacity75">
                            <img src="/Content/img/social/icons/blogger.png" alt="blogger" /></li>
                        <li id="ebay" class="opacity75">
                            <img src="/Content/img/social/icons/ebay.png" alt="ebay" /></li>
                        <li id="facebook" class="opacity75">
                            <img src="/Content/img/social/icons/facebook.png" alt="facebook" /></li>
                        <li id="gbase" class="opacity75">
                            <img src="/Content/img/social/icons/googlebase.png" alt="google" /></li>
                        <li id="shipwire" class="opacity75">
                            <img src="/Content/img/social/icons/shipwire.png" alt="shipwire" />
                            </li>
                        <li id="trademe" class="hidden opacity75">
                            <img src="/Content/img/social/icons/trademe.png" alt="trademe" />
                            </li>
                        <li id="tumblr" class="opacity75">
                            <img src="/Content/img/social/icons/tumblr.png" alt="tumblr" />
                            </li>
                        <li id="wordpress" class="opacity75">
                        <img src="/Content/img/social/icons/wordpress.png" alt="wordpress" />
                        </li>
                </ul>
            <div class="clear">
            </div>
            <div id="networks_content" class="mt50">
            </div>
            <div class="clear">
            </div>
        </div>
    </div>
</asp:Content>
<asp:Content runat="server" ID="Content2" ContentPlaceHolderID="AdditionalJS">
    <script type="text/javascript">
        $('li', '.networks_list').click(function () {
            $('li', '.networks_list').removeClass('opacity100');
            $(this).addClass('opacity100');
            var name = $(this).attr('id');
            $.get('/dashboard/networks/' + name, null, function (result) {
                $('#networks_content').html(result);
            });
            return false;
        });

        $(document).ready(function () {
            $('#navaccount').addClass('navselected_white');
            var hash = window.location.hash;
            if (hash != '') {
                $.get('/dashboard/networks/' + hash.substring(1), null, function (result) {
                    $('#networks_content').html(result);
                });
            }
        });
    </script>
</asp:Content>
