<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<tradelr.Areas.dashboard.Models.account.payment.PaymentMethodViewModel>" %>
<%@ Import Namespace="tradelr.Library.payment" %>
<form id="paymentMethodForm" action="/dashboard/payment/method_save" method="post">
    <div class="fl">
        <div class="form_entry">
            <div class="form_label">
                <label for="method">
                    Method</label>
            </div>
            <%= Html.DropDownList("method", Model.methodList)%>
        </div>
    </div>
    <div class="fl ml20">
        <div id="name_entry" class="form_entry hidden">
            <div class="form_label">
                <label for="title">
                    Name</label>
            </div>
            <%= Html.TextBox("name", Model.name)%>
        </div>
    </div>
    <div class="clear"></div>
    <div id="identifier_entry" class="hidden">
    <div class="fl">
        <div class="form_entry">
            <div class="form_label">
                <label for="identifier">
                    Paypal Email</label>
            </div>
            <%= Html.TextBox("identifier", Model.identifier)%>
        </div>
    </div>
    <div class="fl ml40 mt10 w300px">
        <p class="tip">Don't have a <img src="/Content/img/payments/paypal.jpg" alt="Paypal" /> account? <a
                        href="https://www.paypal.com/nz/mrb/pal=QU9L92B97WJLY" target="_blank">Get one free now!</a>. Paypal allows direct credit card payments.</p>
    </div>
    <div class="clear"></div>
    </div>
        <div class="form_entry">
            <div class="form_label">
                <label for="details">
                    Payment Instructions</label>
            </div>
            <%= Html.TextArea("instructions", Model.instructions)%>
        </div>
        <div class="pt5">
            <button id="saveButton" type="button" class="green ajax">
                save</button>
            <button id="cancelButton" type="button">
                cancel</button>
        </div>
        <%= Html.Hidden("id", Model.id) %>
</form>
<script type="text/javascript">
    $('#method', '#paymentMethodForm').change(function () {
        var selected = $(this).val();
        $('#identifier_entry, #name_entry', '#paymentMethodForm').hide();
        
        switch (selected) {
            case "<%= PaymentMethod.Paypal %>":
                $('#identifier_entry', '#paymentMethodForm').show();
                break;
            case "<%= PaymentMethod.Other %>":
                $('#name_entry', '#paymentMethodForm').show();
            default:
                break;
        }
    });

    $(document).ready(function () {
        $('#method', '#paymentMethodForm').trigger('change');

        init_autogrow('#paymentMethodForm');
        $('#cancelButton', '#paymentMethodForm').bind("click", function () {
            dialogBox_close();
            return false;
        });

        $('#saveButton', '#paymentMethodForm').bind("click", function () {
            $(this).trigger('submit');
            return false;
        });

        $('#paymentMethodForm').submit(function () {
            var action = $(this).attr("action");
            var serialized = $(this).serialize();

            if ($('#method', '#paymentMethodForm').val() == "<%= PaymentMethod.Paypal %>" &&
                $('#identifier', '#paymentMethodForm').val() == '') {
                $.jGrowl("Please specify your Paypal email");
                return false;
            }

            $(this).buttonDisable();

            // post form
            $.ajaxswitch({
                type: "POST",
                url: action,
                data: serialized,
                success: function (result) {
                    if (typeof (result) == "string") {
                        $('#payment_methods').html(result);
                        dialogBox_close();
                    }
                    else {
                        $.jGrowl(result.message);
                    }
                }
            });
            return false;
        });

        // input highlighters
        inputSelectors_init();

    });
</script>
