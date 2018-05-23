<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<tradelr.Models.store.themes.ThemeViewModel>" %>
<form id="currentThemeForm" action="<%= Url.Action("Current","themes") %>" method="post">
<div id="themes_current" class="form_group">
<% Html.RenderPartial("current", Model.current); %>
</div>
<div class="section_header">Theme Gallery</div>
<ul id="themes_list" class="list_fl mt10">
<% foreach (var theme in Model.gallery) {%>
       <li class="ml20 mt20 opacity75"><img class="img_border pointer" src="<%= theme.thumbnail %>" alt="<%= theme.title %>"/></li>
 <%  } %>
</ul>
<div class="clear"></div>
<%= Html.Hidden("name") %>
</form>
<script type="text/javascript">
    $('li', '#themes_list').click(function () {
        var ok = window.confirm('Are you sure? This will delete all existing theme settings and files.');
        if (!ok) {
            return false;
        }
        $('li', '#themes_list').removeClass('opacity100');
        $(this).addClass('opacity100');
        var title = $('img', this).attr('alt');
        $('#name', '#currentThemeForm').val(title);
        $('#currentThemeForm').trigger('submit');
        return true;
    });
</script>