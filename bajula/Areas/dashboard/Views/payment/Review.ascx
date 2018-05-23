<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<tradelr.Models.payment.PaymentReviewViewModel>" %>
<form id="PaymentReviewForm" method="POST" action="/dashboard/payment/review/<%= Model.paymentid %>">
    <h3>Review Payment</h3>
    <h4><%= Model.order_title %></h4>
    <table class="w70p mb20">
        <tr><td><%= Model.order_type %> Total</td><td><%= Model.order_total %></td></tr>
        <tr><td>Payment Date</td><td><%= Model.date_created %></td></tr>
        <tr><td>&nbsp;</td><td></td></tr>
<tr><td>Paid Amount</td><td><strong><%= Model.payment_amount %></strong></td></tr>
<tr><td>Method</td><td><%= Model.payment_method %></td></tr>
<tr><td>Notes</td><td><%= Model.payment_notes %></td></tr>
    </table>
    <div class="mt10">
        <button id="buttonAccept" type="button" class="green ajax">accept</button>
        <button id="buttonReject" type="button" class="red ajax">reject</button>
        <button type="button" onclick="dialogBox_close();">cancel</button>
    </div>
    <%= Html.Hidden("decision") %>
</form>
<script type="text/javascript">
    $('#buttonAccept', '#PaymentReviewForm').click(function () {
        $('#decision', '#PaymentReviewForm').val('accept');
        $(this).trigger('submit');
    });

    $('#buttonReject', '#PaymentReviewForm').click(function () {
        $('#decision', '#PaymentReviewForm').val('reject');
        $(this).trigger('submit');
    });

    $('#PaymentReviewForm').submit(function () {
        var action = $(this).attr('action');
        var data = $(this).serialize();
        
        $.post(action, data, function (json_result) {
            if (json_result.success) {
                window.location.reload();
            } else {
                $.jGrowl(json_result.message);
            }
        });
        return false;
    });
</script>