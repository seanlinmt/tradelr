<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Facebook.Master" Inherits="System.Web.Mvc.ViewPage<tradelr.Facebook.Models.facebook.FacebookViewData>" %>
<%@ Import Namespace="tradelr.Library.Constants" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="mt10">
        <h3>
            If you know your store address, enter it below. If not, choose one and we will create a new account for you.</h3>
        <div>
        <form action="<%= GeneralConstants.FACEBOOK_APP_URL + "/configure" %>" method="post">
            <input type="hidden" name="pageid" value="<%= Model.pageID %>" />
            <input type="hidden" name="profileid" value="<%= Model.profileID %>" />
            <input type="hidden" name="pagetype" value="<%= Model.pageType %>" />
            <div class="pt20 pb20">
            http:// <input type="text" name="address" /> .tradelr.com
            </div>
                <div class="mt10">
                <input type="submit" value="Get Started!" />
                </div>
        </form>
        </div>
    </div>
</asp:Content>
