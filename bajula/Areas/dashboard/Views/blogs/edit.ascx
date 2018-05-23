<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<tradelr.Areas.dashboard.Models.store.blog.Blog>" %>
<div class="mt10">
<h3 id="headingEdit"><%= string.IsNullOrEmpty(Model.title) ? "new blog":Model.title %></h3>
<form id="blogForm" autocomplete="off" method="post" action="<%= Url.Action("Save","Blogs") %>">
<% if (!string.IsNullOrEmpty(Model.title))
   {%>
       <p class="icon_info smaller">You can <a target="_blank" href="<%= Model.fullUrl %>">view your blog here</a></p>
  <% } %>
<div class="form_entry">
            <div class="form_label">
                <label for="title">
                    Title</label>
            </div>
            <%= Html.TextBox("title", Model.title, new Dictionary<string, object>(){{"class","w500px"}})%>
        </div>
        <div class="form_entry">
            <div class="form_label">
                <label for="title">
                    Comments</label>
            </div>
            <%= Html.DropDownList("comments", Model.commentsList, new Dictionary<string, object>(){{"class","w350px"}})%>
        </div>
<div class="mt10">
    <button id="buttonSave" type="button" class="green ajax">
        <img src="/Content/img/save.png" alt='' />
        save</button>
</div>
<%= Html.Hidden("id", Model.id) %>
</form>
</div>
<script type="text/javascript">
    $(document).ready(function () {
        $('#buttonSave', '#blogForm').click(function () {
            $(this).trigger('submit');
        });

        inputSelectors_init('#blogForm');
    });

    $('#blogForm').submit(function () {
        var action = $(this).attr("action");
        var ok = $('#blogForm').validate({
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
        $('#buttonSave', '#blogForm').buttonDisable();

        var serialized = $(this).serialize();

        // post form
        $.post(action, serialized, function (json_data) {
            if (json_data.success) {
                // close tab
                var selectedtab = $('#settings_tabs').tabs('option', 'selected');
                $('#settings_tabs').tabs("remove", selectedtab);

                // reload page tab
                $('#pages').load('/dashboard/pages', function () {
                    $('#settings_tabs').tabs("select", '#pages');
                });
            }
            $.jGrowl(json_data.message);
        }, 'json');
        return false;
    });
</script>
