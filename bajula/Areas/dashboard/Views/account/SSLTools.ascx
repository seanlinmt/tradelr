<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>
<form id="orderSSLForm" action="/dashboard/account/sslorder" method="POST" autocomplete="off" class="hidden">   
<div class="info">
<div>All prices are in <strong>United States Dollar, USD</strong></div>
</div>
<div class="form_group">
<ul>
<li><input id="one_year_ssl" name="plans_ssl" type="radio" value="one_year_ssl" />
        <label for="one_year_ssl">
            GeoTrust Quick SSL 1 year - $99.99
        </label></li>
<li><input id="two_year_ssl" checked="checked" name="plans_ssl" type="radio" value="two_year_ssl" />
        <label for="two_year_ssl">
            GeoTrust Quick SSL 2 years - $158.40
            (20% discount)</label></li>
<li><input id="three_year_ssl" name="plans_ssl" type="radio" value="three_year_ssl" />
        <label for="three_year_ssl">
            GeoTrust Quick SSL 3 years - $244.98
            (25% discount)</label></li>
<li><input id="four_year_ssl" name="plans_ssl" type="radio" value="four_year_ssl" />
        <label for="four_year_ssl">
            GeoTrust Quick SSL 4 years - $279.98
            (30% discount)</label></li>
<li><input id="five_year_ssl" name="plans_ssl" type="radio" value="five_year_ssl" />
        <label for="five_year_ssl">
            GeoTrust Quick SSL 5 years - $324.98
            (35% discount)</label></li>
</ul>
<div id="domain_name_plans" class="form_group">
    <div class="mt10">
        <button id="buttonRegisterSSL" type="button" class="green ajax">
            complete purchase</button>
    </div>
</div>
</div>
</form>
<script type="text/javascript">
    $('#buttonRegisterSSL').click(function () {
        $(this).buttonDisable();
        $('#orderSSLForm').trigger('submit');
    });

    $('#orderSSLForm').submit(function () {
        $.jGrowl("Please wait while we redirect you to your payment page", { sticky: true });
        var action = $(this).attr("action");
        var serialized = $(this).serialize();
        $.post(action, serialized, function (json_data) {
            if (json_data.success) {
                window.location = json_data.data;
            }
            else {
                $.jGrowl(json_data.message);
            }
        });
        return false;
    });
</script>
