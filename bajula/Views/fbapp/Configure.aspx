<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/facebook/FacebookTab.Master" Inherits="System.Web.Mvc.ViewPage<tradelr.Models.facebook.app.FacebookConfigureViewModel>" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div id="tab_configure" class="mt10">
        <img src="/Content/img/tradelr.png" alt="tradelr logo"/>
        <h3>To start using your Facebook store, we just need the following information.</h3>
        <form action="<%= Url.Action("Configure","Fbapp") %>" method="post">
            <input type="hidden" name="pageid" value="<%= Model.pageID %>" />
            <input type="hidden" name="profileid" value="<%= Model.profileID %>" />
            <input type="hidden" name="token" value="<%= Model.signed_request %>" />
            <div class="pt20 pb20">
            <h2>Store Address</h2>
        <p>
            If you have already created a Tradelr account, enter your existing store name below. If not, choose a new address.</p>
            <strong>http:// <input id="tradelr_address" type="text" name="address" /> .tradelr.com</strong>
            </div>
            <div class="pb20">
            <h2>Affiliate ID</h2>
            <p>If you have an affiliate ID, enter it below</p>
            <input id="tradelr_affiliate" type="text" name="affiliate" />
            </div>
                <div class="mt10">
                <input type="submit" value="Create Facebook Store" />
                </div>
        </form>
    </div>
    <script type="text/javascript">
        $('#tradelr_address').alphanumeric();
    </script>
</asp:Content>
