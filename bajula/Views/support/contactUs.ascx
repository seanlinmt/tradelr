<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>
<form id="supportForm" class="form_larger" action="/support/create" method="post">
<div class="form_entry">
    <div class="form_label">
        <label>
            Your First Name</label></div>
    <input type="text" name="firstname" id="firstname" />
</div>
<div class="form_entry">
    <div class="form_label">
        <label>
            Your Last Name</label></div>
    <input type="text" name="lastname" id="lastname" />
</div>
<div class="form_entry">
    <div class="form_label">
        <label class="required">
            Your Email</label></div>
    <input type="text" name="email" id="email" />
</div>
<div class="form_entry">
    <div class="form_label">
        <label>
            Your Message</label></div>
    <%= Html.TextArea("message",new Dictionary<string, object>{{"style","min-height:100px"}}) %>
</div>
<div>
                <button id="buttonSend" type="button" class="green ajax">
                    send</button>
            </div>
</form>
