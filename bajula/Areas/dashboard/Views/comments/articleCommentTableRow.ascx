<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<tradelr.Areas.dashboard.Models.store.blog.ArticleComment>" %>
<%@ Import Namespace="tradelr.Email" %>
<%@ Import Namespace="tradelr.Libraries.Helpers" %>
<tr alt="<%= Model.id %>">
    <td>
        <div><%= Model.creator_name%></div>
        <%= Model.creator_email.ToMailToLink()%>
        <div class="smaller font_darkgrey"><%= Model.posted_time%></div>
    </td>
    <td>
        <%= Model.comment%>
    </td>
    <td class="ar">
        <% if (!Model.isReviewed)
            {%>
        <button type="button" class="small white approvelink" href="/dashboard/comments/<%= Model.id %>/approve">approve</button>
        <%} %>
        <% if (!Model.isSpam)
            {%>
        <button type="button" class="small white spamlink" href="/dashboard/comments/<%= Model.id %>/spam">spam</button>
        <%}
            else
            {%>
        <button type="button" class="small white unspamlink" href="/dashboard/comments/<%= Model.id %>/notspam">not spam</button>
        <%} %>
        <span class="hover_del" href="/dashboard/comments/<%= Model.id %>/delete"></span>
    </td>
</tr>
