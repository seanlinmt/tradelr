<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<tradelr.Areas.dashboard.Models.theme.ThemeSettingsViewModel>" %>
<%@ Import Namespace="tradelr.Areas.dashboard.Models.theme" %>
<%@ Import Namespace="tradelr.Library" %>
<div id="theme_settings_<%= Model.themeType %>">
<form autocomplete="off" id="ThemeSettingsForm" method="post" action="<%= Url.Action("settings","themes") %>"
enctype="multipart/form-data">
<div id="theme-settings">
    <% if (Model.presetList.Count != 0)
       { %>
    <div class="w300px fl smaller">
        <h3 class="mt50 small">
            Your current preset</h3>
        <p>
            Theme settings can be saved into predefined presets. This can be used to quickly
            switch between customized settings.</p>
        <%= Html.DropDownList("preset", Model.presetList)%>
    </div>
    <div class="w650px fr">
        <%=Model.SettingsHtml%>
    </div>
    <% }
       else
       { %>
    <%=Model.SettingsHtml%>
    <% } %>
    <div class="clear">
    </div>
    <div class="mt10 ar">
        <div>
            <%= Html.CheckBox("save_preset") %><label for="save_preset">save current settings as
                a preset</label></div>
        <div id="save_preset_div" class="hidden mt10">
            <label for="save_preset_existing">
                Save current theme settings as</label>
            <%= Html.DropDownList("save_preset_existing", new[] { new SelectListItem(){ Value = "new", Text = @"a new preset"} }.Union(Model.presetList))%>
            <label for="save_preset_custom">
                called</label><%= Html.TextBox("save_preset_custom") %>
        </div>
    </div>
    <div class="ar mt20">
        <button id="buttonSettingsSave" type="submit" class="large green">
            <img src="/Content/img/save.png" alt='' />
            update</button>
    </div>
</div>
<%= Html.Hidden("themeType", Model.themeType)%>
</form>
</div>
<script src="/jsapi/colorpicker" type="text/javascript"></script>
<script type="text/javascript">
$(document).ready(function() {
    new LiquidSettingsClass("#theme_settings_<%= Model.themeType %>", <%= Model.SettingsJson %>);
});
</script>
