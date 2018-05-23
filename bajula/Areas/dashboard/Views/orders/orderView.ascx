<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<OrderView>" %>
<%@ Import Namespace="tradelr.Libraries.Extensions" %>
<%@ Import Namespace="tradelr.Library" %>
<%@ Import Namespace="tradelr.Library.Constants" %>
<%@ Import Namespace="tradelr.Models.transactions" %>
<div class="buttonRow_big">
    <%= Model.buttons %>
</div>
<div class="clear">
</div>
<div id="order_container">
    <div id="order_content">
        <div id="order_top">
            <% if (!string.IsNullOrEmpty(Model.banner))
               { %>
            <div id="order_logo">
                <%= Model.banner %>
            </div>
            <% }%>
            <div class="order_address_sender" title="Sender name & address">
                <%= Model.sender %>
                <p><%= Model.order.orderDate.Value.ToString(GeneralConstants.DATEFORMAT_INVOICE) %></p>
            </div>
            <div id="order_title">
                <div class="ribbon_main">
                <h2>order <strong>#<%= Model.order.orderNumber.ToString("D8")%></strong></h2>
                </div>
                <div class="ribbon_main_triangle"></div>
                <%= Model.bannerStatus %>
            <div class="clear">
            </div>
        </div>
        <div class="clear">
            </div>
        </div>
        <div id="order_side">
        <div id="order_receiver">
                    <div class="order_address_receiver" title="Receiver name & address">
                        <%= Model.receiver %></div>
                </div>
        </div>
        <div id="order_main">
        <div id="order_summary">
                    <%if (!string.IsNullOrEmpty(Model.order.terms))
                      {%>
                    <h4>
                        Notes</h4>
                    <div>
                        <%=Model.order.terms.ToHtmlBreak()%></div>
                        <% } %>
                    </div>
        </div>
        <div class="clear"></div>
        <div id="order_items_container">
                <table id="order_items" cellspacing="4px">
                    <tbody>
                        <tr>
                            <th>
                                ITEM
                            </th>
                            <th class="ac">
                                QTY
                            </th>
                            <th class="ar">
                                PRICE (<%= Model.currency.code%>)
                            </th>
                            <th class="ar">
                                TAX
                            </th>
                            <th class="end ar">
                                SUBTOTAL (<%= Model.currency.code%>)
                            </th>
                        </tr>
                        <%
                            foreach (var item in Model.order.orderItems)
                            {%>
                        <tr>
                            <td class="orderitem_description">
                                <strong><%= item.SKU%></strong> <%= item.description %> 
                            </td>
                            <td class="orderitem_quantity">
                                <%= item.quantity %>
                            </td>
                            <td class="orderitem_unitPrice">
                                <%=item.UnitPrice.ToString("n" + Model.currency.decimalCount) %>
                            </td>
                            <td class="orderitem_tax">
                                <%= item.tax %>
                            </td>
                            <td class="orderitem_subtotal">
                                <%= item.subTotalString%>
                            </td>
                        </tr>
                        <% } %>
                    </tbody>
                    <tfoot>
            <tr>
                    <td colspan="4" class="bt">
                        Subtotal
                    </td>
                    <td class="bt">
                        <%= Model.order.subTotal%>
                    </td>
                </tr>
                <%
                    if (!string.IsNullOrEmpty(Model.order.shippingCost))
                    {%>
                <tr>
                    <td colspan="4">
                        Shipping Cost
                    </td>
                    <td>
                        <%= Model.order.shippingCost%>
                    </td>
                </tr>
                <%
                            }%>
                             <%
                    if (!string.IsNullOrEmpty(Model.order.totalTax))
                    {%>
                <tr>
                    <td colspan="4">
                        Tax
                    </td>
                    <td>
                        <%= Model.order.totalTax%>
                    </td>
                </tr>
                <%
                            }%>
                <%
                    if (!string.IsNullOrEmpty(Model.order.discount))
                    {%>
                <tr>
                    <td colspan="4">
                        Discount
                        <%= !string.IsNullOrEmpty(Model.order.discountCode)? string.Format("({0})",Model.order.discountCode):""  %>
                    </td>
                    <td>
                        <%= Model.order.discountType == "%"?string.Format("{0}%", Model.order.discount) :string.Format("- {0}", Model.order.discount) %>
                    </td>
                </tr>
                <%
                            }%>
                <tr>
                    <td class="due" colspan="4">
                        Total
                    </td>
                    <td class="due">
                        <%= Model.order.orderTotal%>
                    </td>
                </tr>
                <tr>
                    <td colspan="4">
                        Amount Paid
                    </td>
                    <td>
                        -
                        <%= Model.order.totalPaid%>
                    </td>
                </tr>
                <tr>
                <td colspan="2"></td>
                    <td class="due_end highlight_red" colspan="2">
                        Total Due
                    </td>
                    <td class="due_end highlight_red">
                        <%= Model.order.amountDue.ToString("n" + Model.currency.decimalCount)%>
                    </td>
                </tr>
                </tfoot>
                </table>
                </div>
                <div class="clear">
                </div>
                <div id="order_messages">
                <div class="ribbon_heading">
                <h2>message history</h2>
                </div>
                <div class="ribbon_heading_triangle"></div>
                <div class="clear">
                </div>
                <a href="#" class="icon_add" id="comment_add_<%= Model.order.id %>">add a note, send a message to the other party or just leave additional information about this transaction</a>
                    <div id="comments_<%= Model.order.id %>" class="mt20">
                        <%
                            foreach (var comment in Model.comments)
                            {
                                Html.RenderControl(TradelrControls.comments, comment);
                            } %>
                    </div>

            </div>
    </div>
</div>
<script type="text/javascript">
    $(document).ready(function () {
        $("#comment_add_<%= Model.order.id %>").click(function () {
            $(this).insertTextArea('/dashboard/transactions/addnote/<%= Model.transactionID %>',
                '600px',
                '',
                'A notification email will be sent. Please use plain text (no HTML tags).',
                500,
                function (json_result) {
                    if (json_result.success) {
                        $("#comments_<%= Model.order.id %>").prepend(json_result.data);
                        return true;
                    }
                    
                    $.jGrowl(json_result.message);
                    return false;
                });
            return false;
        });

    });
</script>