<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>
<%@ Import Namespace="tradelr.Libraries.Extensions" %>
<div class="content">
    <div class="fl w300px">
        <form id="loginForm" class="form_larger" action="/login" method="post">
        <h3 class="headingLogin">
            Sign In</h3>
        <div class="form_entry">
            <div class="form_label">
                <label for="email">
                    Email Address</label></div>
            <input type="text" name="email" id="email" />
        </div>
        <div class="form_entry">
            <div class="form_label">
                <label for="password">
                    Password</label></div>
            <input type="password" name="password" id="password" />
        </div>
        <div>
            <%= Html.CheckBox("signedin") %>
            <label class="pad0" for="signedin">
                Remember me on this pc</label>
        </div>
        <div id="checkedmessage" class="ba font_darkgrey pad5 smaller w200px hidden">
            <b>We'll keep you logged in</b>
            <br />
            Don't use this on public computers
        </div>
        <div class="mt10">
            <button id="buttonLogin" type="button" class="green ajax">
                sign in</button>
        </div>
        </form>
        <% Html.RenderControl(TradelrControls.forgotPass); %>
    </div>
</div>
<%= Html.RegisterViewJS() %>
