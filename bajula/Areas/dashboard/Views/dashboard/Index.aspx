<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<tradelr.Models.home.HomeViewData>" %>
<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Home
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="content_area_2col">
        <%
            if (Model.isAdmin)
            {
                Html.RenderPartial("homeAdmin", Model);
            }
            else
            {
                Html.RenderPartial("homeUser", Model);
            }
             %>
    </div>
    <span id="panelIndex" class="hidden"><%= Model.panelIndex %></span>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="AdditionalJS" runat="server">
<script type="text/javascript">
$(document).ready(function () {
    $('#profile').find('.edit').click(function () {
        window.location = '/profile/edit';
    });

    // remove add class
    $('.add', '.sideboxEntry').remove();
});
</script>
</asp:Content>
