<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<tradelr.Models.BaseViewModel>" %>
<%@ Import Namespace="tradelr.Libraries.Helpers" %>
<%@ Import Namespace="tradelr.Library" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	Test
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
<div class="mt10">
<form id="pageForm" method="post" action="<%= Url.Action("Save","Pages") %>">
        
<div class="form_entry">
    <textarea id="pagecontent" name="content" class="w700px" style="height:200px;"></textarea>
</div>
<div class="mt10">
    <button id="buttonSave" type="button" class="large green ajax">
        <img src="/Content/img/save.png" alt='' />
        save</button>
</div>
</form>
</div>

</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="HeadContent" runat="server">
<link media="screen, print" type="text/css" rel="stylesheet" href="/editor/jquery.wysiwyg.css" />
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="AdditionalJS" runat="server">
    <script src="/editor/jquery.wysiwyg.js?v=1" type="text/javascript"></script>
<script type="text/javascript">
    $(document).ready(function() {
        $('#pagecontent', '#pageForm').wysiwyg({
            autoGrow: true
        });
    });
</script>
</asp:Content>
