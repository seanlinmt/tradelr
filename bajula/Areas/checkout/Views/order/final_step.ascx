<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<tradelr.Areas.checkout.Models.CreateOrderViewModel>" %>
<div id="shippingmethods" class="hidden">
    <form id="shippingForm" method="post" action="/checkout/order/<%= Model.cartid %>/create">
    <% if (!Model.isDigitalOrder)
       {
           Html.RenderPartial("shipping_step", Model.shipping);
       } %>

    <% if (Model.hasShippingMethods || Model.isDigitalOrder)
           {%>
    <% Html.RenderPartial("payment_step", Model.payment); %>

    <div class="mt20">
        <input type="hidden" name="status" value="SHIPPING_OK" />
        <button class="green ajax" type="submit" id="buttonPayment">
            complete my purchase</button>
    </div>
    <%
           }
           else
           {%>
    <p class="italic">
        We are currently unable to determine shipping costs to your destination. Please
        submit your order and we will contact you soon.</p>
    <div class="pad10">
        <input type="hidden" name="status" value="SHIPPING_FAIL" />
        <button class="green ajax" type="submit" id="buttonSave">submit your order</button>
    </div>
    <%}%>
    </form>
</div>
<script type="text/javascript">
    var win1;
    function check() {
        if (win1.closed) {
            window.location = '/checkout/order/<%= Model.cartid %>';
        } else {
            setTimeout("check()", 1);
        }
    }

    $(document).ready(function () {
        if (!$('#shippingmethods').is(':visible')) {
            $('#shippingmethods').slideDown();
        }

        $('#editAddresses', '#shippingmethods').click(function () {
            $('#shippingmethods').slideUp(function () {
                $('#addresses').slideDown();
            });
            return false;
        });

        $('input:first', '#payment_methods_list').attr('checked', 'checked');

        $('#shippingForm').submit(function () {
            if (self != top) {
                $('#buttonPayment').buttonDisable();
                var action = $(this).attr("action") + ".json";
                var serialized = $(this).serialize();
                win1 = window.open(tradelr.util.buildUrl(action + "?" + serialized), '', 'width=1000px,height=500px,toolbar=0');
                check();
                return false;
            }
            else {
                return true;
            }
        });
    });
</script>
