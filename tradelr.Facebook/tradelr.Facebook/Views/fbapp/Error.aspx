<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Facebook.Master" Inherits="System.Web.Mvc.ViewPage<tradelr.Facebook.Models.facebook.FacebookViewData>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
<style type="text/css">
    .error {
    padding:20px;
    font-size:18px;
    color: #3B5998;
    background-color:#F6F8F5;
    margin:10px 0;
    position:relative;
    }

    .error .errorMsg {
        margin-left:40px;
    }
    .error .icon {
        position:absolute;
        top:15px;
    }
</style>
<div class="error">
<div class="icon">
<img src="http://fb.tradelr.com/Content/img/heading_error.png" alt="" />
</div>
<div class="errorMsg"><%= Model.errorMessage %>. Please <a href="<%= Model.pageUrl %>">try again</a>.</div>
</div>
</asp:Content>
