<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<tradelr.Areas.dashboard.Models.account.AccountViewModel>" %>
    <div class="form_group">
    <a href="#" id="payment_method_add" class="icon_add">add payment method</a>
    <ul id="payment_methods" class="mt20">
    <% Html.RenderAction("method_list", "payment"); %>
      </ul>
    </div>
<script type="text/javascript">
    var currencyval;

    $('#currency').live('click', function () {
        currencyval = $(this).val();
    });

    $('#currency').live('change', function () {
        var ok = window.confirm('Changing your currency will affect product pricing. Are you sure?');
        if (!ok) {
            $(this).val(currencyval);
        }
    });

    $('.hover_del', '#payment_methods').live('click', function () {
        var ok = window.confirm('Are you sure?');
        if (!ok) {
            return false;
        }
        var self = $(this).parents('li');
        var id = $(self).attr('alt');
        $.post('/dashboard/payment/method_del/' + id, function (json_result) {
            if (json_result.success) {
                $(self).slideUp(function () {
                    $(this).remove();
                });
            }
            $.jGrowl(json_result.message);
        });
        return false;
    });

    $('.hover_edit', '#payment_methods').live('click', function () {
        var id = $(this).parents('li').attr('alt');
        dialogBoxTitle_open('/dashboard/payment/method_edit/' + id,
        "<h3 class='font_white headingEdit'>payment method</h3>", 600, true);
        return false;
    });

    $('#payment_method_add').click(function () {
        dialogBoxTitle_open('/dashboard/payment/method_add', 
        "<h3 class='font_white' id='headingAdd'>payment method</h3>", 600, true);
        return false;
    });
</script>