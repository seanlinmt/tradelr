<%@ Control Language="C#"  Inherits="System.Web.Mvc.ViewUserControl<tradelr.Models.history.ChangeHistory>" %>
<p><%= Model.documentType %> <%= Model.documentName %> has been updated. To view the updated version, please follow the link below:</p>
<p><a href="<%= Model.documentLoc %>"><%= Model.documentLoc %></a></p>

