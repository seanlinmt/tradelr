<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>
<p>Thank You for registering and welcome to tradelr.</p>
<strong>Verify Your Email</strong>
<p>To verify your email address and start using your new account, please follow the link below:</p>
<p>
<a href="<%= ViewData["host"] %>/register/verify?email=<%= ViewData["email"] %>&confirm=<%= ViewData["confirmCode"] %>"><%= ViewData["host"]%>/register/verify?email=<%= ViewData["email"]%>&confirm=<%= ViewData["confirmCode"]%></a>
</p>
<p>
If you are unable to click on the link above or if the link does not work, 
please copy and paste the link into your web browser.
</p>
<strong>Login Page</strong>
<p>To access your account, go to your login page below and login with your registered email address and password.</p>
<p><a href="<%= ViewData["host"] %>/login"><%= ViewData["host"]%>/login</a></p>
<p>
Regards,
<br />
The Tradelr Team
</p>
