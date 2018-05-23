<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<tradelr.Areas.dashboard.Models.theme.LiquidFileContent>" %>
<div class="fr pad5">
        <button type="button" class="buttonEditorSave green ajax">
            <img src="/Content/img/save.png" alt='' />
            save changes</button>
    </div>
    <div class="fl pad5 ml20 w600px">
    <h4><%= Model.filename %> </h4>
    <a rel="<%= Model.url %>" class="theme_file_delete icon_del smaller pointer" title="delete file">delete</a>
    </div>
    <div class="clear"></div>
<%= Model.content %>
