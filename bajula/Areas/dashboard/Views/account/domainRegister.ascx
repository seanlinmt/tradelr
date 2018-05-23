<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>
<div id="domain_name_plans" class="form_group">
<strong class="block">Select a Plan</strong>
    <div id="one_year" class="w200px fl">
        <input id="one_year_plan" name="plans" type="radio" value="one_year" />
        <label for="one_year_plan">
            1 year - $24.99/year
        </label>
    </div>
    <div id="two_year" class="w200px fl">
        <input id="two_year_plan" checked="checked" name="plans" type="radio" value="two_year" />
        <label for="two_year_plan">
            2 years - $19.99/year
            <br />
            (20% discount)</label>
    </div>
    <div class="clear pt10">
    </div>
    <div id="five_year" class="w200px fl">
        <input id="five_year_plan" name="plans" type="radio" value="five_year" />
        <label for="five_year_plan">
            5 years - $15.99/year
            <br />
            (36% discount)</label>
    </div>
    <div id="ten_year" class="w200px fl">
        <input id="ten_year_plan" name="plans" type="radio" value="ten_year" />
        <label for="ten_year_plan">
            10 years - $12.99/year
            <br />
            (48% discount)</label>
    </div>
    <div class="clear pt20">
    </div>
    <strong>Additional Features</strong>
    <div>
    <span class="tip">Prevents spammers from obtaining your contact details by hiding your personal information.</span>
        <%= Html.CheckBox("id_theft", true)%>
        <label for="id_theft">
            ID Theft Protection - $10.00/year</label>
        <div class="mt20 fr ar">
        <h2><span class="pr20">Total:</span>$<span id="total_domain_price">59.98</span></h2>
            <div class="mt10">
                <button id="buttonRegisterDomain" type="button" class="green ajax">
                    complete purchase</button>
            </div>
        </div>
    <div class="clear"></div>
    </div>
</div>
<script type="text/javascript">
    $('#domain_name_plans').delegate('input[type=radio],input[type=checkbox]', 'click', function () {
        var plan = $('input[name=plans]:checked').val();
        var id_theft = $('#id_theft', '#domain_name_plans').is(':checked');

        var total = 0;
        var years = 2;
        switch (plan) {
            case "one_year":
                years = 1;
                total = 24.99;
                break;
            case "two_year":
                years = 2;
                total = 39.98;
                break;
            case "five_year":
                years = 5;
                total = 79.95;
                break;
            case "ten_year":
                years = 10;
                total = 129.90;
                break;
        }

        if (id_theft) {
            total += (10 * years);
        }
        $('#total_domain_price').text(tradelr.util.tomoneystring(total, 2));
    });

    $('#buttonRegisterDomain').click(function () {
        $(this).buttonDisable();
        $('#orderDomainForm').trigger('submit');
    });

    $('#orderDomainForm').submit(function () {
        $.jGrowl("Please wait while we redirect you to your payment page", {sticky : true});
        var action = $(this).attr("action");
        var serialized = $(this).serialize();
        $.post(action, serialized, function (json_data) {
            if (json_data.success) {
                window.location = json_data.data;
            }
            else {
                // only possible error is profile not filled out
                $('#content_main').tabs('select', 0);
                $.jGrowl(json_data.message);
            }
        });
        return false;
    });
</script>
