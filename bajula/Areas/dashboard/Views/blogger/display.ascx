<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<IEnumerable<tradelr.Models.google.GoogleBlogData>>" %>
<div>
    <% if (Model.Count() != 0)
       {%>
    <p>
        Auto-posting to the following blogs</p>
    <%foreach (var blog in Model)
      {%>
    <p>
        <img src="/Content/img/social/icons/blogger_16.png" />
        <a href="<%= blog.blogHref %>" target="_blank"><%=blog.name%></a></p>
    <%}
   }
       else
       {%>
    <p>
        Not auto-posting to any blogs</p>
    <% }%>
</div>

