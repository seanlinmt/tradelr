<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<tradelr.Areas.dashboard.Models.store.page.Page>" %>
<%@ Import Namespace="tradelr.Library.Constants" %>
<div class="mt10">
<h3 class="headingEdit"><%= string.IsNullOrEmpty(Model.title) ? "new page":Model.title %></h3>
<form id="pageForm" method="post" action="<%= Url.Action("Save","Pages") %>">
<div class="form_entry">
    <div class="form_label">
        <label for="title">
            Title</label>
    </div>
    <%= Html.TextBox("title", Model.title, new Dictionary<string, object>(){{"class","w500px"}})%>
</div>
<div class="form_entry">
    <div class="form_label">
        <label for="template">
            Template</label>
            <span class="tip">Choose a template that will be used to display this page</span>
    </div>
    <%= Html.DropDownList("template", Model.templateList)%>
</div>
<% if (Model.editMode)
   {%>
<div class="form_entry">
    <div class="form_label">
        <label for="handle">
            Handle</label>
    </div>
    <%= Model.pageUrl  %><%= Html.TextBox("handle", Model.permalink)%>
</div>       
   <%} %>
<div class="form_entry">
<div class="form_label">
        <label for="content">
            Content</label>
    </div>
    <%= Html.TextArea("content", Model.content, 25, 80,
     new Dictionary<string, object> {
		{"class", "w700px tinymce"},
		{"data-mediapicker-uploadpath","pages"},
		{"data-mediapicker-title","Insert/Update Media"}
	 })%>
</div>
<div class="form_entry">
<%= Html.CheckBox("visible", Model.visible) %> <label for="visible">make this page visible</label>
</div>
<div class="mt10">
    <button id="buttonSave" type="submit" class="green ajax">
        <img src="/Content/img/save.png" alt='' />
        save</button>
</div>
<%= Html.Hidden("id", Model.id) %>
</form>
</div>
<script type="text/javascript" src="/Scripts/media/mediapicker.js"></script>
<script type="text/javascript">
    tinyMCE.init({
            height:"700",
            theme: "advanced",
            mode: "specific_textareas",
            editor_selector: "tinymce",
            plugins: "fullscreen,searchreplace,mediapicker",
            theme_advanced_toolbar_location: "top",
            theme_advanced_toolbar_align: "left",
            theme_advanced_buttons1: "search,replace,|,cut,copy,paste,|,undo,redo,|,mediapicker,|,link,unlink,charmap,emoticon,codeblock,|,bold,italic,|,numlist,bullist,formatselect,|,code,fullscreen",
            theme_advanced_buttons2: "",
            theme_advanced_buttons3: "",
            convert_urls: false,
            valid_elements: "*[*]",
        // shouldn't be needed due to the valid_elements setting, but TinyMCE would strip script.src without it.
            extended_valid_elements: "script[type|defer|src|language]"
        });

    inputSelectors_init('#pageForm');
    $('#title', '#pageForm').limit(100);

    $('#pageForm').submit(function () {
        var action = $(this).attr("action");
        var ok = $('#pageForm').validate({
            invalidHandler: function (form, validator) {
                $(validator.invalidElements()[0]).focus();
            },
            focusInvalid: false,
            rules: {
                title: {
                    required: true
                }
            }
        }).form();

        if (!ok) {
            return false;
        }
        $('#buttonSave', '#pageForm').buttonDisable();

        var content = tinyMCE.get('content').getContent();
        $('#content', '#pageForm').val(content);

        var serialized = $(this).serialize();

        // post form
        $.post(action, serialized, function (json_data) {
            if (json_data.success) {
                // close tab
                $('#settings_tabs').tabs("remove", tradelr.tabs.current_index);

                // reload page tab
                $('#pages').load('/dashboard/pages', function () {
                    $('#settings_tabs').tabs("select", tradelr.tabs.last_index);
                });
            }
            $.jGrowl(json_data.message);
        }, 'json');
        return false;
    });
</script>
