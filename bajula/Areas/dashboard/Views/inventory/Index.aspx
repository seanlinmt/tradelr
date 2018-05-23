<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<tradelr.Models.inventory.InventoryViewData>" %>
<%@ Import Namespace="tradelr.Libraries.Extensions" %>
<%@ Import Namespace="tradelr.Models.users" %>
<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Inventory
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div id="inventory_tabs" class="content_area">
        <ul class="hidden">
            <% if (Model.permission.HasPermission(UserPermission.INVENTORY_VIEW))
               {%>
            <li><a href="#inventory_mine">my inventory</a></li>
            <%  
                }%>
        </ul>
        <% if (Model.permission.HasPermission(UserPermission.INVENTORY_VIEW))
           {%>
        <div id="inventory_mine" class="hidden">
            <% Html.RenderPartial("mine", Model); %>
        </div>
        <%
            }%>
    </div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="AdditionalJS" runat="server">
<script type="text/javascript">
    $(document).ready(function () {
        $('#navinventory').addClass('navselected');
        tradelr.tabs.init('#inventory_tabs');
        productBindToGrid(getFilterByField("#inventory_mine"));
    });
</script>
</asp:Content>
