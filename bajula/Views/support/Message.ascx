<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>
<%@ Import Namespace="tradelr.Libraries.Extensions" %>
<h3 id="headingSupport">support / feedback</h3>
<form id="supportForm" class="form_larger" action="/support/message" method="post">
<div class="form_entry">
    <div class="form_label">
        <label>
            Message</label></div>
    <%= Html.TextArea("message",new Dictionary<string, object>{{"style","min-height:100px"}}) %>
</div>
<div>
                <button id="buttonSend" type="button" class="green ajax">
                    send</button>
                    <button id="buttonCancel" type="button" class="ajax">
                    cancel</button>
            </div>
</form>
<%= Html.RegisterViewJS() %>
