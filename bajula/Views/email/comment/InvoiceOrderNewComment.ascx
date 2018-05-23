<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<CommentEmailContent>" %>
<%@ Import Namespace="tradelr.Models.comments"%>
<p><%= Model.creator %> has left a message for <%= Model.targetName %>.</p>
<p>"<%= Model.comment %>"</p>
<p>To view the message, follow the link below:</p>
<p><a href="<%= Model.commentsLink %>"><%= Model.commentsLink %></a></p>
