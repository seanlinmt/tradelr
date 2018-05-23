<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<tradelr.Areas.dashboard.Models.store.blog.ArticleViewModel>" %>
<%@ Import Namespace="tradelr.Libraries.Helpers" %>
<div class="mt10">
    <h3 class="headingEdit">
        <%= string.IsNullOrEmpty(Model.article.title) ? "new blog article" : Model.article.title%></h3>
        <% if (Model.editMode)
   {%>
       <p class="icon_info smaller">You can <a target="_blank" href="<%= Model.article.fullUrl %>">view your article here</a></p>
  <% } %>
    <form class="mb50" autocomplete="off" id="articleForm" method="post" action="<%= Url.Action("ArticleSave","Blogs") %>">
    <div class="form_entry">
        <div class="form_label">
            <label for="title">
                Title</label>
        </div>
        <%= Html.TextBox("title", Model.article.title, new Dictionary<string, object>() { { "class", "w500px" } })%>
    </div>
    <div class="form_entry">
        <div class="form_label">
            <label for="title">
                Publish this article in</label>
        </div>
        <%= Html.DropDownList("target_blog", Model.blogList)%>
    </div>
    <div class="form_entry">
        <%= Html.TextArea("content", Model.article.content, 25, 80,
     new Dictionary<string, object> {
		{"class", "w700px tinymce"},
        {"data-mediapicker-uploadpath","blogs"},
		{"data-mediapicker-title","Insert/Update Media"}
	 })%>
    </div>
    <div class="form_entry">
        <%= Html.CheckBox("publish", Model.article.isPublish) %>
        <label for="visible">
            publish article</label>
    </div>
    <div class="form_entry">
        <div class="form_label">
            <label for="title">
                Tags</label>
        </div>
        <input type="text" id="tags" name="tags" value="<%= Model.article.tags %>" />
    </div>
    <div class="mt10">
        <button id="buttonSave" type="submit" class="large green ajax">
            <img src="/Content/img/save.png" alt='' />
            save</button>
    </div>
    <%= Html.Hidden("id", Model.article.id) %>
    </form>
    <% if (Model.editMode && Model.article.comment_count != 0)
       { %>
    <strong>Article Comments</strong>
    <table class="normal article_comments">
        <thead>
            <tr>
                <td>
                    Posted by
                </td>
                <td>
                    Comment
                </td>
                <td class="w150px">
                </td>
            </tr>
        </thead>
        <tbody>
            <% foreach (var comment in Model.article.comments.OrderByDescending(x => x.id))
               {
            Html.RenderPartial("~/Areas/dashboard/Views/comments/articleCommentTableRow.ascx", comment);
            } %>
        </tbody>
    </table>
    <% } %>
</div>
<script type="text/javascript" src="/Scripts/media/mediapicker.js"></script>
<script type="text/javascript">
    tinyMCE.init({
        height: "700",
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
    
    $('#tags', '#articleForm').tagsInput({
        autocomplete_url: '/tags/ArticleTags',
        autocomplete: {
            autoFill: true,
            selectFirst: false
        },
        width: '700px',
        height: '50px'
    });

    inputSelectors_init('#articleForm');
    
    $('#articleForm').submit(function () {
        var action = $(this).attr("action");
        var ok = $('#articleForm').validate({
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
        $('#buttonSave', '#articleForm').buttonDisable();

        var content = tinyMCE.get('content').getContent();
        $('#content', '#articleForm').val(content);
        
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
