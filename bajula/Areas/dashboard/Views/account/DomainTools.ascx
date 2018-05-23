<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<IEnumerable<SelectListItem>>" %>
<form id="orderDomainForm" action="/dashboard/account/domainorder" method="POST" autocomplete="off" class="hidden">   
<div class="info">
<div>All prices are in <strong>United States Dollar, USD</strong></div>
<div>Once you have purchased your domain, you will be the legal owner of the registered domain.</div>
</div>
<div class="form_group">
<div class="form_entry">
    <div class="form_label">
    </div>
    <input type="text" id="domain_name" name="domain_name" class="w200px" /> 
    <%= Html.DropDownList("domain_ext", Model, new Dictionary<string, object>(){{"class","w60px"}}) %>
    <div class="mt10">
    <button id="buttonCheckDomainAvailable" type="button" class="ajax small">check availability</button>
    </div>
    <div id="domain_availability_result" class="mt10"></div>
</div>
</div>
<div id="select_domain_plan" class="hidden">
<% Html.RenderPartial("domainRegister"); %>
</div>
</form>
<script type="text/javascript">
    $('#domain_name').watermark("Enter desired domain name").alphanumeric();

    $('#buttonCheckDomainAvailable').click(function () {
        $(this).buttonDisable();
        $('#domain_availability_result').html('');
        $('#select_domain_plan').hide();
        var domain = $('#domain_name','#orderDomainForm').val();
        var ext = $('#domain_ext').val();
        $.post("/dashboard/account/domainlookup", { domain_name: domain + ext }, function (json_result) {
            if (json_result.success) {
                var code = json_result.data;
                if (code == null) {
                    $('#domain_availability_result').html("<span class='error_post'>Invalid domain name</span>");
                    return;
                }
                
                if (code.code == "210") {
                    // domain available
                    $('#domain_availability_result').html("<span class='ok_post'><strong>" + domain + ext + "</strong> is available</span>");
                    $('#select_domain_plan').slideDown();
                }
                else {
                    $('#domain_availability_result').html("<span class='error_post'><strong>" + domain + ext + "</strong> is not available.</span>");
                }

            }
        });
    });
</script>
