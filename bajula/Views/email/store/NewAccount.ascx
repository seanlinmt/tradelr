<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Dictionary<string,object>>" %>
<p>Hi <%= Model["firstname"] %>,</p>
<p>Welcome to the store of <strong><%= Model["orgname"] %></strong></p>
<strong><%= Model["orgname"] %> Store</strong>
<p>The store is located at <a href="<%= Model["host"] %>"><%= Model["host"] %></a></p>
<p>
You can login to your account to check the status of your order at <a href="<%= Model["host"] %>/login"><%= Model["host"] %>/login</a>
</p>
