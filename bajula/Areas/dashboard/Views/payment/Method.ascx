<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Payment>" %>
<%@ Import Namespace="tradelr.Models.payment"%>
<div id="paymentMethodForm">
<h3>select payment method</h3>
<ul id="payment_methods_list" class="mt20 ml20 mb20">
            <% foreach (var method in Model.paymentMethods.items)
               {%>
            <li>
                <input id="pmethod_<%= method.Value %>" type="radio" name="paymentmethod" value="<%= method.Value %>" /><label
                    for="pmethod_<%= method.Value %>"><%= method.Text %></label></li>
            <%} %>
        </ul>
<div>
<%= Html.Hidden("orderID", Model.orderID)%>
<button id="ButtonPaymentSelect" class="green">continue</button>
<button id="ButtonPaymentCancel">back</button>
</div>
</div>
<script type="text/javascript">
    $(document).ready(function () {
        $("input[name='paymentmethod']:first", '#paymentMethodForm').attr('checked', true);

        // this appears in a dialog box
        $('#ButtonPaymentCancel', '#paymentMethodForm').bind('click', function () {
            dialogBox_close();
            return false;
        });

        $('#ButtonPaymentSelect', '#paymentMethodForm').bind('click', function () {
            // if manual then just create order
            var orderid = $('#orderID', '#paymentMethodForm').val();
            var method = $("input[name='paymentmethod']:checked", '#paymentMethodForm').val();
            if (method == "paypal") {
                $('#ButtonPaymentSelect', '#paymentMethodForm').buttonDisable();
                $.post('/dashboard/payment/paypalcheckout/' + orderid, function (json_data) {
                    if (json_data.success) {
                        window.location = json_data.data;
                    }
                    else {
                        $.jGrowl(json_data.message);
                    }
                    $('#ButtonPaymentSelect', '#paymentMethodForm').buttonEnable();
                }, "json");
            }
            else {
                dialogBox_close();
                dialogBox_open('/dashboard/payment/new/' + orderid, 550);
            }
        });
    });
</script>
