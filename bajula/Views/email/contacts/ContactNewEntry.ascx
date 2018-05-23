<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<tradelr.Models.contacts.viewmodel.ContactNewViewModel>" %>
<p>
    <%= Model.creatorName %>(<%= Model.creatorEmail %>) has added you to their network
    at
    <a href="http://<%= Model.hostName %>"><%= Model.hostName %></a>.
</p>
<% if (string.IsNullOrEmpty(Model.password))
   { %>
<p>
    To view your profile, follow the link below:
</p>
<p>
    <%= Model.profile %>
</p>
<%}
   else
   { %>
<p>
    To log into your account, go to
    <a href="http://<%= Model.hostName %>/login">your login page</a>
    and log in with the following credentials:</p>
<table>
    <tr>
        <td>
            Email:
        </td>
        <td>
            <%= Model.email %>
        </td>
    </tr>
    <tr>
        <td>
            Password:
        </td>
        <td>
            <%= Model.password %>
        </td>
    </tr>
</table>
<%} %>
<% if (!string.IsNullOrEmpty(Model.note))
   {%>
<p>
    <%=Model.creatorName%>
    says:</p>
<p>
    <%=Model.note%></p>
<%
    }%>
<p>
    Regards,
    <br />
    The Tradelr Team
</p>
