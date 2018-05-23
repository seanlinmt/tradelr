<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/NotLoggedIn.Master"
    Inherits="System.Web.Mvc.ViewPage<Register>" %>

<%@ Import Namespace="tradelr.Models.register" %>
<%@ Import Namespace="tradelr.Libraries.Extensions" %>
<asp:Content ID="Content2" ContentPlaceHolderID="head" runat="server">
Register
</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div id="banner">
        <div class="content">
            <h1>
                Account Sign Up</h1>
        </div>
    </div>
    <div class="banner_main">
        <div class="content">
            <div class="fl w500px">
                <form id="registerForm" class="form_content form_larger" action="<%= Url.Action("Create","Register") %>" method="post">
                <h3 class="headingRegister">
                    Create Your Tradelr Account</h3>
                <div class="form_entry">
                    <div class="form_label">
                        <label>
                            Subscription Plan</label>
                    </div>
                    <%= Model.planName %>
                    <input type="hidden" id="plan" name="plan" value="<%= Model.planName %>" />
                </div>
                <div class="form_entry">
                    <div class="form_label">
                        <label>
                            Affiliate ID</label>
                    </div>
                    <span class="tip">If you have one, specify it to get an extra 30 days free</span>
                    <input type="text" id="affiliate" name="affiliate" class="w250px" value=""/>
                </div>
                <div class="form_entry">
                    <div class="form_label">
                        <label for="loginPage">
                            Select Personal Site Address</label>
                    </div>
                    <span class="tip">Enter a name in the field below to create your unique site name. <strong>
                        Letters & numbers only.</strong></span>
                    <div id="subdomainInput">
                        <input type="text" class="loginPage" name="loginPage" /><span id="tradelr">.tradelr.com</span>
                    </div>
                    <div id="nameAvail" style="padding-top: 5px; height: 20px;">
                    </div>
                </div>
                <div id="signup_facebook" class="mt30">
                <p><span class="pointer" id="signin-facebook"></span><a class="fr" id="useemail" href="#">sign up with email</a></p>
                </div>
                <div id="signup_email" class="mt30 hidden">
                <div class="form_entry">
                    <div class="form_label">
                        <label for="email">
                            Your Email Address</label>
                            <span class="tip">Email address will be verified</span>
                            </div>
                    <input type="text" id="email" name="email" />
                </div>
                <div class="form_entry">
                    <div class="form_label">
                        <label for="password">
                            Your Password</label>
                        <span class="tip">Password must be 6 or more characters</span>
                    </div>
                    <input type="password" id="password" name="password" />
                </div>
                <div class="form_entry">
                    <div class="form_label">
                        <label for="passwordConfirm">
                            Enter Password Again</label></div>
                    <input type="password" id="passwordConfirm" name="passwordConfirm" />
                </div>
                <div class="mt30">
                    <button id="registerButton" type="button" class="green ajax">
                        register</button>
                        <button id="cancelButton" type="button" class="ajax">
                        cancel</button>
                </div>
                </div>
                </form>
            </div>
            <div class="clear">
            </div>
        </div>
    </div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="AdditionalJS" runat="server">
    <%= Html.RegisterViewJS() %>
</asp:Content>
