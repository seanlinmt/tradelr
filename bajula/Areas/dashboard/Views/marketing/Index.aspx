<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage" %>
<%@ Import Namespace="tradelr.Libraries.Extensions" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	Marketing
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
<div class="content_area">
        <ul class="hidden">
            <li><a href="#marketing_coupons">coupons</a></li>
        </ul>
        <div id="marketing_coupons" class="hidden">
            <% Html.RenderAction("Index", "Coupons"); %>
        </div>
    </div>

</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="AdditionalJS" runat="server">
<script type="text/javascript">
    $(document).ready(function () {
        $('#navstore').addClass('navselected');
        var $tabs = $('.content_area').tabs({
            select: function (event, ui) {
                inittabs(ui.index);
            }
        });
        inittabs($tabs.tabs('option', 'selected'));
    });

    function inittabs(index) {
        var active = $('.ui-tabs-nav > li:eq(' + index + ') > a', '.content_area').attr('href');
        switch (active) {
            case "#marketing_coupons":
                couponsBindToGrid();
                break;
            default:
                break;
        }
    }

</script>
</asp:Content>
