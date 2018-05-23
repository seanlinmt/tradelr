<%@ Page Title="" Language="C#" Inherits="System.Web.Mvc.ViewPage<tradelr.Models.BaseViewModel>" MasterPageFile="~/Views/Shared/Login.Master" %>
<asp:Content runat="server" ID="Title" ContentPlaceHolderID="head">Import Products</asp:Content>
<asp:Content runat="server" ID="Main" ContentPlaceHolderID="MainContent">
    <div id="banner">
        <div class="content">
            <h1>
                Import existing products</h1>
        </div>
    </div>
    <div class="container_center">
        <div class="mt10">
            <ul class="networks_list">
                <li>
                    <ul>
                        <li id="etsy" class="opacity75 mr20">
                            <img src="/Content/img/social/icons/etsy_32.png" alt="etsy" />
                            Etsy</li>
                        <li id="ebay" class="opacity75 mr20">
                            <img src="/Content/img/social/icons/ebay_32.png" alt="ebay" />
                            Ebay</li>
                        <li id="facebookImport" class="opacity75 mr20">
                            <img src="/Content/img/social/icons/facebook_32.png" alt="facebook" />
                            Facebook</li>
                            <li>
                                <button type="button" class="green" onclick="javascript:window.location = '/';">do this later</button>
                            </li>
                    </ul>
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
<asp:Content runat="server" ID="AdditionalJs" ContentPlaceHolderID="AdditionalJS">
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
