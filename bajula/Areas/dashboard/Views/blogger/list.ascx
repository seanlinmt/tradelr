<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<List<tradelr.Models.google.GoogleBlogData>>" %>
<div>
<%if (Model.Count != 0)
  {%>
<p>Select blogs to link with tradelr</p>
    <%
    foreach (var blog in Model)
    {
%>
    <div class="blockSelectable">
        <h4>
            <a href="<%= blog.blogHref %>" target="_blank"><%= blog.name %></a></h4>
        <span class="address hidden">
            <%=blog.blogUri%></span>
    </div>
    <%
    }
  }
  else
  {%>
  <p>No blogs found</p>
  <%
  }%>
</div>
<div class="clear"></div>
<div class="mt10">
<button id="blogSave" class="green">continue</button>
</div>
