<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>
<%@ Import Namespace="tradelr.Libraries.Extensions" %>
<div class="mt20">
    <div class="pt5">
        <a href="#" id="LinkGetPass" class="icon_help">I forgot my password</a></div>
    <form id="passwordForm" action="/login/forgotpass" method="post" style="display: none;">
    <div class="form_entry">
        <div class="form_label">
        </div>
        <input type="text" name="myemail" id="myemail" />
        <div class="mt10">
            <button id="buttonGetNewPass" type="button" class="green ajax small">
                reset password</button>
        </div>
    </div>
    </form>
</div>
<%= Html.RegisterViewJS() %>