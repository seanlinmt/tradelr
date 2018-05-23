<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<tradelr.Models.google.gbase.GoogleBaseData>" %>
<div class="section_header">
    <img src="/Content/img/social/icons/google_16.png" alt="" />
    Google Merchant Center
</div>
<div class="form_group">
    <div id="connected" class="hidden">
        <p>
            <img src="/Content/img/tick.png" />
            Connected to Google Merchant Center</p>
            <p>If you have not obtained permission from Google to post through tradelr. <a target="_blank" href="http://blog.tradelr.com/google-base-is-now-google-merchant-center">Please follow the instructions in this blog entry</a></p>
            <div class="form_label">
                <label for="sellingPrice">
                    Target Country</label>
                <span class="tip">target country to post your products to</span>
            </div>
            <%= Html.DropDownList("targetCountry", Model.countries, new Dictionary<string, object>(){{"class","w200px"}})%>
        <div class="mt20">
            <button id="buttonSave" class="green ajax">
                save</button>
            <button id="buttonlogout" class="ajax">
                disconnect from google merchant center</button>
        </div>
    </div>
    <div id="notconnected" class="hidden">
        <div class="info">Existing products on Google Merchant Center will be imported and prices will be automatically converted to 
                your account currency. All future active products will automatically sync with Google Merchant Center.</div>
                <p>
            You will need to provide your Google Merchant Center account ID. If you haven't done so, please <a href="http://www.google.com/base/" target="_blank">create a Google Merchant Center account</a>.</p>
            <p>You should also <a target="_blank" href="http://support.google.com/merchants/bin/answer.py?hl=en&answer=188484">read the Google Product Search Policies</a>.</p>
                <div class="relative">
                <div class="fl">
                <div class="form_entry">
                        <div class="form_label">
                            <label for="merchantid">
                                Google Merchant Account ID</label>
                        </div>
                        <%= Html.TextBox("gbase_merchantid")%>
                    </div>
                <p><input type="checkbox" id="uploadexisting" name="upload" /><label for="uploadexisting">upload any existing active products not on Google Merchant Center</label></p>
                <div class="mt50">
                <button id="buttonlogin" type="button" class="green ajax">sync with google merchant center</button>
                </div>
                </div>
                <div id="gbase_accountid_photo" class="w500px fr hidden">
                <img class="img_border" src="/Content/img/networks/gbase_accountid.png" alt="" />
                </div>
                </div>
                
        
    </div>
</div>
<script type="text/javascript">
    $(document).ready(function () {
        inputSelectors_init();

        $('#gbase_merchantid').focus(function () {
            $('#gbase_accountid_photo').fadeIn();
        }).blur(function () {
            $('#gbase_accountid_photo').fadeOut();
        });
    });

    $('#buttonlogin').click(function () {
        var gbaseid = $('#gbase_merchantid').val();

        if (gbaseid == '') {
            $.jGrowl("Google Merchant Account ID is required");
            return;
        }

        $(this).buttonDisable();

        var requesturl = '/dashboard/gbase/getToken';
        var parameters = [];
        parameters["accountid"] = gbaseid;

        if ($('#uploadexisting').is(':checked')) {
            parameters["upload"] = true;
        }
        
        win1 = window.open(tradelr.util.buildUrl(requesturl, parameters), '', 'width=650px,height=500px,toolbar=0');
        check();
    });

    $('#buttonlogout').click(function () {
        $(this).buttonDisable();
        $.post('/dashboard/gbase/clearToken', null, function (json_result) {
            if (json_result.success && json_result.data) {
                $('#connected').fadeOut(function () {
                    $('#notconnected').fadeIn();
                });
            }
            else {
                $.jGrowl('Unable to disconnect from Google Merchant Center');
            }
            $('#buttonlogout').buttonEnable();
        }, 'json');
    });

    $('#buttonSave').click(function () {
        $(this).buttonDisable();
        var data = { country: $('#targetCountry').val() };
        $.post('/dashboard/gbase/settings', data, function (json_result) {
            $.jGrowl(json_result.message);
            $('#buttonSave').buttonEnable();
        }, 'json');
    });
    var win1;
    function check() {
        if (win1.closed) {
            $.post('/dashboard/gbase/haveToken', null, function (json_result) {
                if (json_result.success && json_result.data) {
                    $('#notconnected').fadeOut(function () {
                        $('#connected').fadeIn();
                    });
                }
                $('#buttonlogin').attr('disabled', false);
            }, 'json');
        } else {
            setTimeout("check()", 1);
        }
    }
    $(document).ready(function () {
        $.post('/dashboard/gbase/haveToken', null, function (json_result) {
            if (json_result.success) {
                if (json_result.data) {
                    $('#connected').show();
                }
                else {
                    $('#notconnected').show();
                }
            }
            else {
                $.jGrowl(json_result.message);
            }
        }, 'json');
    });
</script>
