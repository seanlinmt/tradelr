<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<IEnumerable<ContactBasic>>" %>
<%@ Import Namespace="tradelr.Models.contacts" %>
<div class="fixedHeight scroll_y">
    <%
        foreach (var contact in ViewData.Model)
        {
    %>
    <div class="blockSelectable" title="<%= contact.address %>">
        <div class="fl w50px ac">
            <%= contact.profileThumbnail %>
        </div>
        <div class="content">
        <h4>
            <%= contact.companyName %></h4>
            <span class="firstname">
            <%= contact.firstName %></span> <span class="lastname">
                <%= contact.lastName %></span>
        </div>
         <span class="contactid hidden">
                    <%= contact.id %></span>
    </div>
    <%  } %>
</div>
<div class="clear">
</div>
