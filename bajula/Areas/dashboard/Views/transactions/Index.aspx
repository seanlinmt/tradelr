<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<tradelr.Models.transactions.viewmodel.TransactionViewModel>" %>
<%@ Import Namespace="tradelr.Models.users" %>
<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Transactions
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="content_area">
        <ul class="hidden">
            <li><a href="#transactions_all">transactions</a></li>
        </ul>
        <div id="transactions_all" class="hidden">
            <% Html.RenderPartial("transactions_all", Model); %>
        </div>
    </div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="AdditionalJS" runat="server">
<script type="text/javascript">
    $(document).ready(function () {
        $('#navsales').addClass('navselected');
        var $tabs = $('.content_area').tabs({
            select: function (event, ui) {
                inittabs(ui.index);
            }
        });
        
        $('.hover_del', '.ui-tabs-nav').live('click', function () {
            tradelr.tabs.del(this, '.content_area');
            return false;
        });
        
        inittabs($tabs.tabs('option', 'selected'));
        $('#transactionsGridView').delegate(".jqview,.jqreview,.orderstatus_reviewpayment", 'click', function () {
            var href = $(this).attr('href');
            var id, name;
            var text = $(this).parents('tr').find('td:first span').text();
            var idx = text.indexOf('#');
            if (idx == -1) {
                tradelr.error("Failed to obtain id to view transaction.ID = " + $(this).text());
            }

            id = text.substring(idx + 1);

            if (href.indexOf('invoice') != -1) {
                name = "invoice #" + id;
                id = '#i_' + id;
            }
            else {
                name = "order #" + id;
                id = '#o_' + id;
            }
            tradelr.tabs.add(href, id, name, '.content_area');
            return false;
        });
    });

    function inittabs(index) {
        var active = $('.ui-tabs-nav > li:eq(' + index + ') > a', '.content_area').attr('href');
        switch (active) {
            case "#transactions_all":
                transactionsBindToGrid();
                break;
            default:
                break;
        }
    }
</script>
</asp:Content>
