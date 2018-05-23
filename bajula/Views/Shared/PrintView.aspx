<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<tradelr.Models.transactions.PrintView>" %>

<%@ Import Namespace="tradelr.Common.Library" %>
<%@ Import Namespace="tradelr.Libraries.Extensions" %>
<%@ Import Namespace="tradelr.Libraries.Helpers" %>
<%@ Import Namespace="tradelr.Library" %>
<%@ Import Namespace="tradelr.Models.transactions" %>
<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    <%= Model.primaryView.order.TransactionType.ToDescriptionString() + " #" + Model.primaryView.order.orderNumber.ToString("D8") %>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div id="added" class="boxSuccess hidden">
        <h3>
            Your invoice has been sent</h3>
        <p>
            The following contacts were e-mailed:</p>
        <ul id="sentContacts">
        </ul>
    </div>
    <div id="printview_content">
        <ul class="hidden">
            <li><a href="#primaryView">
                <%= Model.primaryView.order.TransactionType.ToDescriptionString().ToLower() %></a></li>
            <% if (Model.secondaryView != null)
               {%>
            <li><a href="#secondaryView">
                <%= Model.secondaryView.order.TransactionType.ToDescriptionString().ToLower() %></a></li>
            <% } %>
        </ul>
        <div id="primaryView">
            <% if (Model.primaryView.order.TransactionType == TransactionType.ORDER)
               {
                   Html.RenderPartial("~/Areas/dashboard/Views/orders/orderView.ascx", Model.primaryView);
               }
               else
               {
                   Html.RenderPartial("~/Areas/dashboard/Views/invoices/invoiceView.ascx", Model.primaryView);
               } %>
        </div>
        <% if (Model.secondaryView != null)
           {%>
        <div id="secondaryView">
            <% if (Model.primaryView.order.TransactionType == TransactionType.ORDER)
               {
                   Html.RenderPartial("~/Areas/dashboard/Views/orders/orderView.ascx", Model.secondaryView);
               }
               else
               {
                   Html.RenderPartial("~/Areas/dashboard/Views/invoices/invoiceView.ascx", Model.secondaryView);
               } %>
        </div>
        <% } %>
    </div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="AdditionalJS" runat="server">
    <script type="text/javascript">
        $(document).ready(function() {
            // init tabs
            $('#printview_content').tabs();
        });
    </script>
</asp:Content>
