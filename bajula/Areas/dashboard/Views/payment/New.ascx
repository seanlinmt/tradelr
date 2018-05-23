<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Payment>" %>
<%@ Import Namespace="tradelr.Models.payment"%>
<form id="paymentForm" action="<%= Url.Action("Create","Payment") %>" method="post">
    <%= Html.Hidden("orderID", Model.orderID)%>
    <%= Html.Hidden("orderType", Model.orderType)%>
    <h3 id="headingAdd">
     enter payment</h3>
<div class="fl w200px">
<div class="form_entry">
    <div class="form_label">
        <label for="amount" class="required">
            Amount Paid</label>
    </div>
    <%= Html.TextBox("amount") %>
</div>
<div class="form_entry">
    <div class="form_label">
        <label for="paymentDate" class="required">
            Payment Date</label>
    </div>
    <input id="paymentDate" name="paymentDate" style="width:148px" type="text" />
    <label for="paymentDate" class="m0"><img src="/Content/img/date.png" alt="" /></label>
</div>
<div class="form_entry">
    <div class="form_label">
        <label for="method" class="required">
            Payment Method</label>
    </div>
    <%= Html.DropDownList("method", Model.paymentMethods.items) %>
</div>
</div>
<div id="paymentTotalDisplay">
<div class="form_entry">
    <div class="form_label">
        <label>
            Total Unpaid (<%= Model.currencyCode %>)</label>
    </div>
    <div class="big font_serif font_darkred"><%= Model.totalUnpaid %></div>
</div>
</div>
<div class="clear"></div>
<div class="form_entry">
    <div class="form_label">
        <label for="notes">
            Other Payment Info</label>
            <span class="tip">Example: Online banking transaction id, Cheque Number</span>
    </div>
    <%= Html.TextArea("notes", Model.notes, new Dictionary<string, object> { { "style", "width:100%" } })%>
</div>

    <div class="pad5">
        <button id="buttonSave" type="button" class="ajax green">
            save payment</button>&nbsp;
        <button id="buttonCancel" type="button">
            cancel</button>
    </div>

</form>
<script type="text/javascript">
    $(document).ready(function () {
        $('#method', '#paymentForm');
        
        // handle manual payment bit
        $('#paymentDate', '#paymentForm').datepicker({
            dateFormat: 'd M yy'
        });

        $('#paymentDate', '#paymentForm').datepicker("setDate", new Date());

        $('#buttonSave', '#paymentForm').click(function () {
            $(this).trigger('submit');
        });

        $('#buttonCancel', '#paymentForm').click(function () {
            dialogBox_close();
        });

        $('#amount').numeric({ allow: '.' });

        $('#paymentForm').submit(function () {
            var action = $(this).attr("action");
            var serialized = $(this).serialize();
            var ok = $('#paymentForm').validate({
                invalidHandler: function (form, validator) {
                    $(validator.invalidElements()[0]).focus();
                },
                focusInvalid: false,
                rules: {
                    date: {
                        required: true
                    },
                    amount: {
                        required: true
                    },
                    method: {
                        required: true
                    }
                }
            }).form();
            
            if (!ok) {
                return false;
            }

            $(this).buttonDisable();
            
            // post form
            $.ajax({
                type: "POST",
                url: action,
                data: serialized,
                dataType: "json",
                success: function (json_data) {
                    if (json_data.success) {
                        $.jGrowl('Payment successfully saved');
                        if ($('#orderType','#paymentForm').val() == "ORDER") {
                            window.location = '/dashboard/orders/' + $('#orderID', '#paymentForm').val();
                        }
                        else {
                            window.location = '/dashboard/invoices/' + $('#orderID', '#paymentForm').val();
                        }
                        
                        dialogBox_close();
                    }
                    else {
                        $.jGrowl(json_data.message);
                    }
                    $('#buttonSave', '#paymentForm').buttonEnable();
                }
            });
            return false;
        });
        inputSelectors_init();
        init_autogrow('#paymentForm');
    });
</script>
