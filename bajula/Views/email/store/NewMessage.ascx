<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<tradelr.Models.email.store.NewMessage>" %>
<p>The following message was received from <%= Model.name%> (<%= Model.email %>):</p>
<p><%= Model.message %></p>

