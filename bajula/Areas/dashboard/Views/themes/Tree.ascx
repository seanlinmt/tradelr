<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<tradelr.Areas.dashboard.Models.theme.ThemeTreeViewModel>" %>
<%@ Import Namespace="System.IO" %>
<ul class="jqueryFileTree" style="display: none;">
    <% foreach (var folder in Model.dir.GetDirectories().OrderByDescending(x => x.Name))
       {%>
    <li class="directory collapsed"><a href="#" rel="<%= Model.rootPath + folder.Name %>/">
        <%= folder.Name %></a></li>
    <% } %>
    <% foreach (FileInfo fi in Model.dir.GetFiles())
       {
           string ext = "";
           if (fi.Extension.Length > 1)
               ext = fi.Extension.Substring(1).ToLower();
    %>
    <li class="file ext_<%= ext %>"><a href="#" rel="<%= Model.rootPath + fi.Name %>">
        <%= fi.Name %></a></li>
    <%}%>
</ul>
