<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<tradelr.Areas.dashboard.Models.theme.ThemeEditorViewModel>" %>
<%@ Import Namespace="tradelr.Areas.dashboard.Models.theme" %>
<%@ Import Namespace="tradelr.Library" %>
<div id="theme_editor_<%= Model.themeType %>">
<div class="form_group">
    <div class="fl">
        <div class="form_entry w300px">
            <div class="form_label">
                <label>
                    Import</label>
                <span class="tip">Upload your own theme. Uploaded themes will be applied to your store
                    immediately.</span>
            </div>
            <div class="mt10 mb10">
                <div class="buttonUploadTheme">
                </div>
            </div>
        </div>
    </div>
    <div class="fl">
        <div class="form_entry ml100 w300px">
            <div class="form_label">
                <label>
                    Export / Backup</label>
                <span class="tip">Export or backup the current active theme. All theme settings and
                    files will be exported in a zip file.</span>
            </div>
            <div class="mt10 mb10">
                <form id="themeExportForm" class="post" action="<%= Url.Action("Export", "Themes") %>">
                <button id="buttonExportTheme" type="submit" class="">
                    export current theme</button>
                </form>
            </div>
        </div>
    </div>
    <div class="clear">
    </div>
</div>
<div class="section_header">
    Theme Editor</div>
<div class="bb">
    <div class="w200px fl overflow_bar theme_editor_files">
        <ul style="" class="jqueryFileTree">
            <li class="directory expanded"><a href="#" onclick="return false;">
                <%= Model.layout_folder.foldername %></a>
                <ul style="" class="jqueryFileTree">
                    <% foreach (var file in Model.layout_folder.files)
                       { %>
                    <li class="file <%= file.classname %>"><a rel="<%= file.url %>" href="#">
                        <%= file.filename %></a></li>
                    <%} %>
                </ul>
            </li>
            <li class="directory expanded"><a href="#" onclick="return false;">
                <%= Model.templates_folder.foldername%></a>
                <ul class="theme_templates jqueryFileTree">
                    <% foreach (var file in Model.templates_folder.files)
                       { %>
                    <li class="file <%= file.classname %>"><a rel="<%= file.url %>" href="#">
                        <%= file.filename %></a></li>
                    <%} %>
                </ul>
                <div class='theme_new_file_reveal new_template_reveal'>
                    <a href='#' onclick="$('.new_template_reveal', '#theme_editor_<%= Model.themeType %>').hide();$('.new_template', '#theme_editor_<%= Model.themeType %>').show();return false;">create new template</a></div>
                <div class='theme_new_file new_template'>
                    <p>create new template for</p>
                    <select class="new_template_select">
                        <option value="article">article</option>
                        <option value="blog">blog</option>
                        <option value="cart">cart</option>
                        <option value="collection">collection</option>
                        <option value="index">index</option>
                        <option value="page">page</option>
                        <option value="product">product</option>
                        <option value="search">search</option>
                        <option value="404">404</option>
                    </select>
                    <p>called</p>
                    <input class="new_template_name pad5.w150px" type="text" value="alt"/>
                    <p><button type="button" class="new_template_button small">create</button> or 
                    <a href="#" onclick="$('.new_template', '#theme_editor_<%= Model.themeType %>').hide();$('.new_template_reveal', '#theme_editor_<%= Model.themeType %>').show();return false;">cancel</a></p>
                    </div>
            </li>
            <li class="directory expanded"><a href="#" onclick="return false;"><%= Model.snippets_folder.foldername%></a>
                <ul class="theme_snippets jqueryFileTree">
                    <% foreach (var file in Model.snippets_folder.files)
                       { %>
                    <li class="file <%= file.classname %>"><a rel="<%= file.url %>" href="#">
                        <%= file.filename %></a></li>
                    <%} %>
                </ul>
                <div class='theme_new_file_reveal new_snippet_reveal'>
                    <a href='#' onclick="$('.new_snippet_reveal', '#theme_editor_<%= Model.themeType %>').hide();$('.new_snippet', '#theme_editor_<%= Model.themeType %>').show();return false;">create new snippet</a></div>
                <div class='theme_new_file new_snippet'>
                    <p>create new snippet called</p>
                    <input class="new_snippet_name w150px" type="text" value="alt"/>
                    <p><button type="button" class="new_snippet_button small">create</button> or 
                    <a href="#" onclick="$('.new_snippet', '#theme_editor_<%= Model.themeType %>').hide();$('.new_snippet_reveal', '#theme_editor_<%= Model.themeType %>').show();return false;">cancel</a></p>
                    </div>
            </li>
            <li class="directory expanded"><a href="#" onclick="return false;"><%= Model.asset_folder.foldername%></a>
                <ul class="theme_assets jqueryFileTree">
                    <% foreach (var file in Model.asset_folder.files)
                       { %>
                    <li class="file <%= file.classname %>"><a rel="<%= file.url %>" href="#">
                        <%= file.filename %></a></li>
                    <%} %>
                </ul>
                <div class='theme_new_file_reveal new_asset_reveal'>
                    <a href='#' onclick="$('new_asset_reveal', '#theme_editor_<%= Model.themeType %>').hide();$('.new_asset', '#theme_editor_<%= Model.themeType %>').show();return false;">upload an asset file</a></div>
                <div class='theme_new_file new_asset'>
                    <div class="buttonUploadAsset">
                </div>
                    <a href="#" onclick="$('.new_asset', '#theme_editor_<%= Model.themeType %>').hide();$('.new_asset_reveal', '#theme_editor_<%= Model.themeType %>').show();return false;">cancel</a>
                    </div>
            </li>
            <li class="directory expanded"><a href="#" onclick="return false;">
                <%= Model.config_folder.foldername%></a>
                <ul style="" class="jqueryFileTree">
                    <% foreach (var file in Model.config_folder.files)
                       { %>
                    <li class="file <%= file.classname %>"><a rel="<%= file.url %>" href="#">
                        <%= file.filename %></a></li>
                    <%} %>
                </ul>
            </li>
        </ul>
    </div>
    <div class="fr bg_lightgrey theme_editor_content" style="width: 790px;">
    </div>
    <div class="clear">
    </div>
</div>
<%= Html.Hidden("file_changed") %>
</div>
<script src="/jsapi/uploader" type="text/javascript"></script>
<script type="text/javascript">
    $(document).ready(function () {
        new LiquidEditorClass("#theme_editor_<%= Model.themeType %>", <% if(Model.themeType == ThemeType.MOBILE){%>true<%}else{%>false<%} %>);
    });
</script>
