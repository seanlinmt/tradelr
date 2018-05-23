<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>
        <table class="nogrid">
            <tr>
                <th>
                    <span class='orderstatus_sent'>sent</span> / <span class='orderstatus_outstanding'>outstanding</span>
                </th>
                <td>
                    Invoice / Order has been sent but has not been viewed by the receipient. The outstanding status is shown to the receipient.
                </td>
            </tr>
            <tr>
                <th>
                    <span class='orderstatus_sent'>viewed</span> / <span class='orderstatus_outstanding'>outstanding</span>
                </th>
                <td>
                    Invoice / Order has been viewed by the receipient. The outstanding status is shown to the receipient.
                </td>
            </tr>
            <tr>
                <th>
                    <span class='orderstatus_partial'>partial</span>
                </th>
                <td>
                    Invoice / Order has been partially paid.
                </td>
            </tr>
             <tr>
                <th>
                    <span class='orderstatus_paid'>paid</span>
                </th>
                <td>
                    Invoice / Order has been fully paid.
                </td>
            </tr>
            <tr>
                <th>
                    <span class='orderstatus_shipped'>shipped</span>
                </th>
                <td>
                    Invoice / Order has been received / shipped.
                </td>
            </tr>
            <tr>
                <th>
                    <span class='orderstatus_reviewpayment'>review payment</span>
                </th>
                <td>
                    Buyer has entered a payment manually and needs to be verified by the seller.
                </td>
            </tr>
        </table>

